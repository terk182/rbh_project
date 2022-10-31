using BL.Entities;
using BL.Entities.RobinhoodFare;
using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface IFlightReportServices
    {
        List<AirFare> GetAll();
        List<AirFare.Refund> GetAllRefund();
        List<AirFare.Reissue> GetAllReissue();
        List<AirFare> GetAllBysearch(DateTime? StartDate, DateTime? FinishDate, DateTime? Paybefore_StartDate, DateTime? Paybefore_FinishDate,String StatusBooking,String PaymentMethod,String Platform, String ShowListNo, String faresfrom);
        List<AirFare> GetByPayBefore(DateTime StartPayBefore, DateTime FinishPayBefore);
        AirFare GetByID(Guid id);
        void SaveOrUpdate(AirFare airfare);
        void SendBookingEmail(Guid id, string subject, string languageCode);
        void SendBookingEmail(Guid id, string subject, System.Net.Mail.Attachment attachFile, string languageCode);
        void SaveOrUpdateRefund(AirFare airfare);
        void SaveOrUpdateReissue(AirFare airfare);
        bool ResendBookingEmail(Guid id, string subject, string Email);
        bool ResendBookingEmail(Guid id, string subject, string Email, System.Net.Mail.Attachment attachFile);
        //AirFare SendEmail(AirFare mail);
        void SendBookingEmail(string robinhoodID, string subject, string languageCode);
        bool ResendBookingEmail(string robinhoodID, string subject, string Email);
    }
}