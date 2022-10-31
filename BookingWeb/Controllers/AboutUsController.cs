using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TGBookingWeb.Controllers
{
    public class AboutUsController : Controller
    {
        // GET: AboutUs
        public ActionResult Index()
        {
            ViewBag.Menu = "3";
            return View();
        }
        
        public ActionResult TermOfService()
        {
            return View();
        }

        public ActionResult Privacy()
        {
            return View();
        }
    }
}