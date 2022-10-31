using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using DataModel;
using DataModel.UnitOfWork;
using BL.Entities.RobinhoodFare;
using log4net;

namespace BL
{
    public class FlightBookingServices : IFlightBookingServices
    {
        private static readonly ILog Log =
          LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly UnitOfWork _unitOfWork;
        public FlightBookingServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void UpdateBookingStatus(Guid id, int bookingStatus)
        {
            FlightBooking flightBooking = _unitOfWork.FlightBookingRepository.GetFirstOrDefault(x => x.BookingOID == id);

            if (flightBooking != null)
            {
                flightBooking.StatusBooking = bookingStatus;

                _unitOfWork.FlightBookingRepository.DetachAll();
                _unitOfWork.FlightBookingRepository.Update(flightBooking);
                _unitOfWork.Save();
            }
        }
        public void UpdatePayment(Guid id, int paymentStatus, int paymentMethod, int installmentPlan, decimal installmentMonthly, DateTime paymentDate)
        {
            FlightBooking flightBooking = _unitOfWork.FlightBookingRepository.GetFirstOrDefault(x => x.BookingOID == id);

            if (flightBooking != null)
            {
                if (paymentMethod != 0)
                {
                    flightBooking.PaymentMethod = paymentMethod;
                }
                if (installmentPlan != 0)
                {
                    flightBooking.InstallmentMonthly = installmentMonthly;
                    flightBooking.InstallmentPlan = installmentPlan;
                }
                flightBooking.StatusPayment = paymentStatus;
                flightBooking.paymentDate = paymentDate;

                _unitOfWork.FlightBookingRepository.DetachAll();
                _unitOfWork.FlightBookingRepository.Update(flightBooking);
                _unitOfWork.Save();
            }
        }

        public void UpdateFinalPrice(Guid id, decimal finalPrice)
        {
            FlightBooking flightBooking = _unitOfWork.FlightBookingRepository.GetFirstOrDefault(x => x.BookingOID == id);

            if (flightBooking != null)
            {
                flightBooking.FinalPrice = finalPrice;
                _unitOfWork.FlightBookingRepository.DetachAll();
                _unitOfWork.FlightBookingRepository.Update(flightBooking);
                _unitOfWork.Save();
            }
        }
        public Guid SaveBooking(AirFare booking)
        {
            if (booking == null)
            {
                Log.Debug("airfare null");
            }
            else
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(booking);
                Log.Debug("json airfare");
                Log.Debug(json);
                if (booking.type != null)
                {
                    Log.Debug("booking.type="+ booking.type);
                }
            }

            Guid id = Guid.NewGuid();
            string robinhoodID = "";
           
