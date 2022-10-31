using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace TGBookingWeb
{
    public class MemberUI
    {
        public static bool IsLogin()
        {
            //return HttpContext.Current.Session["ssUserProfie"] != null;
            return HttpContext.Current.Session["MEMBER_USER"] != null;
        }
       
        public static BL.Entities.CRM.CRMProfile.UserProfile GetUser()
        {
            return (BL.Entities.CRM.CRMProfile.UserProfile)HttpContext.Current.Session["ssUserProfie"];
        }
    }
}