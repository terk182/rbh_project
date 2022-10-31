using BL;
using DataModel;
using GogojiiWeb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GogojiiWeb.Controllers
{
    public class AirController : Controller
    {
        private readonly IAirPromotionServices _airPromotionServices;
        private readonly IShortenURLServices _shortenURLServices;
        private readonly INamingServices _namingServices;

        public AirController(IAirPromotionServices airPromotionServices, INamingServices namingServices, IShortenURLServices shortenURLServices)
        {
            this._airPromotionServices = airPromotionServices;
            this._namingServices = namingServices;
            this._shortenURLServices = shortenURLServices;
        }

        // GET: Air
        public ActionResult DeepLink()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DeepLink(string PWD)
        {
            if (PWD == "P3ngu1nT+=")
            {
                Session.Add("ssDeepLink", "T");
                return RedirectToAction("DeepLinkList");
            }
            return View();
        }


        public ActionResult ShortenList()
        {
            if (Session["ssDeepLink"] == null)
            {
                return RedirectToAction("DeepLink");
            }
            List<ShortenURL> shortList = _shortenURLServices.GetAll();
            return View(shortList);
        }

        public ActionResult ShortenDelete(string id)
        {
            if (Session["ssDeepLink"] == null)
            {
                return RedirectToAction("DeepLink");
            }
            _shortenURLServices.Delete(id);
            return RedirectToAction("ShortenList");
        }

        public ActionResult ShortenDetail()
        {
            if (Session["ssDeepLink"] == null)
            {
                return RedirectToAction("DeepLink");
            }
            ViewBag.Error = "";
            ShortenURL sURL = new ShortenURL();
            return View(sURL);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ShortenDetail(ShortenURL sURL)
        {
            if (Session["ssDeepLink"] == null)
            {
                return RedirectToAction("DeepLink");
            }

            if (_shortenURLServices.GetById(sURL.Code) != null)
            {
                ViewBag.Error = "This code has already been used";
                return View(sURL);
            }

            _shortenURLServices.SaveOrUpdate(sURL);
          
            
            return RedirectToAction("ShortenList");
        }
        public ActionResult DeepLinkList()
        {
            if (Session["ssDeepLink"] == null)
            {
                return RedirectToAction("DeepLink");
            }
            List<AirPromotion> proList =  _airPromotionServices.GetAll();
            return View(proList);
        }
        
        public ActionResult DeepLinkDelete(Guid id)
        {
            if (Session["ssDeepLink"] == null)
            {
                return RedirectToAction("DeepLink");
            }
            _airPromotionServices.Delete(id);
            return RedirectToAction("DeepLink");
        }

        public ActionResult DeepLinkDetail(Guid id)
        {
            if (Session["ssDeepLink"] == null)
            {
                return RedirectToAction("DeepLink");
            }
            AirPromotion pro = new AirPromotion();
            if (id != Guid.Empty)
            {
                pro = _airPromotionServices.GetById(id);
            }
            else
            {
                pro.AirPromotionOID = Guid.NewGuid();
                pro.DepartDate = DateTime.Today;
                pro.ReturnDate = DateTime.Today;
                pro.PeriodStart = DateTime.Today;
                pro.PeriodEnd = DateTime.Today;
                pro.Price = 0;
            }
            return View(pro);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DeepLinkDetail(AirPromotion pro, HttpPostedFileBase file)
        {
            if (Session["ssDeepLink"] == null)
            {
                return RedirectToAction("DeepLink");
            }
            pro.DepartDate = DateTime.ParseExact(Request["dDate"]
                , "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            pro.ReturnDate = DateTime.ParseExact(Request["rDate"]
                , "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            pro.PeriodStart = DateTime.ParseExact(Request["sDate"]
                , "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            pro.PeriodEnd = DateTime.ParseExact(Request["eDate"]
                , "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            _airPromotionServices.SaveOrUpdate(pro);

            if (file != null)
            {
                string pic = pro.AirPromotionOID.ToString() + ".jpg";
                string path = System.IO.Path.Combine(Server.MapPath("~/images/promotions"), pic);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                // file is uploaded
                file.SaveAs(path);

                // save the image path path to the database or you can send image 
                // directly to database
                // in-case if you want to store byte[] ie. for DB
                using (MemoryStream ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();
                }
            }
            return RedirectToAction("DeepLinkList");
        }

        public ActionResult Link(Guid id)
        {
            AirPromotion pro = new AirPromotion();
            pro = _airPromotionServices.GetById(id);
            return RedirectToAction("Promotion", new { id = pro.DeepLinkText, aid = pro.AirPromotionOID });
        }

        public ActionResult Promotion(string id, Guid aid)
        {
            AirPromotion pro = new AirPromotion();
            pro = _airPromotionServices.GetById(aid);

            if (pro.PeriodStart.GetValueOrDefault() > DateTime.Now || pro.PeriodEnd.GetValueOrDefault() < DateTime.Now || pro.DepartDate.GetValueOrDefault() < DateTime.Today.AddDays(3))
            {
                return RedirectToAction("Index", "Home");
            }

            string originCountryCode = _namingServices.GetCountryCode(pro.Origin.ToUpper());
            string destinationCountryCode = _namingServices.GetCountryCode(pro.Destination.ToUpper());

            string originCity = _namingServices.GetCityName(pro.Origin.ToUpper(), Localize.GetLang());
            if (originCity == "")
            {
                originCity = _namingServices.GetAirportName(pro.Origin.ToUpper(), false, Localize.GetLang());
            }
            string destinationCity = _namingServices.GetCityName(pro.Destination.ToUpper(), Localize.GetLang());
            if (destinationCity == "")
            {
                destinationCity = _namingServices.GetAirportName(pro.Destination.ToUpper(), false, Localize.GetLang());
            }


            ViewBag.From = String.Format("{0} ({1})", originCity, pro.Origin.ToUpper());
            ViewBag.To = String.Format("{0} ({1})", destinationCity, pro.Destination.ToUpper());


            SearchModel search = new SearchModel();
            search.departdate = pro.DepartDate.GetValueOrDefault().ToString("dd'/'MM'/'yyyy");
            search.returndate = pro.ReturnDate.GetValueOrDefault().ToString("dd'/'MM'/'yyyy");
            search.originCode = pro.Origin.ToUpper();
            search.destinationCode = pro.Destination.ToUpper();
            search.origin = String.Format("{0}[{1}] - {2}", originCity, pro.Origin.ToUpper(), _namingServices.GetCountryName(originCountryCode, Localize.GetLang()));
            search.destination = String.Format("{0}[{1}] - {2}", destinationCity, pro.Destination.ToUpper(), _namingServices.GetCountryName(destinationCountryCode, Localize.GetLang()));
            if (!String.IsNullOrEmpty(pro.AirlineCode))
            {
                search.airline = pro.AirlineCode.ToUpper();
            }
            ViewBag.SearchModel = search;
            return View(pro);
        }
    }
}