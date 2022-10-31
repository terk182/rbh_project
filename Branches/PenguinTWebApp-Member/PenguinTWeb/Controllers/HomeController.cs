using BL;
using GogojiiWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GogojiiWeb.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IFlightSearchServices _flightSearchServices;

        public HomeController(IFlightSearchServices flightSearchServices)
        {
            this._flightSearchServices = flightSearchServices;
            ViewBag.Menu = "1";
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Index()
        {
            SearchModel searchModel = new SearchModel();
            if (Session["ssSearch"] != null)
            {
                searchModel = (SearchModel)Session["ssSearch"];
            }

            return View(searchModel);
        }

        public ActionResult SystemProblem(string msg)
        {
            ViewBag.Message = Localize.Show(msg);

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public JsonResult SearchCity(string keyword, string language)
        {
            if (keyword.Length < 3)
            {
                return Json(new List<string>(), JsonRequestBehavior.AllowGet);
            }
            var cityList = _flightSearchServices.SearchCities(keyword, language, 20);
            return Json(cityList.GetAjaxList(), JsonRequestBehavior.AllowGet);
        }
    }
}