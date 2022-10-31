
using DataModel.UnitOfWork;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class CRMServices : ICRMServices
    {
        private static readonly ILog Log =
              LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly UnitOfWork _unitOfWork;
        public CRMServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public BL.Entities.CRM.CRMProfile.UserProfile GetUserProfile(string uuid, string token, string language)
        {
            string jsonResult = Utilities.CRMConnection.GetResponse("profile", uuid, token, language);
            Log.Info("CRM Get Profile");
            Log.Debug(jsonResult);
            if (jsonResult == "")
            {
                return null;
            }
            BL.Entities.CRM.CRMProfile.UserProfile profile = BL.Entities.CRM.CRMProfile.UserProfile.FromJson(jsonResult);
            if (profile.Code != "200")
            {
                return null;
            }
            if (profile.Data == null)
            {
                return null;
            }

            return profile;
        }

        public void TriggerFlight(string uuid, string bookingKeyReference, string userID)
        {
            try
            {
                string requestJson = "\"booking_key_reference\": \"{0}\", \"user_id\": {1}";
                requestJson = "{ " + String.Format(requestJson, bookingKeyReference, String.IsNullOrEmpty(userID) ? "null" : userID) + " }";
                Log.Info("Trigger Flight Request");
                Log.Debug(requestJson);
                string jsonResult = Utilities.CRMConnection.PostResponse(requestJson, "flights/save-booking", uuid, "", "");
                Log.Info("Trigger Flight Response");
                Log.Debug(jsonResult);
            }
            catch { }
        }
        public void TriggerHotel(string uuid, string bookingKeyReference, string userID)
        {
            try
            { 
            string requestJson = "\"booking_key_reference\": \"{0}\", \"user_id\": {1}";
            requestJson = "{ " + String.Format(requestJson, bookingKeyReference, String.IsNullOrEmpty(userID) ? "null" : userID) + " }";
            Log.Info("Trigger Flight Request");
            Log.Debug(requestJson);
            string jsonResult = Utilities.CRMConnection.PostResponse(requestJson, "hotels/save-booking", uuid, "", "");
            Log.Info("Trigger Flight Response");
            Log.Debug(jsonResult);
            }
            catch { }
        }
        public void TriggerHF(string uuid, string bookingKeyReference, string userID)
        {
            try
            { 
            string requestJson = "\"booking_key_reference\": \"{0}\", \"user_id\": {1}";
            requestJson = "{ " + String.Format(requestJson, bookingKeyReference, String.IsNullOrEmpty(userID) ? "null" : userID) + " }";
            Log.Info("Trigger Flight Request");
            Log.Debug(requestJson);
            string jsonResult = Utilities.CRMConnection.PostResponse(requestJson, "bundles/save-booking", uuid, "", "");
            Log.Info("Trigger Flight Response");
            Log.Debug(jsonResult);
            }
            catch { }
        }
        public void TriggerTransfer(string uuid, string bookingKeyReference, string userID)
        {
            try
            { 
            string requestJson = "\"booking_key_reference\": \"{0}\", \"user_id\": {1}";
            requestJson = "{ " + String.Format(requestJson, bookingKeyReference, String.IsNullOrEmpty(userID) ? "null" : userID) + " }";
            Log.Info("Trigger Flight Request");
            Log.Debug(requestJson);
            string jsonResult = Utilities.CRMConnection.PostResponse(requestJson, "airport-transfers/save-booking", uuid, "", "");
            Log.Info("Trigger Flight Response");
            Log.Debug(jsonResult);
            }
            catch { }
        }
    }
}
