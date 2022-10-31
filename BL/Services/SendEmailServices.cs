using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using BL.Entities.RobinhoodFare;
using BL.Entities.RobinhoodPax;
using DataModel;
using DataModel.UnitOfWork;

namespace BL
{
    public class SendEmailServices : ISendEmailServices
    {
        private readonly UnitOfWork _unitOfWork;

        public SendEmailServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

       
        public AirFare GetByID(Guid id)
        {
            AirFare airfareEntity = new AirFare();
            FlightBooking flightBooking = _unitOfWork.FlightBookingRepository.GetFirstOrDefault(x => x.BookingOID == id);
            airfareEntity.bookingOID = flightBooking.BookingOID.ToString();
            airfareEntity.noOfAdults = flightBooking.NoOfAdults ?? 0;
            airfareEntity.noOfChildren = flightBooking.NoOfChildren ?? 0;

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

            return airfareEntity;
        }
    }
}
