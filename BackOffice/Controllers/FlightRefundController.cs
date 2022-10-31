using BL;
using BL.Entities.RobinhoodFare;
using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BackOffice.Controllers
{
    public class FlightRefundController : BaseController
    {
        private readonly IFlightReportRefundServices _reportRefundServices;

        public FlightRefundController(IFlightReportRefundServices reportRefundServices)

        {
            this._reportRefundServices = reportRefundServices;

        }

        public ActionResult FlightRefundtList(List<AirFare.Refund> reporRefundtList)
        {
            if (reporRefundtList == null)
            {
                reporRefundtList = _reportRefundServices.GetAllRefund();
            }
            return View(reporRefundtList);
        }


        public ActionResult FlightReportDetailEdit(Guid id)
        {
            AirFare airfare = _reportRefundServices.GetByID(id);
            var cList = new List<object>();

            cList.Add(new
            {
                ID = "MR",
                Name = "Mr. (นาย)"
            });
            cList.Add(new
            {
                ID = "MRS",
                Name = "Mrs. (นาง)"
            });
            cList.Add(new
            {
                ID = "MS",
                Name = "Ms. (นางสาว)"
            });
            cList.Add(new
            {
                ID = "MISS",
                Name = "Miss (นางสาว)"
            });
            var adtTitleList = new SelectList(cList, "ID", "Name", "");
            ViewData["adtTitleList"] = adtTitleList;

            cList = new List<object>();

            cList.Add(new
            {
                ID = "MSTR",
                Name = "Mstr. (เด็กชาย)"
            });
            cList.Add(new
            {
                ID = "MISS",
                Name = "Miss (เด็กหญิง)"
            });
            var chdTitleList = new SelectList(cList, "ID", "Name", "");
            ViewData["chdTitleList"] = chdTitleList;
            return View(airfare);
        }

        public ActionResult FlightRefundDetail(Guid id)
        {
            AirFare airfare = _reportRefundServices.GetByID(id);
            var cList = new List<object>();

            cList.Add(new
            {
                ID = "0",
                Name = "None-รอการระบุ"
            });
            cList.Add(new
            {
                ID = "1",
                Name = "Precressing-กำลังดำเนินการ"
            });
            cList.Add(new
            {
                ID = "2",
                Name = "Complete-เสร็จสิ้น"
            });

            var StatysRefundList = new SelectList(cList, "ID", "Name", "");
            ViewData["StatysRefundList"] = StatysRefundList;
            //airfare.refund.dueDateOfRefund = DateTime.Now.AddDays(90);
            //airfare.refund.refundGMDate = DateTime.Now;
            //airfare.refund.refundCreateDate = DateTime.Now;
            return View(airfare);
        }

        public ActionResult FlightReportReissueDetail(Guid id)
        {
            AirFare airfare = _reportRefundServices.GetByID(id);
            return View(airfare);
        }

        [HttpPost]
        public ActionResult FlightRefundDetailsave(AirFare model, String dueDateOfRefund, String refundCreateDate, String refundGMDate)
        {
            DateTime? Duedate = null;
            DateTime? Createdate = null;
            DateTime? GMDate = null;
            if (dueDateOfRefund != null)
            {
                string[] duedate = dueDateOfRefund.Split('/');
                Duedate = new DateTime(int.Parse(duedate[2]), int.Parse(duedate[1]), int.Parse(duedate[0]), 0, 0, 0);
                model.refund.dueDateOfRefund = Convert.ToDateTime(Duedate);
            }
            if (refundCreateDate != null)
            {
                string[] create = refundCreateDate.Split('/');
                Createdate = new DateTime(int.Parse(create[2]), int.Parse(create[1]), int.Parse(create[0]), 0, 0, 0);
                model.refund.refundCreateDate = Convert.ToDateTime(Createdate);
            }
            if (refundGMDate != null)
            {
                string[] gmdate = refundGMDate.Split('/');
                GMDate = new DateTime(int.Parse(gmdate[2]), int.Parse(gmdate[1]), int.Parse(gmdate[0]), 0, 0, 0);
                model.refund.refundGMDate = Convert.ToDateTime(GMDate);
            }
            AirFare airfare = _reportRefundServices.GetByID(new Guid(model.bookingOID));
            airfare.refund.status = model.refund.status;
            airfare.refund.newPNR = model.refund.newPNR;
            airfare.refund.refundNo = model.refund.refundNo;
            airfare.refund.refundCreateDate = model.refund.refundCreateDate;
            airfare.refund.refundGMDate = model.refund.refundGMDate;
            airfare.refund.remark = model.refund.remark;
            airfare.refund.dueDateOfRefund = model.refund.dueDateOfRefund;
            airfare.refund.oldPNR = model.PNR;

            if (airfare.adtPaxs != null && airfare.adtPaxs.Count > 0)
            {
                for (int i = 0; i < airfare.adtPaxs.Count; i++)
                {
                        airfare.adtPaxs[i].netRefund = model.adtPaxs[i].netRefund;
                        airfare.adtPaxs[i].agentRefund = model.adtPaxs[i].agentRefund;
                        airfare.adtPaxs[i].refundAmount = model.adtPaxs[i].refundAmount;
                        airfare.adtPaxs[i].tickNoRefund = model.adtPaxs[i].tickNoRefund;
                        airfare.adtPaxs[i].remarkRefund = model.adtPaxs[i].remarkRefund;
                        //airfare.adtPaxs[i].StatusRefund = model.adtPaxs[i].StatusRefund;
                    
                }
            }

            if (airfare.chdPaxs != null && airfare.chdPaxs.Count > 0)
            {
                for (int i = 0; i < airfare.chdPaxs.Count; i++)
                {
                        airfare.chdPaxs[i].netRefund = model.chdPaxs[i].netRefund;
                        airfare.chdPaxs[i].agentRefund = model.chdPaxs[i].agentRefund;
                        airfare.chdPaxs[i].refundAmount = model.chdPaxs[i].refundAmount;
                        airfare.chdPaxs[i].tickNoRefund = model.chdPaxs[i].tickNoRefund;
                        airfare.chdPaxs[i].remarkRefund = model.chdPaxs[i].remarkRefund;
                        //airfare.chdPaxs[i].StatusRefund = model.chdPaxs[i].StatusRefund;
                    
                }
            }


            if (airfare.infPaxs != null && airfare.infPaxs.Count > 0)
            {
                for (int i = 0; i < airfare.infPaxs.Count; i++)
                {
                        airfare.infPaxs[i].netRefund = model.infPaxs[i].netRefund;
                        airfare.infPaxs[i].agentRefund = model.infPaxs[i].agentRefund;
                        airfare.infPaxs[i].refundAmount = model.infPaxs[i].refundAmount;
                        airfare.infPaxs[i].tickNoRefund = model.infPaxs[i].tickNoRefund;
                        airfare.infPaxs[i].remarkRefund = model.infPaxs[i].remarkRefund;
                        //airfare.infPaxs[i].StatusRefund = model.infPaxs[i].StatusRefund;
                    
                }
            }

            _reportRefundServices.SaveOrUpdateRefund(airfare);
            return RedirectToAction("FlightRefundDetail" + "/" + new Guid(model.bookingOID), new { save = "t" });
        }

    }
}