using BL;
using DataModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BackOffice.Controllers
{
    public class FlightDiscountTagController : BaseController
    {
        // GET: FlightDiscountTag
        private readonly IDiscountTagServices _discountTagServices;
        private readonly IPromotionGroupCodeServices _promotionGroupCodeServices;
        public FlightDiscountTagController(IDiscountTagServices discountTagServices, IPromotionGroupCodeServices promotionGroupCodeServices)

        {
            this._discountTagServices = discountTagServices;
            this._promotionGroupCodeServices = promotionGroupCodeServices;

        }

        public ActionResult PromotionGroupCodeList()
        {
            List<PromotionGroupCode> PromotionGroupCodeList = _promotionGroupCodeServices.GetAll();
            if (PromotionGroupCodeList == null)
            {
                PromotionGroupCodeList = new List<PromotionGroupCode>();
            }
            return View(PromotionGroupCodeList);
        }
        public ActionResult PromotionGroupCodeDetail(Guid id)
        {
            PromotionGroupCode promotion = _promotionGroupCodeServices.GetByID(id);
            if (promotion == null)
            {
                promotion = new PromotionGroupCode();
            }

            return View(promotion);
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult PromotionGroupCodeDetailSave(PromotionGroupCode model)
        {
            if (model.PromotionGroupCodeOID == Guid.Empty)
            {
                model.PromotionGroupCodeOID = Guid.NewGuid();
            }
            _promotionGroupCodeServices.SaveOrUpdate(model);
            return RedirectToAction("PromotionGroupCodeList", new { save = "t" });
        }

        public ActionResult DiscountTagList()
        {
            List<BL.Entities.DiscountTag.DiscountTagEntities> discountTags = _discountTagServices.GetAll();
            if (discountTags == null)
            {
                discountTags = new List<BL.Entities.DiscountTag.DiscountTagEntities>();
            }
            return View(discountTags);
        }
        public ActionResult DiscountTagDetail(Guid id)
        {
            BL.Entities.DiscountTag.DiscountTagEntities discountTags = _discountTagServices.GetByID(id);
            if (discountTags == null)
            {
                discountTags = new BL.Entities.DiscountTag.DiscountTagEntities();
                discountTags.discountTag = new DataModel.DiscountTag();
                discountTags.discountTag.DiscountTagOID = new Guid();
                discountTags.discountTag.IsActive = true;
                discountTags.discountTag.IsDelete = false;
                discountTags.discountTag.StartBookingDate = DateTime.Today;
                discountTags.discountTag.FinishBookingDate = DateTime.Today;
                discountTags.discountTag.LastUpdate = DateTime.Now;
                discountTags.discountTagDetail = new List<DataModel.DiscountTagDetail>();
                string[] arr = ConfigurationManager.AppSettings["webpages_LangCode"].Split('|');
                DataModel.DiscountTagDetail detail = new DataModel.DiscountTagDetail();
                for (int i = 0; i < arr.Length; i++)
                {
                    detail = new DataModel.DiscountTagDetail();
                    detail.LanguageCode = arr[i];
                    discountTags.discountTagDetail.Add(detail);
                }
                discountTags.promotionGroupCode = new PromotionGroupCode();
            }
            var cList = new List<object>();
            List<PromotionGroupCode> PromotionGroupCodeList = _promotionGroupCodeServices.GetAll();
            foreach(var group in PromotionGroupCodeList)
            {
                cList.Add(new
                {
                    ID = group.PromotionGroupCodeOID,
                    Name = group.PromotionGroupCode1
                });
            }
            var StatusPaymentList = new SelectList(cList, "ID", "Name", "");
            ViewData["PromotionGroupCodeList"] = StatusPaymentList;
            return View(discountTags);
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult DiscountTagDetailSave(BL.Entities.DiscountTag.DiscountTagEntities model)
        {
            if (model.discountTag.DiscountTagOID == Guid.Empty)
            {
                model.discountTag.DiscountTagOID = Guid.NewGuid();
            }

            string[] searchStartDate = Request["discountTag.StartBookingDate"].Split('/');
            DateTime startBooking = new DateTime(int.Parse(searchStartDate[2]), int.Parse(searchStartDate[1]), int.Parse(searchStartDate[0]), 0, 0, 0);

            string[] searchFinishDate = Request["discountTag.FinishBookingDate"].Split('/');
            DateTime finishBooking = new DateTime(int.Parse(searchFinishDate[2]), int.Parse(searchFinishDate[1]), int.Parse(searchFinishDate[0]), 0, 0, 0);

            model.discountTag.StartBookingDate = Convert.ToDateTime(startBooking);
            model.discountTag.FinishBookingDate = Convert.ToDateTime(finishBooking);
            model.discountTag.LastUpdate = DateTime.Now;
            _discountTagServices.SaveOrUpdate(model);
            return RedirectToAction("DiscountTagList", new { save = "t" });
        }
        public ActionResult DiscountTagDelete(Guid id)
        {
            BL.Entities.DiscountTag.DiscountTagEntities model = _discountTagServices.GetByID(id);
            model.discountTag.IsDelete = true;
            _discountTagServices.SaveOrUpdate(model);
            return RedirectToAction("DiscountTagList", new { save = "t" });
        }

    }
}