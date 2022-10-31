using System;
using System.Collections.Generic;
using System.Configuration;
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
    public class FlightReportServices : IFlightReportServices
    {
        private readonly UnitOfWork _unitOfWork;

        public FlightReportServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<AirFare> GetAll()
        {
            List<AirFare> airfareEntity = new List<AirFare>();
            var flightbookingList = _unitOfWork.FlightBookingRepository.GetAll().OrderByDescending(x => x.bookingDate).ToList();
            foreach (var flight in flightbookingList)
            {
                AirFare airfare = new AirFare();
                airfare.bookingOID = flight.BookingOID.ToString();
                airfare.noOfAdults = flight.NoOfAdults ?? 0;
                airfare.noOfChildren = flight.NoOfChildren ?? 0;
                airfare.noOfInfants = flight.NoOfInfants ?? 0;
                airfare.svc_class = flight.Svc_class;
                airfare.grandTotal = flight.GrandTotal ?? 0;
                airfare.isPassportRequired = flight.IsPassportRequired ?? false;
                airfare.PNR = flight.PNR;
                airfare.Platform = flight.Platform;
                airfare.medId = flight.MedId ?? 0;
                airfare.priceChange = flight.PriceChange ?? false;
                airfare.oldPrice = flight.OldPrice ?? 0;
                airfare.TKTL = flight.TKTL ?? DateTime.MinValue;
                airfare.bookingDate = flight.bookingDate ?? DateTime.MinValue;
                //airfare.remarks = flightBooking.Remarks;
                airfare.isError = flight.IsError ?? false;
                airfare.errorMessage = flight.ErrorMessage;
                airfare.statusPayment = flight.StatusPayment ?? 0;
                airfare.RobinhoodID = flight.RobinhoodID;
                airfare.bookingURN = flight.BookingURN;
                if (flight.PromotionGroupCode != null && flight.PromotionGroupCode.Length > 0)
                {
                    airfare.promotionGroupCode = new List<string>();
                    if (flight.PromotionGroupCode.IndexOf("|") != -1)
                    {
                        string[] arr = flight.PromotionGroupCode.Split('|');
                        foreach (var _arr in arr)
                        {
                            airfare.promotionGroupCode.Add(_arr);
                        }
                    }
                    else
                    {
                        airfare.promotionGroupCode.Add(flight.PromotionGroupCode);
                    }
                }
                airfare.statusBooking = flight.StatusBooking ?? 0;
                airfare.paymentMethod = flight.PaymentMethod ?? 0;
                airfare.isBundle = flight.IsBundle ?? false;
                airfare.sourceBy = flight.sourceBy ?? 0;
                airfare.finalPrice = flight.FinalPrice ?? 0;

                airfare.NFTFee = flight.NFTFee ?? 0;
                airfare.Wallet_Address = flight.Wallet_Address;

                airfare.contactInfo = new ContactInfo();
                ContactInfo contactInfo = new ContactInfo();
                contactInfo = new ContactInfo();
                contactInfo.title = flight.Title;
                contactInfo.firstname = flight.Firstname;
                contactInfo.middlename = flight.Middlename;
                contactInfo.lastname = flight.Lastname;
                contactInfo.email = flight.Email;
                contactInfo.telNo = flight.TelNo;
                contactInfo.countryCode = flight.CountryOfResidence;
                airfare.contactInfo = contactInfo;


                FlightDetail detail = new FlightDetail();
                NamingServices _namingServices = null;
                Airport airportdepart = new Airport(_namingServices, true, "en");
                Airline airlinedepart = new Airline(_namingServices, "en");
                Airport airportreturn = new Airport(_namingServices, true, "en");
                Airline airlinereturn = new Airline(_namingServices, "en");
                airfare.depFlight = new List<FlightDetail>();
                airfare.retFlight = new List<FlightDetail>();
                var flightbookingflightdetaildepartList = _unitOfWork.FlightBookingFlightDetailRepository.GetMany(x => x.BookingOID == flight.BookingOID && x.TripType == "depart").OrderBy(x => x.Seq).ToList();
                foreach (var flightdetail in flightbookingflightdetaildepartList)
                {
                    detail = new FlightDetail();
                    airportdepart = new Airport(_namingServices, true, "en");
                    airportdepart.code = flightdetail.DepCity;
                    detail.depCity = airportdepart;
                    airportreturn = new Airport(_namingServices, true, "en");
                    airportreturn.code = flightdetail.ArrCity;
                    detail.arrCity = airportreturn;
                    airlinedepart = new Airline(_namingServices, "en");
                    airlinedepart.code = flightdetail.Airline;
                    detail.airline = airlinedepart;
                    airlinereturn = new Airline(_namingServices, "en");
                    airlinereturn.code = flightdetail.OperatedAirline;
                    detail.operatedAirline = airlinereturn;

                    //airportdepart.code = flightdetail.DepCity;
                    //detail.depCity = airportdepart;
                    detail.departureDateTime = flightdetail.DepartureDateTime ?? DateTime.MinValue;
                    //airportreturn.code = flightdetail.ArrCity;
                    //detail.arrCity = airportreturn;
                    detail.arrivalDateTime = flightdetail.ArrivalDateTime ?? DateTime.MinValue;
                    detail.Seq = flightdetail.Seq ?? 0;
                    detail.controlNumber = flightdetail.ControlNumber;
                    airfare.depFlight.Add(detail);
                }

                Airport airportdepart1 = new Airport(_namingServices, true, "en");
                Airline airlinedepart1 = new Airline(_namingServices, "en");
                Airport airportreturn1 = new Airport(_namingServices, true, "en");
                Airline airlinereturn1 = new Airline(_namingServices, "en");
                var flightbookingflightdetailreturnList = _unitOfWork.FlightBookingFlightDetailRepository.GetMany(x => x.BookingOID == flight.BookingOID && x.TripType == "return").OrderBy(x => x.Seq).ToList();
                foreach (var flightdetail in flightbookingflightdetailreturnList)
                {
                    detail = new FlightDetail();
                    airportdepart1 = new Airport(_namingServices, true, "en");
                    airportdepart1.code = flightdetail.DepCity;
                    detail.depCity = airportdepart1;
                    airportreturn1 = new Airport(_namingServices, true, "en");
                    airportreturn1.code = flightdetail.ArrCity;
                    detail.arrCity = airportreturn1;
                    airlinedepart1 = new Airline(_namingServices, "en");
                    airlinedepart.code = flightdetail.Airline;
                    detail.airline = airlinedepart;
                    airlinereturn1 = new Airline(_namingServices, "en");
                    airlinereturn.code = flightdetail.OperatedAirline;
                    detail.operatedAirline = airlinereturn;

                    //airportdepart1.code = flightdetail.DepCity;
                    //detail.depCity = airportdepart1;
                    detail.departureDateTime = flightdetail.DepartureDateTime ?? DateTime.MinValue;
                    //airportreturn1.code = flightdetail.ArrCity;
                    //detail.arrCity = airportreturn1;
                    detail.arrivalDateTime = flightdetail.ArrivalDateTime ?? DateTime.MinValue;
                    detail.Seq = flightdetail.Seq ?? 0;
                    detail.controlNumber = flightdetail.ControlNumber;
                    airfare.retFlight.Add(detail);
                }

                List<FlightBookingFlightDetail> flightdetailmultiList = _unitOfWork.FlightBookingFlightDetailRepository.GetMany(x => x.BookingOID == flight.BookingOID && x.TripType.IndexOf("multi") != -1).OrderBy(x => x.TripType).ToList();
                List<FlightBookingFlightDetail> flightdetailmulti = new List<FlightBookingFlightDetail>();
                FlightDetail flightmulti = new FlightDetail();
                List<FlightDetail> multiList = new List<FlightDetail>();
                airfare.multiFlight = new List<List<FlightDetail>>();
                for (int i = 0; i < 6; i++)
                {
                    flightdetailmulti = flightdetailmultiList.FindAll(x => x.TripType == ("multi" + (i + 1)));
                    if (flightdetailmulti != null && flightdetailmulti.Count > 0)
                    {
                        multiList = new List<FlightDetail>();
                        flightdetailmulti = flightdetailmulti.OrderBy(x => x.Seq).ToList();
                        foreach (var flightdetail in flightdetailmulti)
                        {
                            flightmulti = new FlightDetail();
                            airportdepart1 = new Airport(_namingServices, true, "en");
                            airportdepart1.code = flightdetail.DepCity;
                            flightmulti.depCity = airportdepart1;
                            airportreturn1 = new Airport(_namingServices, true, "en");
                            airportreturn1.code = flightdetail.ArrCity;
                            flightmulti.arrCity = airportreturn1;
                            airlinedepart1 = new Airline(_namingServices, "en");
                            airlinedepart.code = flightdetail.Airline;
                            flightmulti.airline = airlinedepart;
                            airlinereturn1 = new Airline(_namingServices, "en");
                            airlinereturn.code = flightdetail.OperatedAirline;
                            flightmulti.operatedAirline = airlinereturn;

                            //airportdepart1.code = flightdetail.DepCity;
                            //detail.depCity = airportdepart1;
                            flightmulti.departureDateTime = flightdetail.DepartureDateTime ?? DateTime.MinValue;
                            //airportreturn1.code = flightdetail.ArrCity;
                            //detail.arrCity = airportreturn1;
                            flightmulti.arrivalDateTime = flightdetail.ArrivalDateTime ?? DateTime.MinValue;
                            flightmulti.Seq = flightdetail.Seq ?? 0;
                            flightmulti.controlNumber = flightdetail.ControlNumber;
                            multiList.Add(flightmulti);
                        }
                        airfare.multiFlight.Add(multiList);
                    }
                }

                airfare.adtPaxs = new List<PaxInfo>();
                PaxInfo paxinfodetail = new PaxInfo();
                var flightbookinfpaxifo = _unitOfWork.FlightBookingPaxInfoRepository.GetMany(x => x.BookingOID == flight.BookingOID && x.PaxType == "ADT").ToList();
                foreach (var paxinfo in flightbookinfpaxifo)
                {
                    paxinfodetail = new PaxInfo();
                    paxinfodetail.title = paxinfo.Title;
                    paxinfodetail.firstname = paxinfo.Firstname;
                    paxinfodetail.middlename = paxinfo.Middlename;
                    paxinfodetail.lastname = paxinfo.Lastname;
                    paxinfodetail.email = paxinfo.Email;
                    paxinfodetail.telNo = paxinfo.TelNo;
                    airfare.adtPaxs.Add(paxinfodetail);
                }
                //airfare.adtFare = new Fare();
                //Fare adtFaredetail = new Fare();
                //var flightbookingfare = _unitOfWork.FlightBookingFareRepository.GetMany(x => x.BookingOID == flight.BookingOID && x.PessengerType == "adtFare").ToList();
                //foreach (var adtfare in flightbookingfare)
                //{
                //    adtFaredetail = new Fare();
                //    adtFaredetail.baseFare = adtfare.BaseFare ?? 0;
                //    adtFaredetail.sellingBaseFare = adtfare.SellingBaseFare ?? 0;
                //    adtFaredetail.tax = adtfare.Tax ?? 0;
                //    adtFaredetail.qtax = adtfare.Qtax ?? 0;
                //    adtFaredetail.lessFare = adtfare.LessFare ?? 0;
                //    airfare.adtFare = adtFaredetail;
                //}

                airfare.refund = new BL.Entities.RobinhoodFare.AirFare.Refund();
                BL.Entities.RobinhoodFare.AirFare.Refund refunddetail = new BL.Entities.RobinhoodFare.AirFare.Refund();
                var flightbookrefund = _unitOfWork.FlightBookingRefundRepository.GetMany(x => x.BookingOID == flight.BookingOID).ToList();
                foreach (var refund in flightbookrefund)
                {
                    refunddetail = new BL.Entities.RobinhoodFare.AirFare.Refund();
                    refunddetail.status = refund.Status ?? 0;
                    refunddetail.refundNo = refund.RefundNo;
                    airfare.refund = refunddetail;
                }

                airfare.reissue = new BL.Entities.RobinhoodFare.AirFare.Reissue();
                BL.Entities.RobinhoodFare.AirFare.Reissue reissuedetail = new BL.Entities.RobinhoodFare.AirFare.Reissue();
                var flightbookreissue = _unitOfWork.FlightBookingReissueRepository.GetMany(x => x.BookingOID == flight.BookingOID).ToList();
                foreach (var reissue in flightbookreissue)
                {
                    reissuedetail = new BL.Entities.RobinhoodFare.AirFare.Reissue();
                    reissuedetail.status = reissue.Status ?? 0;
                    airfare.reissue = reissuedetail;
                }
             
                airfareEntity.Add(airfare);
            }


            return airfareEntity;
            //return _unitOfWork.FlightBookingRepository.GetAll().OrderBy(x => x.TKTL).ToList();
        }

        public List<AirFare.Refund> GetAllRefund()
        {
            List<AirFare.Refund> airfareEntity = new List<AirFare.Refund>();
            var flightbookrefund = _unitOfWork.FlightBookingRefundRepository.GetAll().OrderByDescending(x => x.RefundCreateDate).ToList();
            foreach (var flight in flightbookrefund)
            {
                AirFare.Refund airfare = new AirFare.Refund();
                airfare.FlightBookingRefundOID = flight.BookingOID;
                airfare.refundCreateDate = flight.RefundCreateDate ?? DateTime.MinValue;
                airfare.dueDateOfRefund = flight.DueDateOfRefund ?? DateTime.MinValue;
                airfare.status = flight.Status ?? 0;
                airfare.refundNo = flight.RefundNo;
                airfare.newPNR = flight.NewPNR;
                airfareEntity.Add(airfare);
            }


            return airfareEntity;
            //return _unitOfWork.FlightBookingRepository.GetAll().OrderBy(x => x.TKTL).ToList();
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

        public List<AirFare> GetAllBysearch(DateTime? StartDate, DateTime? FinishDate, DateTime? Paybefore_StartDate, DateTime? Paybefore_FinishDate, String StatusBooking, String PaymentMethod, String Platform, String ShowListNo, String faresfrom)
        {
            List<AirFare> airfareEntity = new List<AirFare>();

            if (ShowListNo != "0" && ShowListNo != null)
            {
                IEnumerable<FlightBooking> flightbookingList = _unitOfWork.FlightBookingRepository.GetAll().OrderByDescending(x => x.bookingDate).Take(Convert.ToInt32(ShowListNo));
                if(faresfrom != "00")
                {
                    flightbookingList = flightbookingList.Where(x => x.type == faresfrom).ToList();
                }
                if (StartDate != null && StartDate.ToString() != "" && FinishDate != null && FinishDate.ToString() != "")
                {
                    flightbookingList = flightbookingList.Where(x => x.bookingDate >= StartDate && x.bookingDate <= FinishDate).ToList();
                }
                if (Paybefore_StartDate != null && Paybefore_StartDate.ToString() != "" && Paybefore_FinishDate != null && Paybefore_FinishDate.ToString() != "")
                {
                    flightbookingList = flightbookingList.Where(x => x.TKTL >= Paybefore_StartDate && x.TKTL <= Paybefore_FinishDate).ToList();
                }
                if (StatusBooking != "00")
                {
                    flightbookingList = flightbookingList.Where(x => x.StatusBooking == Convert.ToInt32(StatusBooking)).ToList();
                }

                if (PaymentMethod != "00")
                {
                    flightbookingList = flightbookingList.Where(x => x.PaymentMethod == Convert.ToInt32(PaymentMethod)).ToList();
                }

                if (Platform != "00")
                {
                    flightbookingList = flightbookingList.Where(x => x.Platform == Platform).ToList();
                }

                //flightbookingList.OrderByDescending(x => x.TKTL);
                foreach (var flight in flightbookingList)
                {
                    AirFare airfare = new AirFare();
                    airfare.bookingOID = flight.BookingOID.ToString();
                    airfare.noOfAdults = flight.NoOfAdults ?? 0;
                    airfare.noOfChildren = flight.NoOfChildren ?? 0;
                    airfare.noOfInfants = flight.NoOfInfants ?? 0;
                    airfare.svc_class = flight.Svc_class;
                    airfare.grandTotal = flight.GrandTotal ?? 0;
                    airfare.isPassportRequired = flight.IsPassportRequired ?? false;
                    airfare.PNR = flight.PNR;
                    airfare.Platform = flight.Platform;
                    airfare.medId = flight.MedId ?? 0;
                    airfare.priceChange = flight.PriceChange ?? false;
                    airfare.oldPrice = flight.OldPrice ?? 0;
                    airfare.TKTL = flight.TKTL ?? DateTime.MinValue;
                    airfare.bookingDate = flight.bookingDate ?? DateTime.MinValue;
                    //airfare.remarks = flightBooking.Remarks;
                    airfare.isError = flight.IsError ?? false;
                    airfare.errorMessage = flight.ErrorMessage;
                    airfare.statusPayment = flight.StatusPayment ?? 0;
                    airfare.RobinhoodID = flight.RobinhoodID;
                    airfare.statusBooking = flight.StatusBooking ?? 0;
                    airfare.paymentMethod = flight.PaymentMethod ?? 0;
                    airfare.sourceBy = flight.sourceBy ?? 0;
                    airfare.isBundle = flight.IsBundle ?? false;

                    ContactInfo contactInfo = new ContactInfo();
                    var contactList = _unitOfWork.FlightBookingRepository.GetMany(x => x.BookingOID == flight.BookingOID).ToList();
                    foreach (var contact in contactList)
                    {
                        contactInfo = new ContactInfo();
                        contactInfo.title = contact.Title;
                        contactInfo.firstname = contact.Firstname;
                        contactInfo.middlename = contact.Middlename;
                        contactInfo.lastname = contact.Lastname;
                        contactInfo.email = contact.Email;
                        contactInfo.telNo = contact.TelNo;
                        contactInfo.countryCode = contact.CountryOfResidence;
                        airfare.contactInfo = contactInfo;
                    }

                    airfare.installmentMonthlyPaid = flight.InstallmentMonthly.GetValueOrDefault();
                    airfare.installmentPlan = flight.InstallmentPlan.GetValueOrDefault();
                    airfare.finalPrice = flight.FinalPrice.GetValueOrDefault();

                    airfare.promotionCode = flight.PromoCode;
                    airfare.promotionDiscount = flight.DiscountAmount.GetValueOrDefault();
                    airfare.promotionName = "";

                    FlightDetail detail = new FlightDetail();
                    NamingServices _namingServices = null;
                    Airport airportdepart = new Airport(_namingServices, true, "en");
                    Airline airlinedepart = new Airline(_namingServices, "en");
                    Airport airportreturn = new Airport(_namingServices, true, "en");
                    Airline airlinereturn = new Airline(_namingServices, "en");
                    airfare.depFlight = new List<FlightDetail>();
                    airfare.retFlight = new List<FlightDetail>();
                    var flightbookingflightdetaildepartList = _unitOfWork.FlightBookingFlightDetailRepository.GetMany(x => x.BookingOID == flight.BookingOID && x.TripType == "depart").OrderBy(x => x.Seq).ToList();
                    foreach (var flightdetail in flightbookingflightdetaildepartList)
                    {
                        detail = new FlightDetail();
                        airportdepart = new Airport(_namingServices, true, "en");
                        airportdepart.code = flightdetail.DepCity;
                        detail.depCity = airportdepart;
                        airportreturn = new Airport(_namingServices, true, "en");
                        airportreturn.code = flightdetail.ArrCity;
                        detail.arrCity = airportreturn;
                        airlinedepart = new Airline(_namingServices, "en");
                        airlinedepart.code = flightdetail.Airline;
                        detail.airline = airlinedepart;
                        airlinereturn = new Airline(_namingServices, "en");
                        airlinereturn.code = flightdetail.OperatedAirline;
                        detail.operatedAirline = airlinereturn;

                        //airportdepart.code = flightdetail.DepCity;
                        //detail.depCity = airportdepart;
                        detail.departureDateTime = flightdetail.DepartureDateTime ?? DateTime.MinValue;
                        //airportreturn.code = flightdetail.ArrCity;
                        //detail.arrCity = airportreturn;
                        detail.arrivalDateTime = flightdetail.ArrivalDateTime ?? DateTime.MinValue;
                        detail.Seq = flightdetail.Seq ?? 0;
                        airfare.depFlight.Add(detail);
                    }

                    Airport airportdepart1 = new Airport(_namingServices, true, "en");
                    Airline airlinedepart1 = new Airline(_namingServices, "en");
                    Airport airportreturn1 = new Airport(_namingServices, true, "en");
                    Airline airlinereturn1 = new Airline(_namingServices, "en");
                    var flightbookingflightdetailreturnList = _unitOfWork.FlightBookingFlightDetailRepository.GetMany(x => x.BookingOID == flight.BookingOID && x.TripType == "return").OrderBy(x => x.Seq).ToList();
                    foreach (var flightdetail in flightbookingflightdetailreturnList)
                    {
                        detail = new FlightDetail();
                        airportdepart1 = new Airport(_namingServices, true, "en");
                        airportdepart1.code = flightdetail.DepCity;
                        detail.depCity = airportdepart1;
                        airportreturn1 = new Airport(_namingServices, true, "en");
                        airportreturn1.code = flightdetail.ArrCity;
                        detail.arrCity = airportreturn1;
                        airlinedepart1 = new Airline(_namingServices, "en");
                        airlinedepart.code = flightdetail.Airline;
                        detail.airline = airlinedepart;
                        airlinereturn1 = new Airline(_namingServices, "en");
                        airlinereturn.code = flightdetail.OperatedAirline;
                        detail.operatedAirline = airlinereturn;

                        //airportdepart1.code = flightdetail.DepCity;
                        //detail.depCity = airportdepart1;
                        detail.departureDateTime = flightdetail.DepartureDateTime ?? DateTime.MinValue;
                        //airportreturn1.code = flightdetail.ArrCity;
                        //detail.arrCity = airportreturn1;
                        detail.arrivalDateTime = flightdetail.ArrivalDateTime ?? DateTime.MinValue;
                        detail.Seq = flightdetail.Seq ?? 0;
                        airfare.retFlight.Add(detail);
                    }

                    airfare.adtPaxs = new List<PaxInfo>();
                    PaxInfo paxinfodetail = new PaxInfo();
                    var flightbookinfpaxifo = _unitOfWork.FlightBookingPaxInfoRepository.GetMany(x => x.BookingOID == flight.BookingOID && x.PaxType == "ADT").ToList();
                    foreach (var paxinfo in flightbookinfpaxifo)
                    {
                        paxinfodetail = new PaxInfo();
                        paxinfodetail.firstname = paxinfo.Firstname;
                        paxinfodetail.middlename = paxinfo.Middlename;
                        paxinfodetail.lastname = paxinfo.Lastname;
                        paxinfodetail.email = paxinfo.Email;
                        paxinfodetail.telNo = paxinfo.TelNo;
                        airfare.adtPaxs.Add(paxinfodetail);
                    }

                    if (airfare.refund != null)
                    {
                        airfare.refund = new BL.Entities.RobinhoodFare.AirFare.Refund();
                        BL.Entities.RobinhoodFare.AirFare.Refund refunddetail = new BL.Entities.RobinhoodFare.AirFare.Refund();
                        var flightbookrefund = _unitOfWork.FlightBookingRefundRepository.GetMany(x => x.BookingOID == flight.BookingOID).ToList();
                        foreach (var refund in flightbookrefund)
                        {
                            refunddetail = new BL.Entities.RobinhoodFare.AirFare.Refund();
                            refunddetail.status = refund.Status ?? 0;
                            refunddetail.refundNo = refund.RefundNo;
                            airfare.refund = refunddetail;
                        }
                    }


                    airfareEntity.Add(airfare);
                }
            }
            else
            {
                IEnumerable<FlightBooking> flightbookingList = _unitOfWork.FlightBookingRepository.GetAll().OrderByDescending(x => x.bookingDate);
                if (faresfrom != "00")
                {
                    flightbookingList = flightbookingList.Where(x => x.type == faresfrom).ToList();
                }
                if (StartDate != null && StartDate.ToString() != "" && FinishDate != null && FinishDate.ToString() != "")
                {
                    flightbookingList = flightbookingList.Where(x => x.bookingDate >= StartDate && x.bookingDate <= FinishDate).ToList();
                }
                if (Paybefore_StartDate != null && Paybefore_StartDate.ToString() != "" && Paybefore_FinishDate != null && Paybefore_FinishDate.ToString() != "")
                {
                    flightbookingList = flightbookingList.Where(x => x.TKTL >= Paybefore_StartDate && x.TKTL <= Paybefore_FinishDate).ToList();
                }
                if (StatusBooking != "00")
                {
                    flightbookingList = flightbookingList.Where(x => x.StatusBooking == Convert.ToInt32(StatusBooking)).ToList();
                }

                if (PaymentMethod != "00")
                {
                    flightbookingList = flightbookingList.Where(x => x.PaymentMethod == Convert.ToInt32(PaymentMethod)).ToList();
                }

                if (Platform != "00")
                {
                    flightbookingList = flightbookingList.Where(x => x.Platform == Platform).ToList();
                }

                //flightbookingList.OrderByDescending(x => x.TKTL);
                foreach (var flight in flightbookingList)
                {
                    AirFare airfare = new AirFare();
                    airfare.bookingOID = flight.BookingOID.ToString();
                    airfare.noOfAdults = flight.NoOfAdults ?? 0;
                    airfare.noOfChildren = flight.NoOfChildren ?? 0;
                    airfare.noOfInfants = flight.NoOfInfants ?? 0;
                    airfare.svc_class = flight.Svc_class;
                    airfare.grandTotal = flight.GrandTotal ?? 0;
                    airfare.isPassportRequired = flight.IsPassportRequired ?? false;
                    airfare.PNR = flight.PNR;
                    airfare.Platform = flight.Platform;
                    airfare.medId = flight.MedId ?? 0;
                    airfare.priceChange = flight.PriceChange ?? false;
                    airfare.oldPrice = flight.OldPrice ?? 0;
                    airfare.TKTL = flight.TKTL ?? DateTime.MinValue;
                    airfare.bookingDate = flight.bookingDate ?? DateTime.MinValue;
                    //airfare.remarks = flightBooking.Remarks;
                    airfare.isError = flight.IsError ?? false;
                    airfare.errorMessage = flight.ErrorMessage;
                    airfare.statusPayment = flight.StatusPayment ?? 0;
                    airfare.RobinhoodID = flight.RobinhoodID;
                    airfare.statusBooking = flight.StatusBooking ?? 0;
                    airfare.paymentMethod = flight.PaymentMethod ?? 0;
                    airfare.sourceBy = flight.sourceBy ?? 0;
                    airfare.isBundle = flight.IsBundle ?? false;

                    ContactInfo contactInfo = new ContactInfo();
                    var contactList = _unitOfWork.FlightBookingRepository.GetMany(x => x.BookingOID == flight.BookingOID).ToList();
                    foreach (var contact in contactList)
                    {
                        contactInfo = new ContactInfo();
                        contactInfo.title = contact.Title;
                        contactInfo.firstname = contact.Firstname;
                        contactInfo.middlename = contact.Middlename;
                        contactInfo.lastname = contact.Lastname;
                        contactInfo.email = contact.Email;
                        contactInfo.telNo = contact.TelNo;
                        contactInfo.countryCode = contact.CountryOfResidence;
                        airfare.contactInfo = contactInfo;
                    }

                    airfare.installmentMonthlyPaid = flight.InstallmentMonthly.GetValueOrDefault();
                    airfare.installmentPlan = flight.InstallmentPlan.GetValueOrDefault();
                    airfare.finalPrice = flight.FinalPrice.GetValueOrDefault();

                    airfare.promotionCode = flight.PromoCode;
                    airfare.promotionDiscount = flight.DiscountAmount.GetValueOrDefault();
                    airfare.promotionName = "";

                    FlightDetail detail = new FlightDetail();
                    NamingServices _namingServices = null;
                    Airport airportdepart = new Airport(_namingServices, true, "en");
                    Airline airlinedepart = new Airline(_namingServices, "en");
                    Airport airportreturn = new Airport(_namingServices, true, "en");
                    Airline airlinereturn = new Airline(_namingServices, "en");
                    airfare.depFlight = new List<FlightDetail>();
                    airfare.retFlight = new List<FlightDetail>();
                    var flightbookingflightdetaildepartList = _unitOfWork.FlightBookingFlightDetailRepository.GetMany(x => x.BookingOID == flight.BookingOID && x.TripType == "depart").OrderBy(x => x.Seq).ToList();
                    foreach (var flightdetail in flightbookingflightdetaildepartList)
                    {
                        detail = new FlightDetail();
                        airportdepart = new Airport(_namingServices, true, "en");
                        airportdepart.code = flightdetail.DepCity;
                        detail.depCity = airportdepart;
                        airportreturn = new Airport(_namingServices, true, "en");
                        airportreturn.code = flightdetail.ArrCity;
                        detail.arrCity = airportreturn;
                        airlinedepart = new Airline(_namingServices, "en");
                        airlinedepart.code = flightdetail.Airline;
                        detail.airline = airlinedepart;
                        airlinereturn = new Airline(_namingServices, "en");
                        airlinereturn.code = flightdetail.OperatedAirline;
                        detail.operatedAirline = airlinereturn;

                        //airportdepart.code = flightdetail.DepCity;
                        //detail.depCity = airportdepart;
                        detail.departureDateTime = flightdetail.DepartureDateTime ?? DateTime.MinValue;
                        //airportreturn.code = flightdetail.ArrCity;
                        //detail.arrCity = airportreturn;
                        detail.arrivalDateTime = flightdetail.ArrivalDateTime ?? DateTime.MinValue;
                        detail.Seq = flightdetail.Seq ?? 0;
                        airfare.depFlight.Add(detail);
                    }

                    Airport airportdepart1 = new Airport(_namingServices, true, "en");
                    Airline airlinedepart1 = new Airline(_namingServices, "en");
                    Airport airportreturn1 = new Airport(_namingServices, true, "en");
                    Airline airlinereturn1 = new Airline(_namingServices, "en");
                    var flightbookingflightdetailreturnList = _unitOfWork.FlightBookingFlightDetailRepository.GetMany(x => x.BookingOID == flight.BookingOID && x.TripType == "return").OrderBy(x => x.Seq).ToList();
                    foreach (var flightdetail in flightbookingflightdetailreturnList)
                    {
                        detail = new FlightDetail();
                        airportdepart1 = new Airport(_namingServices, true, "en");
                        airportdepart1.code = flightdetail.DepCity;
                        detail.depCity = airportdepart1;
                        airportreturn1 = new Airport(_namingServices, true, "en");
                        airportreturn1.code = flightdetail.ArrCity;
                        detail.arrCity = airportreturn1;
                        airlinedepart1 = new Airline(_namingServices, "en");
                        airlinedepart.code = flightdetail.Airline;
                        detail.airline = airlinedepart;
                        airlinereturn1 = new Airline(_namingServices, "en");
                        airlinereturn.code = flightdetail.OperatedAirline;
                        detail.operatedAirline = airlinereturn;

                        //airportdepart1.code = flightdetail.DepCity;
                        //detail.depCity = airportdepart1;
                        detail.departureDateTime = flightdetail.DepartureDateTime ?? DateTime.MinValue;
                        //airportreturn1.code = flightdetail.ArrCity;
                        //detail.arrCity = airportreturn1;
                        detail.arrivalDateTime = flightdetail.ArrivalDateTime ?? DateTime.MinValue;
                        detail.Seq = flightdetail.Seq ?? 0;
                        airfare.retFlight.Add(detail);
                    }

                    airfare.adtPaxs = new List<PaxInfo>();
                    PaxInfo paxinfodetail = new PaxInfo();
                    var flightbookinfpaxifo = _unitOfWork.FlightBookingPaxInfoRepository.GetMany(x => x.BookingOID == flight.BookingOID && x.PaxType == "ADT").ToList();
                    foreach (var paxinfo in flightbookinfpaxifo)
                    {
                        paxinfodetail = new PaxInfo();
                        paxinfodetail.firstname = paxinfo.Firstname;
                        paxinfodetail.middlename = paxinfo.Middlename;
                        paxinfodetail.lastname = paxinfo.Lastname;
                        paxinfodetail.email = paxinfo.Email;
                        paxinfodetail.telNo = paxinfo.TelNo;
                        airfare.adtPaxs.Add(paxinfodetail);
                    }

                    if (airfare.refund != null)
                    {
                        airfare.refund = new BL.Entities.RobinhoodFare.AirFare.Refund();
                        BL.Entities.RobinhoodFare.AirFare.Refund refunddetail = new BL.Entities.RobinhoodFare.AirFare.Refund();
                        var flightbookrefund = _unitOfWork.FlightBookingRefundRepository.GetMany(x => x.BookingOID == flight.BookingOID).ToList();
                        foreach (var refund in flightbookrefund)
                        {
                            refunddetail = new BL.Entities.RobinhoodFare.AirFare.Refund();
                            refunddetail.status = refund.Status ?? 0;
                            refunddetail.refundNo = refund.RefundNo;
                            airfare.refund = refunddetail;
                        }
                    }


                    airfareEntity.Add(airfare);
                }
            }

            return airfareEntity;
            //return _unitOfWork.FlightBookingRepository.GetAll().OrderBy(x => x.TKTL).ToList();
        }


        public List<AirFare> GetByPayBefore(DateTime StartPayBefore, DateTime FinishPayBefore)
        {
            List<AirFare> airfareEntity = new List<AirFare>();
            var flightbookingList = _unitOfWork.FlightBookingRepository.GetMany(x => x.TKTL >= StartPayBefore && x.TKTL <= FinishPayBefore).OrderByDescending(x => x.bookingDate).ToList();
            foreach (var flight in flightbookingList)
            {
                AirFare airfare = new AirFare();
                airfare.bookingOID = flight.BookingOID.ToString();
                airfare.noOfAdults = flight.NoOfAdults ?? 0;
                airfare.noOfChildren = flight.NoOfChildren ?? 0;
                airfare.noOfInfants = flight.NoOfInfants ?? 0;
                airfare.svc_class = flight.Svc_class;
                airfare.grandTotal = flight.GrandTotal ?? 0;
                airfare.isPassportRequired = flight.IsPassportRequired ?? false;
                airfare.PNR = flight.PNR;
                airfare.Platform = flight.Platform;
                airfare.medId = flight.MedId ?? 0;
                airfare.priceChange = flight.PriceChange ?? false;
                airfare.oldPrice = flight.OldPrice ?? 0;
                airfare.TKTL = flight.TKTL ?? DateTime.MinValue;
                airfare.bookingDate = flight.bookingDate ?? DateTime.MinValue;
                //airfare.remarks = flightBooking.Remarks;
                airfare.isError = flight.IsError ?? false;
                airfare.errorMessage = flight.ErrorMessage;


                FlightDetail detail = new FlightDetail();
                NamingServices _namingServices = null;
                Airport airportdepart = new Airport(_namingServices, true, "en");
                Airline airlinedepart = new Airline(_namingServices, "en");
                Airport airportreturn = new Airport(_namingServices, true, "en");
                Airline airlinereturn = new Airline(_namingServices, "en");
                airfare.depFlight = new List<FlightDetail>();
                airfare.retFlight = new List<FlightDetail>();
                var flightbookingflightdetaildepartList = _unitOfWork.FlightBookingFlightDetailRepository.GetMany(x => x.BookingOID == flight.BookingOID && x.TripType == "depart").OrderBy(x => x.Seq).ToList();
                foreach (var flightdetail in flightbookingflightdetaildepartList)
                {
                    detail = new FlightDetail();
                    airportdepart = new Airport(_namingServices, true, "en");
                    airportdepart.code = flightdetail.DepCity;
                    detail.depCity = airportdepart;
                    airportreturn = new Airport(_namingServices, true, "en");
                    airportreturn.code = flightdetail.ArrCity;
                    detail.arrCity = airportreturn;
                    airlinedepart = new Airline(_namingServices, "en");
                    airlinedepart.code = flightdetail.Airline;
                    detail.airline = airlinedepart;
                    airlinereturn = new Airline(_namingServices, "en");
                    airlinereturn.code = flightdetail.OperatedAirline;
                    detail.operatedAirline = airlinereturn;

                    //airportdepart.code = flightdetail.DepCity;
                    //detail.depCity = airportdepart;
                    detail.departureDateTime = flightdetail.DepartureDateTime ?? DateTime.MinValue;
                    //airportreturn.code = flightdetail.ArrCity;
                    //detail.arrCity = airportreturn;
                    detail.arrivalDateTime = flightdetail.ArrivalDateTime ?? DateTime.MinValue;
                    detail.Seq = flightdetail.Seq ?? 0;
                    airfare.depFlight.Add(detail);
                }

                Airport airportdepart1 = new Airport(_namingServices, true, "en");
                Airline airlinedepart1 = new Airline(_namingServices, "en");
                Airport airportreturn1 = new Airport(_namingServices, true, "en");
                Airline airlinereturn1 = new Airline(_namingServices, "en");
                var flightbookingflightdetailreturnList = _unitOfWork.FlightBookingFlightDetailRepository.GetMany(x => x.BookingOID == flight.BookingOID && x.TripType == "return").OrderBy(x => x.Seq).ToList();
                foreach (var flightdetail in flightbookingflightdetailreturnList)
                {
                    detail = new FlightDetail();
                    airportdepart1 = new Airport(_namingServices, true, "en");
                    airportdepart1.code = flightdetail.DepCity;
                    detail.depCity = airportdepart1;
                    airportreturn1 = new Airport(_namingServices, true, "en");
                    airportreturn1.code = flightdetail.ArrCity;
                    detail.arrCity = airportreturn1;
                    airlinedepart1 = new Airline(_namingServices, "en");
                    airlinedepart.code = flightdetail.Airline;
                    detail.airline = airlinedepart;
                    airlinereturn1 = new Airline(_namingServices, "en");
                    airlinereturn.code = flightdetail.OperatedAirline;
                    detail.operatedAirline = airlinereturn;

                    //airportdepart1.code = flightdetail.DepCity;
                    //detail.depCity = airportdepart1;
                    detail.departureDateTime = flightdetail.DepartureDateTime ?? DateTime.MinValue;
                    //airportreturn1.code = flightdetail.ArrCity;
                    //detail.arrCity = airportreturn1;
                    detail.arrivalDateTime = flightdetail.ArrivalDateTime ?? DateTime.MinValue;
                    detail.Seq = flightdetail.Seq ?? 0;
                    airfare.retFlight.Add(detail);
                }

                airfare.adtPaxs = new List<PaxInfo>();
                PaxInfo paxinfodetail = new PaxInfo();
                var flightbookinfpaxifo = _unitOfWork.FlightBookingPaxInfoRepository.GetMany(x => x.BookingOID == flight.BookingOID && x.PaxType == "ADT").ToList();
                foreach (var paxinfo in flightbookinfpaxifo)
                {
                    paxinfodetail = new PaxInfo();
                    paxinfodetail.title = paxinfo.Title;
                    paxinfodetail.firstname = paxinfo.Firstname;
                    paxinfodetail.middlename = paxinfo.Middlename;
                    paxinfodetail.lastname = paxinfo.Lastname;
                    paxinfodetail.email = paxinfo.Email;
                    paxinfodetail.telNo = paxinfo.TelNo;
                    paxinfodetail.paxType = paxinfo.PaxType;
                    airfare.adtPaxs.Add(paxinfodetail);
                }


                airfareEntity.Add(airfare);
            }


            return airfareEntity;
            //return _unitOfWork.FlightBookingRepository.GetAll().OrderBy(x => x.TKTL).ToList();
        }

        public AirFare GetByID(Guid id)
        {

            AirFare airfareEntity = new AirFare();
            FlightBooking flightBooking = _unitOfWork.FlightBookingRepository.GetFirstOrDefault(x => x.BookingOID == id);
            if (flightBooking == null)
            {
                return null;
            }
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
            airfareEntity.RobinhoodID = flightBooking.RobinhoodID;
            airfareEntity.bookingURN = flightBooking.BookingURN;
            if(flightBooking.PromotionGroupCode!=null && flightBooking.PromotionGroupCode.Length>0)
            {
                airfareEntity.promotionGroupCode = new List<string>();
                if (flightBooking.PromotionGroupCode.IndexOf("|") != -1)
                {
                    string [] arr = flightBooking.PromotionGroupCode.Split('|');
                    foreach (var _arr in arr) {
                        airfareEntity.promotionGroupCode.Add(_arr);
                    }
                }
                else
                {
                    airfareEntity.promotionGroupCode.Add(flightBooking.PromotionGroupCode);
                }
            }            
            airfareEntity.statusPayment = flightBooking.StatusPayment.GetValueOrDefault();
            airfareEntity.statusBooking = flightBooking.StatusBooking.GetValueOrDefault();
            airfareEntity.paymentMethod = flightBooking.PaymentMethod.GetValueOrDefault();

            airfareEntity.paymentDate= flightBooking.paymentDate ?? DateTime.MinValue;

            PaymentLog paymentLog = _unitOfWork.PaymentLogRepository.GetFirstOrDefault(x => x.RobinhoodID == airfareEntity.RobinhoodID);
            if (paymentLog != null)
            {
                BL.Entities.UpdatePayment.Request updatePaymentRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<BL.Entities.UpdatePayment.Request>(paymentLog.LogRequest);
                foreach (var _remark in updatePaymentRequest.remark)
                {
                    if (_remark.ToLower().IndexOf("paymenttype") != -1)
                    {
                        airfareEntity.paymentType = _remark.Split(':')[1];
                    }
                    if (_remark.ToLower().IndexOf("paymentvalue") != -1 || _remark.ToLower().IndexOf("cardmaskinfo") != -1)
                    {
                        airfareEntity.paymentValue = _remark.Split(':')[1];
                    }
                }
            }

            airfareEntity.contactInfo = new ContactInfo();
            airfareEntity.contactInfo.title = flightBooking.Title;
            airfareEntity.contactInfo.firstname = flightBooking.Firstname;
            airfareEntity.contactInfo.middlename = flightBooking.Middlename;
            airfareEntity.contactInfo.lastname = flightBooking.Lastname;
            airfareEntity.contactInfo.telNo = flightBooking.TelNo;
            airfareEntity.contactInfo.email = flightBooking.Email;
            airfareEntity.contactInfo.countryCode = flightBooking.CountryOfResidence;

            airfareEntity.uuid = flightBooking.UUID;
            airfareEntity.userID = flightBooking.UserID;
            airfareEntity.installmentMonthlyPaid = flightBooking.InstallmentMonthly.GetValueOrDefault();
            airfareEntity.installmentPlan = flightBooking.InstallmentPlan.GetValueOrDefault();
            airfareEntity.finalPrice = flightBooking.FinalPrice.GetValueOrDefault();

            airfareEntity.promotionCode = flightBooking.PromoCode;
            airfareEntity.promotionDiscount = flightBooking.DiscountAmount.GetValueOrDefault();
            airfareEntity.promotionName = "";
            airfareEntity.sourceBy = flightBooking.sourceBy ?? 0;
            airfareEntity.NFTFee = flightBooking.NFTFee.GetValueOrDefault();
            airfareEntity.Wallet_Address = flightBooking.Wallet_Address;

            NamingServices _namingServices = new NamingServices(_unitOfWork);
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
            if (bookingfare != null)
            {
                fare = new Fare();
                fare.fareOID = bookingfare.FlightBookingFareOID;
                fare.baseFare = bookingfare.BaseFare ?? 0;
                fare.sellingBaseFare = bookingfare.SellingBaseFare ?? 0;
                fare.lessFare = bookingfare.LessFare ?? 0;
                fare.qtax = bookingfare.Qtax ?? 0;
                fare.tax = bookingfare.Tax ?? 0;
                airfareEntity.adtFare = fare;

                airfareEntity.adtFare.baggages = new List<Baggage>();
                Baggage baggage = new Baggage();
                List<FlightBookingBaggage> baggageList = _unitOfWork.FlightBookingBaggageRepository.GetMany(x => x.FlightBookingFareOID == bookingfare.FlightBookingFareOID).ToList();
                FlightBookingBaggage bookingbaggage = new FlightBookingBaggage();
                foreach (var baggagedetail in baggageList)
                {
                    baggage.baggageNo = baggagedetail.BaggageNo;
                    baggage.baggageUnit = baggagedetail.BaggageUnit;
                    airfareEntity.adtFare.baggages.Add(baggage);
                }
            }

            fare = new Fare();
            bookingfare = bookingfareList.Find(x => x.PessengerType == "chdFare");
            if (bookingfare != null)
            {
                fare = new Fare();
                fare.fareOID = bookingfare.FlightBookingFareOID;
                fare.baseFare = bookingfare.BaseFare ?? 0;
                fare.sellingBaseFare = bookingfare.SellingBaseFare ?? 0;
                fare.lessFare = bookingfare.LessFare ?? 0;
                fare.qtax = bookingfare.Qtax ?? 0;
                fare.tax = bookingfare.Tax ?? 0;
                airfareEntity.chdFare = fare;

                airfareEntity.chdFare.baggages = new List<Baggage>();
                Baggage baggage = new Baggage();
                List<FlightBookingBaggage> baggageList = _unitOfWork.FlightBookingBaggageRepository.GetMany(x => x.FlightBookingFareOID == bookingfare.FlightBookingFareOID).ToList();
                FlightBookingBaggage bookingbaggage = new FlightBookingBaggage();
                foreach (var baggagedetail in baggageList)
                {
                    baggage.baggageNo = baggagedetail.BaggageNo;
                    baggage.baggageUnit = baggagedetail.BaggageUnit;
                    airfareEntity.chdFare.baggages.Add(baggage);
                }
            }

            fare = new Fare();
            bookingfare = bookingfareList.Find(x => x.PessengerType == "intFare");
            if (bookingfare != null)
            {
                fare = new Fare();
                fare.fareOID = bookingfare.FlightBookingFareOID;
                fare.baseFare = bookingfare.BaseFare ?? 0;
                fare.sellingBaseFare = bookingfare.SellingBaseFare ?? 0;
                fare.lessFare = bookingfare.LessFare ?? 0;
                fare.qtax = bookingfare.Qtax ?? 0;
                fare.tax = bookingfare.Tax ?? 0;
                airfareEntity.infFare = fare;

                airfareEntity.infFare.baggages = new List<Baggage>();
                Baggage baggage = new Baggage();
                List<FlightBookingBaggage> baggageList = _unitOfWork.FlightBookingBaggageRepository.GetMany(x => x.FlightBookingFareOID == bookingfare.FlightBookingFareOID).ToList();
                FlightBookingBaggage bookingbaggage = new FlightBookingBaggage();
                foreach (var baggagedetail in baggageList)
                {
                    baggage.baggageNo = baggagedetail.BaggageNo;
                    baggage.baggageUnit = baggagedetail.BaggageUnit;
                    airfareEntity.infFare.baggages.Add(baggage);
                }
            }



            airfareEntity.multiFlight = new List<List<FlightDetail>>();
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

                flightdepart.flightNumber = flightdetail.FlightNumber;
                flightdepart.equipmentType = flightdetail.EquipmentType;
                flightdepart.flightTime = flightdetail.FlightTime;
                flightdepart.fareBasis = flightdetail.FareBasis;
                flightdepart.fareType = flightdetail.FareType;
                flightdepart.cabin = flightdetail.Cabin;
                flightdepart.rbd = flightdetail.Rbd;
                flightdepart.departureDateTime = flightdetail.DepartureDateTime ?? DateTime.MinValue;
                flightdepart.arrivalDateTime = flightdetail.ArrivalDateTime ?? DateTime.MinValue;
                flightdepart.Seq = flightdetail.Seq ?? 0;
                flightdepart.controlNumber = flightdetail.ControlNumber;
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
                    airlinedepart1.code = flightdetail.Airline;
                    flightreturn.airline = airlinedepart1;
                    airlinereturn1 = new Airline(_namingServices, "en");
                    airlinereturn1.code = flightdetail.OperatedAirline;
                    flightreturn.operatedAirline = airlinereturn1;
                    flightreturn.flightNumber = flightdetail.FlightNumber;
                    flightreturn.flightTime = flightdetail.FlightTime;
                    flightreturn.fareBasis = flightdetail.FareBasis;
                    flightreturn.fareType = flightdetail.FareType;
                    flightreturn.cabin = flightdetail.Cabin;
                    flightreturn.rbd = flightdetail.Rbd;
                    flightreturn.departureDateTime = flightdetail.DepartureDateTime ?? DateTime.MinValue;
                    flightreturn.arrivalDateTime = flightdetail.ArrivalDateTime ?? DateTime.MinValue;
                    flightreturn.Seq = flightdetail.Seq ?? 0;
                    flightreturn.equipmentType = flightdetail.EquipmentType;
                    flightreturn.controlNumber = flightdetail.ControlNumber;
                    airfareEntity.retFlight.Add(flightreturn);
                }
            }

            List<FlightBookingFlightDetail> flightdetailListmulti = _unitOfWork.FlightBookingFlightDetailRepository.GetMany(x => x.BookingOID == id && x.TripType.IndexOf("multi") != -1).OrderBy(x => x.TripType).ToList();
            List<FlightBookingFlightDetail> flightdetailmulti = new List<FlightBookingFlightDetail>();
            FlightDetail flightmulti = new FlightDetail();
            List<FlightDetail> multiList = new List<FlightDetail>();
            for (int i = 0; i < 6; i++)
            {
                flightdetailmulti = flightdetailListmulti.FindAll(x => x.TripType == ("multi" + (i + 1)));
                if (flightdetailmulti != null && flightdetailmulti.Count > 0)
                {
                    multiList = new List<FlightDetail>();
                    flightdetailmulti = flightdetailmulti.OrderBy(x => x.Seq).ToList();
                    foreach (var flightdetail in flightdetailmulti)
                    {
                        flightmulti = new FlightDetail();
                        airportdepart1 = new Airport(_namingServices, true, "en");
                        airportdepart1.code = flightdetail.DepCity;
                        flightmulti.depCity = airportdepart1;
                        airportreturn1 = new Airport(_namingServices, true, "en");
                        airportreturn1.code = flightdetail.ArrCity;
                        flightmulti.arrCity = airportreturn1;
                        airlinedepart1 = new Airline(_namingServices, "en");
                        airlinedepart1.code = flightdetail.Airline;
                        flightmulti.airline = airlinedepart1;
                        airlinereturn1 = new Airline(_namingServices, "en");
                        airlinereturn1.code = flightdetail.OperatedAirline;
                        flightmulti.operatedAirline = airlinereturn1;
                        flightmulti.flightNumber = flightdetail.FlightNumber;
                        flightmulti.flightTime = flightdetail.FlightTime;
                        flightmulti.fareBasis = flightdetail.FareBasis;
                        flightmulti.fareType = flightdetail.FareType;
                        flightmulti.cabin = flightdetail.Cabin;
                        flightmulti.rbd = flightdetail.Rbd;
                        airportdepart1.code = flightdetail.DepCity;
                        flightmulti.depCity = airportdepart1;
                        flightmulti.departureDateTime = flightdetail.DepartureDateTime ?? DateTime.MinValue;
                        airportreturn1.code = flightdetail.ArrCity;
                        flightmulti.arrCity = airportreturn1;
                        flightmulti.arrivalDateTime = flightdetail.ArrivalDateTime ?? DateTime.MinValue;
                        flightmulti.Seq = flightdetail.Seq ?? 0;
                        flightmulti.equipmentType = flightdetail.EquipmentType;
                        flightmulti.controlNumber = flightdetail.ControlNumber;
                        multiList.Add(flightmulti);
                    }
                    airfareEntity.multiFlight.Add(multiList);
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
                paxInfoadt.kiwiBag = adt.KiwiBag ?? 0;
                paxInfoadt.kiwiBagPrice = adt.KiwiBagPrice ?? 0;
                paxInfoadt.kiwiBagWeight = adt.KiwiBagWeight ?? 0;

                if (adt.FrequencyFlyerAirline!=null && adt.FrequencyFlyerAirline.Length>0 && adt.FrequencyFlyerAirline.IndexOf("|")!=-1)
                {
                    paxInfoadt.frequentFlyList = new List<FrequentFlyList>();
                    string[] arrAirline = adt.FrequencyFlyerAirline.Split('|');
                    string[] arrNumber = adt.FrequencyFlyerNumber.Split('|');
                    FrequentFlyList frequentFly = new FrequentFlyList();
                    for(int i=0;i< arrAirline.Length;i++)
                    {
                        frequentFly = new FrequentFlyList();
                        frequentFly.Airline = arrAirline[i];
                        frequentFly.Number = arrNumber[i];
                        paxInfoadt.frequentFlyList.Add(frequentFly);
                    }
                }
                else
                {
                    paxInfoadt.frequencyFlyerAirline = adt.FrequencyFlyerAirline;
                    paxInfoadt.frequencyFlyerNumber = adt.FrequencyFlyerNumber;
                }

                paxInfoadt.mealRequest = adt.MealRequest;
                if (adt.SeatRequest != null && adt.SeatRequest.Length > 0 && adt.SeatRequest.IndexOf("|") != -1)
                {
                    paxInfoadt.seatsRequest = new List<SeatsRequest>();
                    string[] arr = adt.SeatRequest.Split('|');
                    SeatsRequest seat = new SeatsRequest();
                    for (int i = 0; i < arr.Length; i++)
                    {
                        seat = new SeatsRequest();
                        seat.seatType = "NSST";
                        seat.seatRefNo = arr[i];
                        paxInfoadt.seatsRequest.Add(seat);
                    }
                }
                else
                {
                    paxInfoadt.seatRequest = adt.SeatRequest;
                }

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
                    paxInfochd.kiwiBag = chd.KiwiBag ?? 0;
                    paxInfochd.kiwiBagPrice = chd.KiwiBagPrice ?? 0;
                    paxInfochd.kiwiBagWeight = chd.KiwiBagWeight ?? 0;

                    if (chd.FrequencyFlyerAirline != null && chd.FrequencyFlyerAirline.Length > 0 && chd.FrequencyFlyerAirline.IndexOf("|") != -1)
                    {
                        paxInfochd.frequentFlyList = new List<FrequentFlyList>();
                        string[] arrAirline = chd.FrequencyFlyerAirline.Split('|');
                        string[] arrNumber = chd.FrequencyFlyerNumber.Split('|');
                        FrequentFlyList frequentFly = new FrequentFlyList();
                        for (int i = 0; i < arrAirline.Length; i++)
                        {
                            frequentFly = new FrequentFlyList();
                            frequentFly.Airline = arrAirline[i];
                            frequentFly.Number = arrNumber[i];
                            paxInfochd.frequentFlyList.Add(frequentFly);
                        }
                    }
                    else
                    {
                        paxInfochd.frequencyFlyerAirline = chd.FrequencyFlyerAirline;
                        paxInfochd.frequencyFlyerNumber = chd.FrequencyFlyerNumber;
                    }

                    paxInfochd.mealRequest = chd.MealRequest;
                    if (chd.SeatRequest != null && chd.SeatRequest.Length > 0 && chd.SeatRequest.IndexOf("|") != -1)
                    {
                        paxInfochd.seatsRequest = new List<SeatsRequest>();
                        string[] arr = chd.SeatRequest.Split('|');
                        SeatsRequest seat = new SeatsRequest();
                        for (int i = 0; i < arr.Length; i++)
                        {
                            seat = new SeatsRequest();
                            seat.seatType = "NSST";
                            seat.seatRefNo = arr[i];
                            paxInfochd.seatsRequest.Add(seat);
                        }
                    }
                    else
                    {
                        paxInfochd.seatRequest = chd.SeatRequest;
                    }

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


                    if (inf.FrequencyFlyerAirline != null && inf.FrequencyFlyerAirline.Length > 0 && inf.FrequencyFlyerAirline.IndexOf("|") != -1)
                    {
                        paxInfoinf.frequentFlyList = new List<FrequentFlyList>();
                        string[] arrAirline = inf.FrequencyFlyerAirline.Split('|');
                        string[] arrNumber = inf.FrequencyFlyerNumber.Split('|');
                        FrequentFlyList frequentFly = new FrequentFlyList();
                        for (int i = 0; i < arrAirline.Length; i++)
                        {
                            frequentFly = new FrequentFlyList();
                            frequentFly.Airline = arrAirline[i];
                            frequentFly.Number = arrNumber[i];
                            paxInfoinf.frequentFlyList.Add(frequentFly);
                        }
                    }
                    else
                    {
                        paxInfoinf.frequencyFlyerAirline = inf.FrequencyFlyerAirline;
                        paxInfoinf.frequencyFlyerNumber = inf.FrequencyFlyerNumber;
                    }

                    paxInfoinf.mealRequest = inf.MealRequest;
                    if (inf.SeatRequest != null && inf.SeatRequest.Length > 0 && inf.SeatRequest.IndexOf("|") != -1)
                    {
                        paxInfoinf.seatsRequest = new List<SeatsRequest>();
                        string[] arr = inf.SeatRequest.Split('|');
                        SeatsRequest seat = new SeatsRequest();
                        for (int i = 0; i < arr.Length; i++)
                        {
                            seat = new SeatsRequest();
                            seat.seatType = "NSST";
                            seat.seatRefNo = arr[i];
                            paxInfoinf.seatsRequest.Add(seat);
                        }
                    }
                    else
                    {
                        paxInfoinf.seatRequest = inf.SeatRequest;
                    }

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
            List<FlightBookingFareRule> fareruleList = _unitOfWork.FlightBookingFareRuleRepository.GetMany(x => x.BookingOID == id).ToList();

            foreach (var fList in fareruleList)
            {
                FareRule farerule = new FareRule();
                farerule.fareBasis = fList.FareBasis;
                City city1 = new City(_namingServices, "en");
                city1.code = fList.Origin;
                farerule.origin = city1;
                city1 = new City(_namingServices, "en");
                city1.code = fList.Destination;
                farerule.destination = city1;

                farerule.rules = new List<FareRuleDatail>();
                List<FlightBookingFareRuleDatail> detailList = _unitOfWork.FlightBookingFareRuleDatailRepository.GetMany(x => x.FlightBookingFareRuleOID == fList.FlightBookingFareRuleOID).ToList();
                foreach (var ruleDetail in detailList)
                {
                    FareRuleDatail r = new FareRuleDatail();
                    r.fareRuleText = ruleDetail.FareRuleText.Split('|').ToList();
                    farerule.rules.Add(r);
                }
                airfareEntity.fareRules.Add(farerule);
            }

            foreach (var flight in airfareEntity.depFlight)
            {
                flight.setDisplayDateTime("en", airfareEntity.depFlight[0].departureDateTime);
            }
            if (airfareEntity.retFlight != null)
            {
                foreach (var flight in airfareEntity.retFlight)
                {
                    flight.setDisplayDateTime("en", airfareEntity.retFlight[0].departureDateTime);
                }
            }
            if (airfareEntity.multiFlight != null && airfareEntity.multiFlight.Count > 0)
            {
                foreach (var multi in airfareEntity.multiFlight)
                {
                    foreach (var flight in multi)
                    {
                        flight.setDisplayDateTime("en", multi[0].departureDateTime);
                    }
                }
            }

            return airfareEntity;
        }

        public void SaveOrUpdate(AirFare airfare)
        {
            using (var scope = new TransactionScope())
            {
                AirFare airfareEntity = new AirFare();
                FlightBooking flightBooking = _unitOfWork.FlightBookingRepository.GetFirstOrDefault(x => x.BookingOID.ToString() == airfare.bookingOID);
                if (flightBooking == null)
                {
                    _unitOfWork.FlightBookingRepository.Insert(flightBooking);
                }
                else
                {
                    //flightBooking.GrandTotal = airfare.grandTotal;
                    //flightBooking.TKTL = airfare.TKTL;
                    flightBooking.PaymentMethod = airfare.paymentMethod;
                    flightBooking.StatusPayment = airfare.statusPayment;
                    flightBooking.StatusBooking = airfare.statusBooking;
                    flightBooking.Note = airfare.note;
                    flightBooking.Title = airfare.contactInfo.title;
                    flightBooking.Firstname = airfare.contactInfo.firstname;
                    flightBooking.Middlename = airfare.contactInfo.middlename;
                    flightBooking.Lastname = airfare.contactInfo.lastname;
                    flightBooking.Email = airfare.contactInfo.email;
                    flightBooking.TelNo = airfare.contactInfo.telNo;
                    _unitOfWork.FlightBookingRepository.Update(flightBooking);
                }

                //Fare fare = new Fare();
                //List<FlightBookingFare> bookingfareList = _unitOfWork.FlightBookingFareRepository.GetMany(x => x.BookingOID.ToString() == airfare.bookingOID && x.PessengerType == "adtFare").ToList();
                //if (bookingfareList != null && bookingfareList.Count > 0)
                //{
                //    foreach(var bookingfare in bookingfareList) {
                //        bookingfare.SellingBaseFare = airfare.adtFare.sellingBaseFare;
                //    _unitOfWork.FlightBookingFareRepository.Update(bookingfare);
                //    }
                //}


                //List<FlightBookingPaxInfo> adtpaxList = _unitOfWork.FlightBookingPaxInfoRepository.GetMany(x => x.BookingOID.ToString() == airfare.bookingOID && x.PaxType == "ADT").ToList();
                //if (adtpaxList != null && adtpaxList.Count > 0)
                //{
                //    foreach (var adtPax in adtpaxList)
                //    {
                //        adtPax.Title = airfare.adtPaxs.Find(x => x.firstname == adtPax.Firstname && x.lastname == adtPax.Lastname).title;
                //        _unitOfWork.FlightBookingPaxInfoRepository.Update(adtPax);
                //    }
                //}

                //List<FlightBookingPaxInfo> chdpaxList = _unitOfWork.FlightBookingPaxInfoRepository.GetMany(x => x.BookingOID.ToString() == airfare.bookingOID && x.PaxType == "CHD").ToList();
                //if (chdpaxList != null && chdpaxList.Count > 0)
                //{
                //    foreach (var chdPax in chdpaxList)
                //    {
                //        chdPax.Title = airfare.chdPaxs.Find(x => x.firstname == chdPax.Firstname && x.lastname == chdPax.Lastname).title;
                //        _unitOfWork.FlightBookingPaxInfoRepository.Update(chdPax);
                //    }
                //}

                //List<FlightBookingPaxInfo> infpaxList = _unitOfWork.FlightBookingPaxInfoRepository.GetMany(x => x.BookingOID.ToString() == airfare.bookingOID && x.PaxType == "INF").ToList();
                //if (infpaxList != null && infpaxList.Count > 0)
                //{
                //    foreach (var infPax in infpaxList)
                //    {
                //        infPax.Title = airfare.infPaxs.Find(x => x.firstname == infPax.Firstname && x.lastname == infPax.Lastname).title;
                //        _unitOfWork.FlightBookingPaxInfoRepository.Update(infPax);
                //    }
                //}

                _unitOfWork.Save();

                scope.Complete();
            }
        }

        public void SaveOrUpdateRefund(AirFare airfare)
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

                //FlightBookingRefund refund1 = new FlightBookingRefund();
                FlightBookingRefund refund = _unitOfWork.FlightBookingRefundRepository.GetFirstOrDefault(x => x.BookingOID.ToString() == airfare.bookingOID);
                if (refund == null)
                {
                    FlightBookingRefund refund1 = new FlightBookingRefund();
                    Guid refundid = Guid.NewGuid();
                    refund1.FlightBookingRefundOID = refundid;
                    refund1.BookingOID = new Guid(airfare.bookingOID);
                    refund1.Status = 1;
                    refund1.NewPNR = airfare.refund.newPNR;
                    refund1.RefundNo = airfare.refund.refundNo;
                    refund1.RefundCreateDate = airfare.refund.refundCreateDate;
                    refund1.RefundGMDate = airfare.refund.refundGMDate;
                    refund1.DueDateOfRefund = airfare.refund.dueDateOfRefund;
                    refund1.Remark = airfare.refund.remark;
                    refund1.OldPNR = airfare.refund.oldPNR;
                    _unitOfWork.FlightBookingRefundRepository.Insert(refund1);
                    _unitOfWork.Save();
                }
                else
                {
                    refund.Status = airfare.refund.status;
                    refund.NewPNR = airfare.refund.newPNR;
                    refund.RefundNo = airfare.refund.refundNo;
                    refund.RefundCreateDate = airfare.refund.refundCreateDate;
                    refund.RefundGMDate = airfare.refund.refundGMDate;
                    refund.DueDateOfRefund = airfare.refund.dueDateOfRefund;
                    refund.Remark = airfare.refund.remark;
                    refund.OldPNR = airfare.refund.oldPNR;
                    _unitOfWork.FlightBookingRefundRepository.DetachAll();
                    _unitOfWork.FlightBookingRefundRepository.Update(refund);
                    _unitOfWork.Save();
                }

                List<FlightBookingPaxInfo> adtpaxList = _unitOfWork.FlightBookingPaxInfoRepository.GetMany(x => x.BookingOID.ToString() == airfare.bookingOID && x.PaxType == "ADT").ToList();
                if (adtpaxList != null && adtpaxList.Count > 0)
                {
                    foreach (var adtPax in adtpaxList)
                    {
                        adtPax.NetRefund = airfare.adtPaxs.Find(x => x.firstname == adtPax.Firstname && x.lastname == adtPax.Lastname).netRefund;
                        adtPax.AgentRefund = airfare.adtPaxs.Find(x => x.firstname == adtPax.Firstname && x.lastname == adtPax.Lastname).agentRefund;
                        adtPax.RefundAmount = airfare.adtPaxs.Find(x => x.firstname == adtPax.Firstname && x.lastname == adtPax.Lastname).refundAmount;
                        adtPax.TickNoRefund = airfare.adtPaxs.Find(x => x.firstname == adtPax.Firstname && x.lastname == adtPax.Lastname).tickNoRefund;
                        adtPax.RemarkRefund = airfare.adtPaxs.Find(x => x.firstname == adtPax.Firstname && x.lastname == adtPax.Lastname).remarkRefund;
                        adtPax.StatusRefund = airfare.adtPaxs.Find(x => x.firstname == adtPax.Firstname && x.lastname == adtPax.Lastname).StatusRefund;
                        _unitOfWork.FlightBookingPaxInfoRepository.Update(adtPax);
                        _unitOfWork.Save();
                    }
                }

                List<FlightBookingPaxInfo> chdpaxList = _unitOfWork.FlightBookingPaxInfoRepository.GetMany(x => x.BookingOID.ToString() == airfare.bookingOID && x.PaxType == "CHD").ToList();
                if (chdpaxList != null && chdpaxList.Count > 0)
                {
                    foreach (var chdPax in chdpaxList)
                    {
                        chdPax.NetRefund = airfare.chdPaxs.Find(x => x.firstname == chdPax.Firstname && x.lastname == chdPax.Lastname).netRefund;
                        chdPax.AgentRefund = airfare.chdPaxs.Find(x => x.firstname == chdPax.Firstname && x.lastname == chdPax.Lastname).agentRefund;
                        chdPax.RefundAmount = airfare.chdPaxs.Find(x => x.firstname == chdPax.Firstname && x.lastname == chdPax.Lastname).refundAmount;
                        chdPax.TickNoRefund = airfare.chdPaxs.Find(x => x.firstname == chdPax.Firstname && x.lastname == chdPax.Lastname).tickNoRefund;
                        chdPax.RemarkRefund = airfare.chdPaxs.Find(x => x.firstname == chdPax.Firstname && x.lastname == chdPax.Lastname).remarkRefund;
                        chdPax.StatusRefund = airfare.chdPaxs.Find(x => x.firstname == chdPax.Firstname && x.lastname == chdPax.Lastname).StatusRefund;
                        _unitOfWork.FlightBookingPaxInfoRepository.Update(chdPax);
                        _unitOfWork.Save();
                    }
                }

                List<FlightBookingPaxInfo> infpaxList = _unitOfWork.FlightBookingPaxInfoRepository.GetMany(x => x.BookingOID.ToString() == airfare.bookingOID && x.PaxType == "INF").ToList();
                if (infpaxList != null && infpaxList.Count > 0)
                {
                    foreach (var infPax in infpaxList)
                    {
                        infPax.NetRefund = airfare.infPaxs.Find(x => x.firstname == infPax.Firstname && x.lastname == infPax.Lastname).netRefund;
                        infPax.AgentRefund = airfare.infPaxs.Find(x => x.firstname == infPax.Firstname && x.lastname == infPax.Lastname).agentRefund;
                        infPax.RefundAmount = airfare.infPaxs.Find(x => x.firstname == infPax.Firstname && x.lastname == infPax.Lastname).refundAmount;
                        infPax.TickNoRefund = airfare.infPaxs.Find(x => x.firstname == infPax.Firstname && x.lastname == infPax.Lastname).tickNoRefund;
                        infPax.RemarkRefund = airfare.infPaxs.Find(x => x.firstname == infPax.Firstname && x.lastname == infPax.Lastname).remarkRefund;
                        infPax.StatusRefund = airfare.infPaxs.Find(x => x.firstname == infPax.Firstname && x.lastname == infPax.Lastname).StatusRefund;
                        _unitOfWork.FlightBookingPaxInfoRepository.Update(infPax);
                        _unitOfWork.Save();
                    }
                }



                //_unitOfWork.Save();

                scope.Complete();
            }
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

        public void SendBookingEmail(Guid id, string subject, string languageCode)
        {
            var result = this.GetByID(id);
            string url = ConfigurationManager.AppSettings["Booking.URL"] + "Flight/Email/" + id.ToString() + (languageCode != null && languageCode.Length > 0 ? ("?lang=" + languageCode) : "");
            string body = BL.Utilities.EmailUtilities.captureHtml(url);

            string bcc = ConfigurationManager.AppSettings["Mail.BCC"];
            BL.Utilities.EmailUtilities.sendMail(result.contactInfo.email, subject, body, "", bcc);
        }
        public void SendBookingEmail(Guid id, string subject, System.Net.Mail.Attachment attachFile, string languageCode)
        {
            var result = this.GetByID(id);
            string url = ConfigurationManager.AppSettings["Booking.URL"] + "Flight/Email/" + id.ToString() + (languageCode != null && languageCode.Length > 0 ? ("?lang=" + languageCode) : "");
            string body = BL.Utilities.EmailUtilities.captureHtml(url);

            string bcc = ConfigurationManager.AppSettings["Mail.BCC"];
            BL.Utilities.EmailUtilities.sendMail(result.contactInfo.email, subject, body, "", bcc, attachFile);
        }

        public bool ResendBookingEmail(Guid id, string subject, string Email)
        {
            var result = this.GetByID(id);
            string url = ConfigurationManager.AppSettings["Booking.URL"] + "Flight/Email/" + id.ToString();
            string body = BL.Utilities.EmailUtilities.captureHtml(url);

            string bcc = ConfigurationManager.AppSettings["Mail.BCC"];
            bool bSend = BL.Utilities.EmailUtilities.SendMailReturnStatus(Email, subject, body, "", bcc);
            return bSend;
        }
        public bool ResendBookingEmail(Guid id, string subject, string Email, System.Net.Mail.Attachment attachFile)
        {
            var result = this.GetByID(id);
            string url = ConfigurationManager.AppSettings["Booking.URL"] + "Flight/Email/" + id.ToString();
            string body = BL.Utilities.EmailUtilities.captureHtml(url);

            string bcc = ConfigurationManager.AppSettings["Mail.BCC"];
            bool bSend = BL.Utilities.EmailUtilities.SendMailReturnStatus(Email, subject, body, "", bcc, attachFile);
            return bSend;
        }

        public void SendBookingEmail(string robinhoodID, string subject, string languageCode)
        {
            List<FlightBooking> flightBookingList = _unitOfWork.FlightBookingRepository.GetMany(x => x.RobinhoodID == robinhoodID).ToList();
            if (flightBookingList != null && flightBookingList.Count > 0)
            {
                var result = this.GetByID(flightBookingList[0].BookingOID);
                string url = ConfigurationManager.AppSettings["Booking.URL"] + "Flight/Email/?robinhoodID=" + robinhoodID + (languageCode != null && languageCode.Length > 0 ? ("&lang=" + languageCode) : "");
                string body = BL.Utilities.EmailUtilities.captureHtml(url);

                string bcc = ConfigurationManager.AppSettings["Mail.BCC"];
                BL.Utilities.EmailUtilities.sendMail(result.contactInfo.email, subject, body, "", bcc);
            }
        }
        public bool ResendBookingEmail(string robinhoodID, string subject, string Email)
        {
            List<FlightBooking> flightBookingList = _unitOfWork.FlightBookingRepository.GetMany(x => x.RobinhoodID == robinhoodID).ToList();
            if (flightBookingList != null && flightBookingList.Count > 0)
            {
                var result = this.GetByID(flightBookingList[0].BookingOID);
                string url = ConfigurationManager.AppSettings["Booking.URL"] + "Flight/Email/?robinhoodID=" + robinhoodID;
                string body = BL.Utilities.EmailUtilities.captureHtml(url);

                string bcc = ConfigurationManager.AppSettings["Mail.BCC"];
                bool bSend = BL.Utilities.EmailUtilities.SendMailReturnStatus(Email, subject, body, "", bcc);
                return bSend;
            }
            else
            {
                return false;
            }
        }
    }
}
