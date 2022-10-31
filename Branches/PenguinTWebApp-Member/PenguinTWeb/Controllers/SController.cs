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
    public class SController : Controller
    {
        private readonly IShortenURLServices _shortenURLServices;

        public SController(IShortenURLServices shortenURLServices)
        {
            this._shortenURLServices = shortenURLServices;
        }

        // GET: S
        public ActionResult U(string id)
        {
            var url = _shortenURLServices.GetById(id);
            if (url != null)
            {
                return View(url);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}