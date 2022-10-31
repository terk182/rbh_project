using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BackOffice.Utilities
{
    public static class AdminUtil
    {
        public static string adminName()
        {
            BackOfficeAdmin admin = (BackOfficeAdmin)System.Web.HttpContext.Current.Session["admin"];
            return admin.Firstname + " " + admin.Lastname;
        }

    }

}