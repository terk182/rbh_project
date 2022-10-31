using BL;
using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BackOffice.Controllers
{
    public class MarkupFlightController : BaseController
    {
        private readonly IMarkupServices _markupFlightServices;

        public MarkupFlightController(IMarkupServices markupFlightServices)

        {
            this._markupFlightServices = markupFlightServices;

        }

        public ActionResult MarkupFlightList()
        {
            List<MarkupFlight> productList = _markupFlightServices.GetAll();
            return View(productList);
        }

        public ActionResult MarkupFlightDetail(Guid id)
        {
            MarkupFlight markup = _markupFlightServices.GetByID(id);
            if (markup == null)
            {
                markup = new MarkupFlight();
                markup.RobinhoodMarkupOID = new Guid();
                //markup.RobinhoodMarkupOID = Guid.NewGuid();
                markup.StartBookingDate = DateTime.Today;
                markup.FinishBookingDate = DateTime.Today;
                markup.StartTravelDate = DateTime.Today;
                markup.FinishTravelDate = DateTime.Today;
                markup.LV1Type = "Discount";
                markup.LV1TypeRT = "Discount";
                markup.IsPercentLV1 = true;
                markup.IsPercentLV1RT = true;
            }

            return View(markup);
        }


        [HttpPost]
        public ActionResult Details(MarkupFlight model, String StartBookingDate, String FinishBookingDate, String StartTravelDate, String FinishTravelDate, string btn_Save)
        {

            DateTime? startBooking = null;
            DateTime? finishBooking = null;
            //DateTime? startTravel = null;
            //DateTime? finishTravel = null;
            MarkupFlight markup = _markupFlightServices.GetByID(model.RobinhoodMarkupOID);
            string[] searchStartDate = StartBookingDate.Split('/');
            startBooking = new DateTime(int.Parse(searchStartDate[2]), int.Parse(searchStartDate[1]), int.Parse(searchStartDate[0]), 0, 0, 0);

            string[] searchFinishDate = FinishBookingDate.Split('/');
            finishBooking = new DateTime(int.Parse(searchFinishDate[2]), int.Parse(searchFinishDate[1]), int.Parse(searchFinishDate[0]), 0, 0, 0);

            //string[] searchTravelStartDate = StartTravelDate.Split('/');
            //startTravel = new DateTime(int.Parse(searchTravelStartDate[2]), int.Parse(searchTravelStartDate[1]), int.Parse(searchTravelStartDate[0]), 0, 0, 0);

            //string[] searchTravelFinishDate = FinishTravelDate.Split('/');
            //finishTravel = new DateTime(int.Parse(searchTravelFinishDate[2]), int.Parse(searchTravelFinishDate[1]), int.Parse(searchTravelFinishDate[0]), 0, 0, 0);

            model.StartBookingDate = Convert.ToDateTime(startBooking);
            model.FinishBookingDate = Convert.ToDateTime(finishBooking);
            model.IsDelete = false;
            model.LastUpdate = DateTime.Now;
            if (btn_Save == "2")
            {
                model.RobinhoodMarkupOID = Guid.NewGuid();
            }
            MarkupFlight flight = _markupFlightServices.GetDuplicate(model);
            if (flight == null)
            {
                if (model.MinPrice > model.MaxPrice)
                {
                    TempData["ErrorMessage"] = "Max Price must be more than Min Price";
                    return View("MarkupFlightDetail", model);
                }
                else if (model.MinPrice == model.MaxPrice)
                {
                    TempData["ErrorMessage"] = "Max Price must be more than Min Price";
                    return View("MarkupFlightDetail", model);
                }
                else if (model.RobinhoodMarkupOID == new Guid())
                {
                    model.RobinhoodMarkupOID = Guid.NewGuid();
                    _markupFlightServices.SaveOrUpdate(model);
                }
                else
                {
                    _markupFlightServices.SaveOrUpdate(model);
                }
                _markupFlightServices.RemoveCache();
            }
            else
            {
                TempData["ErrorMessage"] = "Your Data Duplicate";
                return View("MarkupFlightDetail", model);
            }
            return RedirectToAction("MarkupFlightList", new { save = "t" });
        }

        public ActionResult MarkupFlightDelete(Guid id)
        {
            var markup = _markupFlightServices.GetByID(id);
            markup.IsDelete = true;
            _markupFlightServices.SaveOrUpdate(markup);
            _markupFlightServices.RemoveCache();
            return RedirectToAction("MarkupFlightList", new { save = "t" });
        }

    }
}