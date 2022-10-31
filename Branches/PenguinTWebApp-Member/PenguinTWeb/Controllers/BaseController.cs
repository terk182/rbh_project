using GogojiiWeb.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GogojiiWeb.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        public BaseController()
        {
            if (!String.IsNullOrEmpty(System.Web.HttpContext.Current.Request["lang"]))
            {
                Localize.SetLang(System.Web.HttpContext.Current.Request["lang"]);
            }

            ViewBag.WebMode = ConfigurationManager.AppSettings["WEBMODE"];
        }

        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }
    }
}