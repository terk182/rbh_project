using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BackOffice.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        public BackOfficeAdmin admin;
        System.Web.HttpContext current = System.Web.HttpContext.Current;
        public BaseController()
        {
            if (current.Session["Admin"] == null)
            {
                current.Response.Redirect("~/Account/Index");
            }
            else
            {
                //Check Permission
                admin = (BackOfficeAdmin)current.Session["admin"];
                ViewBag.Admin = admin;
            }
        }
    }
}