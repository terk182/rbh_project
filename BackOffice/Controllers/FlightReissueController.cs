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
    public class FlightReissueController : BaseController
    {
        private readonly IFlightReportReissueServices _reportReissueServices;

        public FlightReissueController(IFlightReportReissueServices reportReissueServices)

        {
            this._reportReissueServices = reportReissueServices;

        }

        public ActionResult FlightReissuetList(List<AirFare.Reissue> reporReissuetList)
        {
            if (reporReissuetList == null)
            {
                reporReissuetList = _reportReissueServices.GetAllReissue();
            }
            return View(reporReissuetList);
        }



        public ActionResult FlightReissueDetail(Guid id)
        {
            AirFare airfare = _reportReissueServices.GetByID(id);
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
                Name = "Updated-เปลี่ยนแล้ว"
            });
            cList.Add(new
            {
                ID = "3",
                Name = "CustomerRequest-ลูกค้าเปลี่ยนเอง"
            });
            var StatusReissueList = new SelectList(cList, "ID", "Name", "");
            ViewData["StatusReissueList"] = StatusReissueList;

            cList = new List<object>();
            cList.Add(new
            {
                ID = "0",
                Name = "None-รอการระบุ"
            });
            cList.Add(new
            {
                ID = "1",
                Name = "Reissue"
            });
            cList.Add(new
            {
                ID = "2",
                Name = "Revalidate"
            });
            var TypeChange = new SelectList(cList, "ID", "Name", "");
            ViewData["TypeChangeList"] = TypeChange;

            cList = new List<object>();
            cList.Add(new
            {
                ID = "0",
                Name = "None-รอการระบุ"
            });
            cList.Add(new
            {
                ID = "1",
                Name = "Flight/Date Change"
            });
            cList.Add(new
            {
                ID = "2",
                Name = "Upgrade"
            });
            cList.Add(new
            {
                ID = "3",
                Name = "Re-route"
            });
            cList.Add(new
            {
                ID = "4",
                Name = "DOB-Change"
            });
            cList.Add(new
            {
                ID = "5",
                Name = "Title/Name/Midle/Surname Change"
            });
            var DetailChange = new SelectList(cList, "ID", "Name", "");
            ViewData["DetailChangeList"] = DetailChange;
            return View(airfare);
        }


        [HttpPost]
        public ActionResult FlightReportReissueDetailsave(AirFare model, String reissueCreateDate)
        {
            DateTime? Createdate = null;
            if (reissueCreateDate != null)
            {
                string[] createdate = reissueCreateDate.Split('/');
                Createdate = new DateTime(int.Parse(createdate[2]), int.Parse(createdate[1]), int.Parse(createdate[0]), 0, 0, 0);
                model.reissue.reissueCreateDate = Convert.ToDateTime(Createdate);
            }

            AirFare airfare = _reportReissueServices.GetByID(new Guid(model.bookingOID));
            airfare.reissue.status = model.reissue.status;
            airfare.reissue.newPNR = model.reissue.newPNR;
            airfare.reissue.reissueCreateDate = model.reissue.reissueCreateDate;
            airfare.reissue.typeChage = model.reissue.typeChage;
            airfare.reissue.detailChage = model.reissue.detailChage;
            airfare.refund.remark = model.reissue.remark;

            if (airfare.adtPaxs != null && airfare.adtPaxs.Count > 0)
            {
                for (int i = 0; i < airfare.adtPaxs.Count; i++)
                {
                    airfare.adtPaxs[i].netReissue = model.adtPaxs[i].netReissue;
                    airfare.adtPaxs[i].agentReissue = model.adtPaxs[i].agentReissue;
                    airfare.adtPaxs[i].reissueSelling = model.adtPaxs[i].reissueSelling;
                    airfare.adtPaxs[i].tickNoReissueOld = model.adtPaxs[i].tickNoReissueOld;
                    airfare.adtPaxs[i].tickNoReissueNew = model.adtPaxs[i].tickNoReissueNew;
                    airfare.adtPaxs[i].remarkReissue = model.adtPaxs[i].remarkReissue;
                    //airfare.adtPaxs[i].StatusReissue = model.adtPaxs[i].StatusReissue;
                }
            }

            if (airfare.chdPaxs != null && airfare.chdPaxs.Count > 0)
            {
                for (int i = 0; i < airfare.chdPaxs.Count; i++)
                {
                    airfare.chdPaxs[i].netReissue = model.chdPaxs[i].netReissue;
                    airfare.chdPaxs[i].agentReissue = model.chdPaxs[i].agentReissue;
                    airfare.chdPaxs[i].reissueSelling = model.chdPaxs[i].reissueSelling;
                    airfare.chdPaxs[i].tickNoReissueOld = model.chdPaxs[i].tickNoReissueOld;
                    airfare.chdPaxs[i].tickNoReissueNew = model.chdPaxs[i].tickNoReissueNew;
                    airfare.chdPaxs[i].remarkReissue = model.chdPaxs[i].remarkReissue;
                    //airfare.chdPaxs[i].StatusReissue = model.chdPaxs[i].StatusReissue;
                }
            }


            if (airfare.infPaxs != null && airfare.infPaxs.Count > 0)
            {
                for (int i = 0; i < airfare.infPaxs.Count; i++)
                {
                    airfare.infPaxs[i].netReissue = model.infPaxs[i].netReissue;
                    airfare.infPaxs[i].agentReissue = model.infPaxs[i].agentReissue;
                    airfare.infPaxs[i].reissueSelling = model.infPaxs[i].reissueSelling;
                    airfare.infPaxs[i].tickNoReissueOld = model.infPaxs[i].tickNoReissueOld;
                    airfare.infPaxs[i].tickNoReissueNew = model.infPaxs[i].tickNoReissueNew;
                    airfare.infPaxs[i].remarkReissue = model.infPaxs[i].remarkReissue;
                    //airfare.infPaxs[i].StatusReissue = model.infPaxs[i].StatusReissue;
                }
            }

            _reportReissueServices.SaveOrUpdateReissue(airfare);
            return RedirectToAction("FlightReissueDetail" + "/" + new Guid(model.bookingOID), new { save = "t" });
        }
    }
}