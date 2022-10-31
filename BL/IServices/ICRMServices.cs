using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface ICRMServices
    {
        BL.Entities.CRM.CRMProfile.UserProfile GetUserProfile(string uuid, string token, string language);
        void TriggerFlight(string uuid, string bookingKeyReference, string userID);
        void TriggerHotel(string uuid, string bookingKeyReference, string userID);
        void TriggerHF(string uuid, string bookingKeyReference, string userID);
        void TriggerTransfer(string uuid, string bookingKeyReference, string userID);
    }
}
