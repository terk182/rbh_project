using BL;
using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TGBookingWeb.Controllers
{
    public class APIConfigController : Controller
    {
        private readonly IAPIConfigServices _apiConfigServices;

        public APIConfigController(IAPIConfigServices apiConfigServices)
        {
            this._apiConfigServices = apiConfigServices;
        }
        // GET: APIConfig
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string PWD)
        {
            if (PWD == "P3ngu1nT+=")
            {
                Session.Add("ssAPIConfig", "T");
                return RedirectToAction("APIUserList");
            }
            return View();
        }

        public ActionResult APIUserList()
        {
            if (Session["ssAPIConfig"] == null)
            {
                return RedirectToAction("Index");
            }
            List<APIUser> userList = _apiConfigServices.GetAll();
            return View(userList);
        }

        public ActionResult APIUserDelete(Guid id)
        {
            if (Session["ssAPIConfig"] == null)
            {
                return RedirectToAction("Index");
            }
            _apiConfigServices.Delete(id);
            return RedirectToAction("APIUserList");
        }

        public ActionResult APIUserDetail(Guid id)
        {
            if (Session["ssAPIConfig"] == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Error = "";
            APIUser user = _apiConfigServices.GetById(id);
            if (user == null)
            {
                user = new APIUser();
                user.APIUserOID = Guid.NewGuid();
            }
            return View(user);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult APIUserDetail(APIUser user)
        {
            if (Session["ssAPIConfig"] == null)
            {
                return RedirectToAction("Index");
            }
            _apiConfigServices.SaveOrUpdate(user);
            return RedirectToAction("APIUserList");
        }
    }
}