            if (booking.RobinhoodID==null || (booking.RobinhoodID!=null && booking.RobinhoodID.Length==0))//(booking.isBundle == false)
            {
                Log.Debug("booking.RobinhoodID==null or Length==0");
                RunningNumberServices runningSvc = new RunningNumberServices(_unitOfWork);
                int rbhID = runningSvc.GetNumber("FBOOKING" + DateTime.Today.ToString("yyMMdd"));
                robinhoodID = "F" + DateTime.Today.ToString("yyMMdd") + rbhID.ToString().PadLeft(4, '0');
            }
            else
            {
                Log.Debug("booking.RobinhoodID=" + booking.RobinhoodID);
                robinhoodID = booking.RobinhoodID;
            }
            try
            {
                using (var scope = new TransactionScope())
                {
                    FlightBooking book = new FlightBooking();
                    book.BookingOID = id;
                    book.NoOfAdults = booking.noOfAdults;
                    book.NoOfChildren = booking.noOfChildren;
                    book.NoOfInfants = booking.noOfInfants;
                    book.Svc_class = booking.svc_class;
                    book.GrandTotal = booking.grandTotal;
                    book.IsPassportRequired = booking.isPassportRequired;
                    book.PNR = booking.PNR;
                    book.Platform = booking.Platform;
                    book.MedId = booking.medId;
                    book.PriceChange = booking.priceChange;
                    book.OldPrice = booking.oldPrice;
                    Log.Debug("booking.TKTL.Year=" + booking.TKTL.Year);
                    if (booking.TKTL.Year > 1900)
                    {
                        book.TKTL = booking.TKTL;
                    }
                    else
                    {
                        book.TKTL = DateTime.Now.AddHours(48);
                    }
                    book.bookingDate = DateTime.Now;
                    book.RobinhoodID = robinhoodID;
                    book.BookingURN = booking.bookingURN;
                    if(booking.promotionGroupCode!=null && booking.promotionGroupCode.Count > 0)
                    {
                        book.PromotionGroupCode = "";
                        for (int iP = 0; iP < booking.promotionGroupCode.Count; iP++)
                        {
                            if (book.PromotionGroupCode.Length > 0)
                            {
                                book.PromotionGroupCode += "|";
                            }

                            book.PromotionGroupCode += booking.promotionGroupCode[iP];
                        }
                    }
                    
                    book.StatusPayment = 0;
                    book.PaymentMethod = 1;
                    book.StatusBooking = 0;
                    book.sourceBy = booking.sourceBy;//1:web,0:app
                    book.IsBundle = booking.isBundle;
                    book.type = booking.type != null && booking.type.Length > 0 ? booking.type : "A";//A=Amadeus,K=Kiwi

                    book.Title = booking.contactInfo.title;
                    book.Firstname = booking.contactInfo.firstname;
                    book.Middlename = booking.contactInfo.middlename;
                    book.Lastname = booking.contactInfo.lastname;
                    book.TelNo = booking.contactInfo.telNo;
                    book.Email = booking.contactInfo.email;
                    book.CountryOfResidence = booking.contactInfo.countryCode;

                    book.UUID = booking.uuid;
                    book.UserID = booking.userID;
                    book.MemberOID = booking.memberOID;
                    string remark = "";
                    if (booking.remarks != null)
                    {
                        foreach (var r in booking.remarks)
                        {
                            remark += r + "|";
                        }
                    }
                    book.Remarks = remark;
                    book.TotalFare = booking.totalFare;
                    book.IsError = booking.isError;
                    book.ErrorMessage = booking.errorMessage;
                    if (booking.multiFlight != null && booking.multiFlight.Count > 0)
                    {
                        book.Origin = booking.multiFlight[0][0].depCity.code;
                        int last = booking.multiFlight[booking.multiFlight.Count - 1].Count;
                        book.Destination = booking.multiFlight[booking.multiFlight.Count - 1][last - 1].arrCity.code;
                    }
                    else
                    {
                        book.Origin = booking.origin.code;
                        book.Destination = booking.destination.code;
                    }

                    book.Wallet_Address = booking.Wallet_Address!=null && booking.Wallet_Address.Length>0? booking.Wallet_Address:"";
                    book.NFTFee = booking.NFTFee != null ? booking.NFTFee : 0;
                    FlightBookingFare adtFare = new FlightBookingFare();
                    if (booking.adtFare != null)
                    {
                        Guid fareId = Guid.NewGuid();
                        adtFare.FlightBookingFareOID = fareId;
                        adtFare.BookingOID = id;
                        adtFare.BaseFare = booking.adtFare.baseFare;
                        adtFare.SellingBaseFare = booking.adtFare.sellingBaseFare;
                        adtFare.Tax = booking.adtFare.tax;
                        adtFare.Qtax = booking.adtFare.qtax;
                        adtFare.LessFare = booking.adtFare.lessFare;
                        adtFare.Net = booking.adtFare.net;
                        adtFare.PessengerType = "adtFare";
                        _unitOfWork.FlightBookingFareRepository.Insert(adtFare);
                        _unitOfWork.Save();
                        if (booking.adtFare.baggages != null)
                        {
                            FlightBookingBaggage baggages = new FlightBookingBaggage();
                            Guid fareId1 = Guid.NewGuid();
                            baggages.FlightBookingBaggageOID = fareId1;
                            baggages.FlightBookingFareOID = fareId;
                            foreach (var r in booking.adtFare.baggages)
                            {
                                baggages.BaggageNo = r.baggageNo;
                                baggages.BaggageUnit = r.baggageUnit;
                            }
                            _unitOfWork.FlightBookingBaggageRepository.Insert(baggages);
                            _unitOfWork.Save();
                        }
                    }

                    FlightBookingFare chdFare = new FlightBookingFare();
                    if (booking.chdFare != null)
                    {
                        Guid fareId = Guid.NewGuid();
                        chdFare.FlightBookingFareOID = fareId;
                        chdFare.BookingOID = id;
                        chdFare.BaseFare = booking.chdFare.baseFare;
                        chdFare.SellingBaseFare = booking.chdFare.sellingBaseFare;
                        chdFare.Tax = booking.chdFare.tax;
                        chdFare.Qtax = booking.chdFare.qtax;
                        chdFare.LessFare = booking.chdFare.lessFare;
                        chdFare.Net = booking.chdFare.net;
                        chdFare.PessengerType = "chdFare";
                        _unitOfWork.FlightBookingFareRepository.Insert(chdFare);
                        _unitOfWork.Save();
                        if (booking.chdFare.baggages != null)
                        {
                            FlightBookingBaggage baggages = new FlightBookingBaggage();
                            Guid fareId1 = Guid.NewGuid();
                            baggages.FlightBookingBaggageOID = fareId1;
                            baggages.FlightBookingFareOID = fareId;
                            foreach (var r in booking.chdFare.baggages)
                            {
                                baggages.BaggageNo = r.baggageNo;
                                baggages.BaggageUnit = r.baggageUnit;
                            }
                            _unitOfWork.FlightBookingBaggageRepository.Insert(baggages);
                            _unitOfWork.Save();
                        }
                    }

                    FlightBookingFare infFare = new FlightBookingFare();
                    if (booking.infFare != null)
                    {
                        Guid fareId = Guid.NewGuid();
                        infFare.FlightBookingFareOID = fareId;
                        infFare.BookingOID = id;
                        infFare.BaseFare = booking.infFare.baseFare;
                        infFare.SellingBaseFare = booking.infFare.sellingBaseFare;
                        infFare.Tax = booking.infFare.tax;
                        infFare.Qtax = booking.infFare.qtax;
                        infFare.LessFare = booking.infFare.lessFare;
                        infFare.Net = booking.infFare.net;
                        infFare.PessengerType = "intFare";
                        _unitOfWork.FlightBookingFareRepository.Insert(infFare);
                        _unitOfWork.Save();
                        if (booking.infFare.baggages != null)
                        {
                            FlightBookingBaggage baggages = new FlightBookingBaggage();
                            Guid fareId1 = Guid.NewGuid();
                            baggages.FlightBookingBaggageOID = fareId1;
                            baggages.FlightBookingFareOID = fareId;
                            foreach (var r in booking.infFare.baggages)
                            {
                                baggages.BaggageNo = r.baggageNo;
                                baggages.BaggageUnit = r.baggageUnit;
                            }
                            _unitOfWork.FlightBookingBaggageRepository.Insert(baggages);
                            _unitOfWork.Save();
                        }
                    }

                    FlightBookingFlightDetail depFlight = new FlightBookingFlightDetail();
                    {
                        if (booking.depFlight != null && booking.depFlight.Count>0)
                        {
                            for (int i = 0; i < booking.depFlight.Count; i++)
                            {
                                depFlight = new FlightBookingFlightDetail();
                                depFlight.FlightBookingFlightDetailOID = Guid.NewGuid();
                                depFlight.BookingOID = id;
                                depFlight.FlightNumber = booking.depFlight[i].flightNumber;
                                depFlight.DepartureDateTime = booking.depFlight[i].departureDateTime;
                                depFlight.ArrivalDateTime = booking.depFlight[i].arrivalDateTime;
                                depFlight.FlightTime = booking.depFlight[i].flightTime;
                                depFlight.ConnectingTime = booking.depFlight[i].connectingTime;
                                depFlight.Rbd = booking.depFlight[i].rbd;
                                depFlight.FareBasis = booking.depFlight[i].fareBasis;
                                depFlight.AvailableSeat = booking.depFlight[i].availableSeat;
                                depFlight.FareType = booking.depFlight[i].fareType;
                                depFlight.Cabin = booking.depFlight[i].cabin;
                                depFlight.EquipmentType = booking.depFlight[i].equipmentType;
                                depFlight.DepCity = booking.depFlight[i].depCity.code;
                                depFlight.ArrCity = booking.depFlight[i].arrCity.code;
                                depFlight.Airline = booking.depFlight[i].airline.code;
                                depFlight.OperatedAirline = booking.depFlight[i].operatedAirline.code;
                                depFlight.TripType = "depart";
                                depFlight.Seq = i + 1;
                                _unitOfWork.FlightBookingFlightDetailRepository.Insert(depFlight);
                                _unitOfWork.Save();



                            }
                        }

                    }

                    FlightBookingFlightDetail retFlight = new FlightBookingFlightDetail();
                    {
                        if (booking.retFlight != null && booking.retFlight.Count>0)
                        {
                            
                            for (int i = 0; i < booking.retFlight.Count; i++)
                            {
                                retFlight = new FlightBookingFlightDetail();
                                retFlight.FlightBookingFlightDetailOID = Guid.NewGuid();
                                retFlight.BookingOID = id;
                                retFlight.FlightNumber = booking.retFlight[i].flightNumber;
                                retFlight.DepartureDateTime = booking.retFlight[i].departureDateTime;
                                retFlight.ArrivalDateTime = booking.retFlight[i].arrivalDateTime;
                                retFlight.FlightTime = booking.retFlight[i].flightTime;
                                retFlight.ConnectingTime = booking.retFlight[i].connectingTime;
                                retFlight.Rbd = booking.retFlight[i].rbd;
                                retFlight.FareBasis = booking.retFlight[i].fareBasis;
                                retFlight.AvailableSeat = booking.retFlight[i].availableSeat;
                                retFlight.FareType = booking.retFlight[i].fareType;
                                retFlight.Cabin = booking.retFlight[i].cabin;
                                retFlight.EquipmentType = booking.retFlight[i].equipmentType;
                                retFlight.DepCity = booking.retFlight[i].depCity.code;
                                retFlight.ArrCity = booking.retFlight[i].arrCity.code;
                                retFlight.Airline = booking.retFlight[i].airline.code;
                                retFlight.OperatedAirline = booking.retFlight[i].operatedAirline.code;
                                retFlight.TripType = "return";
                                retFlight.Seq = i + 1;
                                _unitOfWork.FlightBookingFlightDetailRepository.Insert(retFlight);
                                _unitOfWork.Save();


                            }
                           
                        }

                    }

                    FlightBookingFlightDetail multiFlight = new FlightBookingFlightDetail();
                    {
                        if (booking.multiFlight != null && booking.multiFlight.Count > 0)
                        {
                            for (int i = 0; i < booking.multiFlight.Count; i++)
                            {
                                for (int j = 0; j < booking.multiFlight[i].Count; j++)
                                {
                                    multiFlight = new FlightBookingFlightDetail();
                                    multiFlight.FlightBookingFlightDetailOID = Guid.NewGuid();
                                    multiFlight.BookingOID = id;
                                    multiFlight.FlightNumber = booking.multiFlight[i][j].flightNumber;
                                    multiFlight.DepartureDateTime = booking.multiFlight[i][j].departureDateTime;
                                    multiFlight.ArrivalDateTime = booking.multiFlight[i][j].arrivalDateTime;
                                    multiFlight.FlightTime = booking.multiFlight[i][j].flightTime;
                                    multiFlight.ConnectingTime = booking.multiFlight[i][j].connectingTime;
                                    multiFlight.Rbd = booking.multiFlight[i][j].rbd;
                                    multiFlight.FareBasis = booking.multiFlight[i][j].fareBasis;
                                    multiFlight.AvailableSeat = booking.multiFlight[i][j].availableSeat;
                                    multiFlight.FareType = booking.multiFlight[i][j].fareType;
                                    multiFlight.Cabin = booking.multiFlight[i][j].cabin;
                                    multiFlight.EquipmentType = booking.multiFlight[i][j].equipmentType;
                                    multiFlight.DepCity = booking.multiFlight[i][j].depCity.code;
                                    multiFlight.ArrCity = booking.multiFlight[i][j].arrCity.code;
                                    multiFlight.Airline = booking.multiFlight[i][j].airline.code;
                                    multiFlight.OperatedAirline = booking.multiFlight[i][j].operatedAirline.code;
                                    multiFlight.TripType = "multi" + (i + 1);
                                    multiFlight.Seq = j + 1;
                                    _unitOfWork.FlightBookingFlightDetailRepository.Insert(multiFlight);
                                    _unitOfWork.Save();

                                }

                            }
                        }

                    }



                    FlightBookingPaxInfo paxinfo = new FlightBookingPaxInfo();

                    if (booking.adtPaxs != null)
                    {
                        foreach (var r in booking.adtPaxs)
                        {
                            paxinfo = new FlightBookingPaxInfo();
                            paxinfo.FlightBookingPaxInfoOID = Guid.NewGuid();
                            paxinfo.BookingOID = id;
                            paxinfo.PaxType = r.paxType;
                            paxinfo.Title = r.title;
                            paxinfo.Firstname = r.firstname;
                            paxinfo.Middlename = r.middlename;
                            paxinfo.Lastname = r.lastname;
                            if (r.birthday.Year > 1900)
                            {
                                paxinfo.Birthday = r.birthday;
                            }
                            paxinfo.Email = r.email;
                            paxinfo.TelNo = r.telNo;
                            paxinfo.PassportNumber = r.passportNumber;
                            if (r.passportIssuingDate.Year > 1900)
                            {
                                paxinfo.PassportIssuingDate = r.passportIssuingDate;
                            }
                            if (r.passportExpiryDate.Year > 1900)
                            {
                                paxinfo.PassportExpiryDate = r.passportExpiryDate;
                            }
                            paxinfo.PassportIssuingCountry = r.passportIssuingCountry;
                            paxinfo.PassportNationality = r.passportNationality;
                            paxinfo.PessengerType = "adtPaxs";

                            paxinfo.KiwiBag = r.kiwiBag;
                            paxinfo.KiwiBagPrice = r.kiwiBagPrice;
                            paxinfo.KiwiBagWeight = r.kiwiBagWeight;
                            if (r.frequentFlyList != null && r.frequentFlyList.Count > 0)
                            {
                                paxinfo.FrequencyFlyerAirline = "";
                                paxinfo.FrequencyFlyerNumber = "";
                                foreach (var list in r.frequentFlyList)
                                {
                                    if (paxinfo.FrequencyFlyerAirline.Length > 0)
                                    {
                                        paxinfo.FrequencyFlyerAirline += "|";
                                        paxinfo.FrequencyFlyerNumber += "|";
                                    }
                                    paxinfo.FrequencyFlyerAirline += list.Airline;
                                    paxinfo.FrequencyFlyerNumber += list.Number;
                                }
                            }
                            else
                            {
                                paxinfo.FrequencyFlyerAirline = r.frequencyFlyerAirline;
                                paxinfo.FrequencyFlyerNumber = r.frequencyFlyerNumber;
                            }
                            paxinfo.MealRequest = r.mealRequest;
                            if (r.seatsRequest != null && r.seatsRequest.Count > 0)
                            {
                                paxinfo.SeatRequest = "";
                                foreach (var list in r.seatsRequest)
                                {
                                    if (paxinfo.SeatRequest.Length > 0)
                                    {
                                        paxinfo.SeatRequest += "|";
                                    }
                                    paxinfo.SeatRequest += list.seatRefNo;
                                }
                            }
                            else
                            {
                                paxinfo.SeatRequest = r.seatRequest;
                            }

                            _unitOfWork.FlightBookingPaxInfoRepository.Insert(paxinfo);
                            _unitOfWork.Save();
                        }

                    }


                    if (booking.chdPaxs != null)
                    {
                        foreach (var r in booking.chdPaxs)
                        {
                            paxinfo = new FlightBookingPaxInfo();
                            paxinfo.FlightBookingPaxInfoOID = Guid.NewGuid();
                            paxinfo.BookingOID = id;
                            paxinfo.PaxType = r.paxType;
                            paxinfo.Title = r.title;
                            paxinfo.Firstname = r.firstname;
                            paxinfo.Middlename = r.middlename;
                            paxinfo.Lastname = r.lastname;
                            if (r.birthday.Year > 1900)
                            {
                                paxinfo.Birthday = r.birthday;
                            }
                            paxinfo.Email = r.email;
                            paxinfo.TelNo = r.telNo;
                            paxinfo.PassportNumber = r.passportNumber;
                            if (r.passportIssuingDate.Year > 1900)
                            {
                                paxinfo.PassportIssuingDate = r.passportIssuingDate;
                            }
                            if (r.passportExpiryDate.Year > 1900)
                            {
                                paxinfo.PassportExpiryDate = r.passportExpiryDate;
                            }
                            paxinfo.PassportIssuingCountry = r.passportIssuingCountry;
                            paxinfo.PassportNationality = r.passportNationality;
                            paxinfo.PessengerType = "chdPaxs";

                            paxinfo.KiwiBag = r.kiwiBag;
                            paxinfo.KiwiBagPrice = r.kiwiBagPrice;
                            paxinfo.KiwiBagWeight = r.kiwiBagWeight;

                            if (r.frequentFlyList != null && r.frequentFlyList.Count > 0)
                            {
                                paxinfo.FrequencyFlyerAirline = "";
                                paxinfo.FrequencyFlyerNumber = "";
                                foreach (var list in r.frequentFlyList)
                                {
                                    if (paxinfo.FrequencyFlyerAirline.Length > 0)
                                    {
                                        paxinfo.FrequencyFlyerAirline += "|";
                                        paxinfo.FrequencyFlyerNumber += "|";
                                    }
                                    paxinfo.FrequencyFlyerAirline += list.Airline;
                                    paxinfo.FrequencyFlyerNumber += list.Number;
                                }
                            }
                            else
                            {
                                paxinfo.FrequencyFlyerAirline = r.frequencyFlyerAirline;
                                paxinfo.FrequencyFlyerNumber = r.frequencyFlyerNumber;
                            }
                            paxinfo.MealRequest = r.mealRequest;
                            if (r.seatsRequest != null && r.seatsRequest.Count > 0)
                            {
                                paxinfo.SeatRequest = "";
                                foreach (var list in r.seatsRequest)
                                {
                                    if (paxinfo.SeatRequest.Length > 0)
                                    {
                                        paxinfo.SeatRequest += "|";
                                    }
                                    paxinfo.SeatRequest += list.seatRefNo;
                                }
                            }
                            else
                            {
                                paxinfo.SeatRequest = r.seatRequest;
                            }

                            _unitOfWork.FlightBookingPaxInfoRepository.Insert(paxinfo);
                            _unitOfWork.Save();
                        }
                       
                    }
                   
                    if (booking.infPaxs != null)
                    {
                        foreach (var r in booking.infPaxs)
                        {
                            paxinfo = new FlightBookingPaxInfo();
                            paxinfo.FlightBookingPaxInfoOID = Guid.NewGuid();
                            paxinfo.BookingOID = id;
                            paxinfo.PaxType = r.paxType;
                            paxinfo.Title = r.title;
                            paxinfo.Firstname = r.firstname;
                            paxinfo.Middlename = r.middlename;
                            paxinfo.Lastname = r.lastname;
                            if (r.birthday.Year > 1900)
                            {
                                paxinfo.Birthday = r.birthday;
                            }
                            paxinfo.Email = r.email;
                            paxinfo.TelNo = r.telNo;
                            paxinfo.PassportNumber = r.passportNumber;
                            if (r.passportIssuingDate.Year > 1900)
                            {
                                paxinfo.PassportIssuingDate = r.passportIssuingDate;
                            }
                            if (r.passportExpiryDate.Year > 1900)
                            {
                                paxinfo.PassportExpiryDate = r.passportExpiryDate;
                            }
                            paxinfo.PassportIssuingCountry = r.passportIssuingCountry;
                            paxinfo.PassportNationality = r.passportNationality;
                            paxinfo.PessengerType = "infPaxs";

                            paxinfo.KiwiBag = r.kiwiBag;
                            paxinfo.KiwiBagPrice = r.kiwiBagPrice;
                            paxinfo.KiwiBagWeight = r.kiwiBagWeight;

                            if (r.frequentFlyList != null && r.frequentFlyList.Count > 0)
                            {
                                paxinfo.FrequencyFlyerAirline = "";
                                paxinfo.FrequencyFlyerNumber = "";
                                foreach (var list in r.frequentFlyList)
                                {
                                    if (paxinfo.FrequencyFlyerAirline.Length > 0)
                                    {
                                        paxinfo.FrequencyFlyerAirline += "|";
                                        paxinfo.FrequencyFlyerNumber += "|";
                                    }
                                    paxinfo.FrequencyFlyerAirline += list.Airline;
                                    paxinfo.FrequencyFlyerNumber += list.Number;
                                }
                            }
                            else
                            {
                                paxinfo.FrequencyFlyerAirline = r.frequencyFlyerAirline;
                                paxinfo.FrequencyFlyerNumber = r.frequencyFlyerNumber;
                            }
                            paxinfo.MealRequest = r.mealRequest;
                            if (r.seatsRequest != null && r.seatsRequest.Count > 0)
                            {
                                paxinfo.SeatRequest = "";
                                foreach (var list in r.seatsRequest)
                                {
                                    if (paxinfo.SeatRequest.Length > 0)
                                    {
                                        paxinfo.SeatRequest += "|";
                                    }
                                    paxinfo.SeatRequest += list.seatRefNo;
                                }
                            }
                            else
                            {
                                paxinfo.SeatRequest = r.seatRequest;
                            }

                            _unitOfWork.FlightBookingPaxInfoRepository.Insert(paxinfo);
                            _unitOfWork.Save();
                        }
                       
                    }


                    if (booking.fareRules != null)
                    {
                        foreach (var r in booking.fareRules)
                        {
                            FlightBookingFareRule farerule = new FlightBookingFareRule();
                            Guid fareruleID = Guid.NewGuid();
                            farerule.FlightBookingFareRuleOID = fareruleID;
                            farerule.BookingOID = id;
                            farerule.Origin = r.origin.code;
                            farerule.Destination = r.destination.code;
                            farerule.FareBasis = r.fareBasis;
                            foreach (var i in r.rules)
                            {
                                FlightBookingFareRuleDatail fareruledetail = new FlightBookingFareRuleDatail();
                                Guid fareruledetailID = Guid.NewGuid();
                                fareruledetail.FlightBookingFareRuleDatailOID = fareruledetailID;
                                fareruledetail.FlightBookingFareRuleOID = fareruleID;
                                fareruledetail.Category = i.category;
                                string fareRuleText = "";
                                foreach (var x in i.fareRuleText)
                                {
                                    fareRuleText += x + "|";
                                }
                                fareruledetail.FareRuleText = fareRuleText;
                                _unitOfWork.FlightBookingFareRuleDatailRepository.Insert(fareruledetail);
                            }
                            _unitOfWork.FlightBookingFareRuleRepository.Insert(farerule);
                            _unitOfWork.Save();

                        }
                    }


                    _unitOfWork.FlightBookingRepository.Insert(book);
                    _unitOfWork.Save();
                    scope.Complete();
                }
            }
            catch(Exception ex)
            {               
                Log.Error(ex);
                return Guid.Empty;
            }
            return id;
        }
        public void UpdateTransaction_Hash(Guid id, string transactionHash)
        {
            FlightBooking flightBooking = _unitOfWork.FlightBookingRepository.GetFirstOrDefault(x => x.BookingOID == id);

            if (flightBooking != null)
            {
                flightBooking.Transaction_Hash = transactionHash;
                _unitOfWork.FlightBookingRepository.DetachAll();
                _unitOfWork.FlightBookingRepository.Update(flightBooking);
                _unitOfWork.Save();
            }
        }

        public List<FlightBooking> GetFlightBookings(string transactionID)
        {
            List<FlightBooking> flightBookingList = _unitOfWork.FlightBookingRepository.GetMany(x => x.RobinhoodID == transactionID).ToList();
            return flightBookingList;
        }

        public FlightBooking GetFlightBooking(Guid id)
        {
            return _unitOfWork.FlightBookingRepository.GetFirstOrDefault(x => x.BookingOID == id);
        }

        public void SaveFlightBooking(FlightBooking flightBooking)
        {
            if (flightBooking != null)
            {
                _unitOfWork.FlightBookingRepository.DetachAll();
                _unitOfWork.FlightBookingRepository.Update(flightBooking);
                _unitOfWork.Save();
            }
        }
    }
}
