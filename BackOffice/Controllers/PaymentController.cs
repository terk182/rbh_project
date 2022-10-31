using BL;
using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BackOffice.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly IPaymentServices _paymentServices;

        public PaymentController(IPaymentServices paymentServices)

        {
            this._paymentServices = paymentServices;

        }

        //public ActionResult PaymentList()
        //{
        //    List<Payment> productList = _paymentServices.GetAll();
        //    return View(productList);
        //}

        public ActionResult PaymentDetail()
        {
            Payment productList = _paymentServices.GetAll();
            return View(productList);
        }

        //public ActionResult PaymentDetail(Guid id)
        //{
        //    Payment markup = _paymentServices.GetByID(id);
        //    if (markup == null)
        //    {
        //        markup = new Payment();
        //        markup.PaymentOID = Guid.NewGuid();

        //        //admin.Password = "";
        //    }

        //    return View(markup);
        //}


        [HttpPost]
        public ActionResult Details(Payment model)
        {
            Payment payment = _paymentServices.GetByID(model.PaymentOID);
            model.IsDelete = false;
            _paymentServices.SaveOrUpdate(model);
            return RedirectToAction("PaymentDetail", new { save = "t" });
        }

        public ActionResult MarkupDelete(Guid id)
        {
            var payment = _paymentServices.GetByID(id);
            payment.IsDelete = true;
            _paymentServices.SaveOrUpdate(payment);
            return RedirectToAction("PaymentList", new { save = "t" });
        }


    }
}