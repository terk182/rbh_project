using BackOffice.Models;
using BL;
using BL.Entities.RobinhoodFare;
using BL.Entities.RobinhoodFlight;
using BL.Entities.RobinhoodPax;
using DataModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace BackOffice.Controllers
{
    public class ReportFlightController : BaseController
    {
        private readonly IFlightReportServices _reportServices;
        private readonly ICRMServices _crmServices;

        public ReportFlightController(IFlightReportServices reportServices,
            ICRMServices crmServices)

        {
            this._reportServices = reportServices;
            this._crmServices = crmServices;
        }

        public ActionResult FlightReportList(List<AirFare> reportList)
        {
            if (reportList == null)
            {
                reportList = _reportServices.GetAll();
            }
            return View(reportList);
        }

        public ActionResult SearchFlightList(String bookingdate, String paybefore, String BookingStatus, String PaymentMethod, String Platform, String ShowListNo, String faresfrom)
        {
            DateTime? StartDate = null;
            DateTime? FinishDate = null;
            DateTime? StartPayBefore = null;
            DateTime? FinishPayBefore = null;
            if (bookingdate != null && bookingdate != "")
            {
                string[] searchDate = bookingdate.Split('-');
                string SSearch = searchDate[0];
                string[] searchStartDate = SSearch.Trim().Split('/');
                StartDate = new DateTime(int.Parse(searchStartDate[2]), int.Parse(searchStartDate[1]), int.Parse(searchStartDate[0]), 0, 0, 0);

                SSearch = searchDate[1];
                string[] searchFinishDate = SSearch.Trim().Split('/');
                FinishDate = new DateTime(int.Parse(searchFinishDate[2]), int.Parse(searchFinishDate[1]), int.Parse(searchFinishDate[0]), 23, 59, 59);
            }

            if (paybefore != null && paybefore != "")
            {
                string[] searchDatePayBefore = paybefore.Split('-');
                string SSearchPayBefore = searchDatePayBefore[0];
                string[] searchStartDatePayBefore = SSearchPayBefore.Trim().Split('/');
                StartPayBefore = new DateTime(int.Parse(searchStartDatePayBefore[2]), int.Parse(searchStartDatePayBefore[1]), int.Parse(searchStartDatePayBefore[0]), 0, 0, 0);

                SSearchPayBefore = searchDatePayBefore[1];
                string[] searchFinishDatePayBefore = SSearchPayBefore.Trim().Split('/');
                FinishPayBefore = new DateTime(int.Parse(searchFinishDatePayBefore[2]), int.Parse(searchFinishDatePayBefore[1]), int.Parse(searchFinishDatePayBefore[0]), 23, 59, 59);
            }

            List<AirFare> reportList = _reportServices.GetAllBysearch(StartDate, FinishDate, StartPayBefore, FinishPayBefore, BookingStatus, PaymentMethod, Platform, ShowListNo, faresfrom);
            return View("FlightReportList", reportList);
        }

        public ActionResult FlightReportDetail(Guid id)
        {
            AirFare airfare = _reportServices.GetByID(id);
            var cList = new List<object>();
            cList.Add(new
            {
                ID = "0",
                Name = "Waiting for payment"
            });
            cList.Add(new
            {
                ID = "1",
                Name = "Paid"
            });
            cList.Add(new
            {
                ID = "2",
                Name = "Fail"
            });
            cList.Add(new
            {
                ID = "3",
                Name = "Refunding"
            });
            cList.Add(new
            {
                ID = "4",
                Name = "Refunded"
            });
            var StatusPaymentList = new SelectList(cList, "ID", "Name", "");
            ViewData["StatusPaymentList"] = StatusPaymentList;

            cList = new List<object>();
            cList.Add(new
            {
                ID = "0",
                Name = "Unknown"
            });
            cList.Add(new
            {
                ID = "1",
                Name = "Credit/Debit Card"
            });
            cList.Add(new
            {
                ID = "2",
                Name = "Installment By K-Bank Credit Card"
            });
            cList.Add(new
            {
                ID = "3",
                Name = "Promptpay QR Code"
            });
            cList.Add(new
            {
                ID = "4",
                Name = "Bank-Transfer"
            });
            cList.Add(new
            {
                ID = "5",
                Name = "Chillpay"
            });
            var PaymentMethod = new SelectList(cList, "ID", "Name", "");
            ViewData["PaymentMethodList"] = PaymentMethod;

            cList = new List<object>();
            cList.Add(new
            {
                ID = "0",
                Name = "New"
            });
            cList.Add(new
            {
                ID = "1",
                Name = "Confirmed"
            });
            cList.Add(new
            {
                ID = "2",
                Name = "Fail"
            });
            cList.Add(new
            {
                ID = "3",
                Name = "Cancelled"
            });
            cList.Add(new
            {
                ID = "4",
                Name = "Ticketed"
            });
            cList.Add(new
            {
                ID = "5",
                Name = "Refunded Ticket"
            });
            cList.Add(new
            {
                ID = "6",
                Name = "Reissue Ticket"
            });
            var Statusbooking = new SelectList(cList, "ID", "Name", "");
            ViewData["StatusBookingList"] = Statusbooking;


            return View(airfare);
        }

        public ActionResult FlightReportSendEmail(Guid id)
        {
            //subject = "Remider";
            _reportServices.SendBookingEmail(id, "Robinhood Payment Reminder","en");
            return RedirectToAction("FlightReportList", "ReportFlight");
        }
        //public ActionResult FlightReportSendEmail(Guid id)
        //{
        //    AirFare airfare = _reportServices.GetByID(id);
        //    //BuildEmailTemplateFlight(new Guid(airfare.bookingOID));
        //    Information info = _reportServices.GetAllInfo();
        //    try
        //    {

        //        MailMessage mailMsgSmtpClient = new MailMessage();
        //        mailMsgSmtpClient.Sender = new MailAddress("saharat.1234@outlook.com");
        //        mailMsgSmtpClient.BodyEncoding = Encoding.UTF8;
        //        mailMsgSmtpClient.IsBodyHtml = true;
        //        string MailForm = "saharat.1234@outlook.com";
        //        string MailTo = "saharat.1234@outlook.com";
        //        mailMsgSmtpClient.From = new MailAddress(MailForm);
        //        mailMsgSmtpClient.To.Add(new MailAddress(airfare.contactInfo.email));
        //        mailMsgSmtpClient.Bcc.Add(new MailAddress(MailTo));

        //        mailMsgSmtpClient.Subject = ("สวัสดีครับคุณ " + airfare.contactInfo.firstname + " เลขที่การจองของคุณคือ : " + airfare.PNR + " กรุณาชำระเงินด้วยครับ");
        //        string sETemplate = "";
        //        using (var writer = new StringWriter())
        //        {
        //            Controller controller = this;
        //            //AirFare emailM = new AirFare();
        //            //emailM.PNR = airfare.PNR;
        //            SendEmailFlight emailM = new SendEmailFlight();
        //            emailM.emailAirfare = new AirFare();
        //            emailM.emailAirfare.PNR = airfare.PNR;
        //            emailM.emailAirfare.statusPayment = airfare.statusPayment;

        //            emailM.emailAirfare.contactInfo = new BL.Entities.RobinhoodPax.ContactInfo();
        //            emailM.emailAirfare.contactInfo.title = airfare.contactInfo.title;
        //            emailM.emailAirfare.contactInfo.firstname = airfare.contactInfo.firstname;
        //            emailM.emailAirfare.contactInfo.lastname = airfare.contactInfo.lastname;
        //            emailM.emailAirfare.contactInfo.email = airfare.contactInfo.email;
        //            emailM.emailAirfare.contactInfo.telNo = airfare.contactInfo.telNo;

        //            emailM.emailAirfare.adtPaxs = new List<PaxInfo>();
        //            PaxInfo adtpressenger = new PaxInfo();
        //            for (int i = 0; i < airfare.adtPaxs.Count; i++)
        //            {
        //                adtpressenger = new PaxInfo();
        //                adtpressenger.title = airfare.adtPaxs[i].title;
        //                adtpressenger.firstname = airfare.adtPaxs[i].firstname;
        //                adtpressenger.middlename = airfare.adtPaxs[i].middlename;
        //                adtpressenger.lastname = airfare.adtPaxs[i].lastname;
        //                emailM.emailAirfare.adtPaxs.Add(adtpressenger);
        //            }

        //            emailM.emailAirfare.chdPaxs = new List<PaxInfo>();
        //            PaxInfo chdpressenger = new PaxInfo();
        //            if (airfare.chdPaxs != null && airfare.chdPaxs.Count > 0)
        //            {
        //                for (int i = 0; i < airfare.chdPaxs.Count; i++)
        //                {
        //                    chdpressenger = new PaxInfo();
        //                    chdpressenger.title = airfare.chdPaxs[i].title;
        //                    chdpressenger.firstname = airfare.chdPaxs[i].firstname;
        //                    chdpressenger.middlename = airfare.chdPaxs[i].middlename;
        //                    chdpressenger.lastname = airfare.chdPaxs[i].lastname;
        //                    emailM.emailAirfare.chdPaxs.Add(chdpressenger);
        //                }
        //            }

        //            emailM.emailAirfare.infPaxs = new List<PaxInfo>();
        //            PaxInfo infpressenger = new PaxInfo();
        //            if (airfare.infPaxs != null && airfare.infPaxs.Count > 0)
        //            {
        //                for (int i = 0; i < airfare.infPaxs.Count; i++)
        //                {
        //                    infpressenger = new PaxInfo();
        //                    infpressenger.title = airfare.infPaxs[i].title;
        //                    infpressenger.firstname = airfare.infPaxs[i].firstname;
        //                    infpressenger.middlename = airfare.infPaxs[i].middlename;
        //                    infpressenger.lastname = airfare.infPaxs[i].lastname;
        //                    emailM.emailAirfare.infPaxs.Add(infpressenger);
        //                }
        //            }

        //            NamingServices _namingServices = null;
        //            Airport airportdepart = new Airport(_namingServices, true, "en");
        //            Airline airlinedepart = new Airline(_namingServices, "en");
        //            Airport airportreturn = new Airport(_namingServices, true, "en");
        //            Airline airlinereturn = new Airline(_namingServices, "en");
        //            emailM.emailAirfare.depFlight = new List<FlightDetail>();
        //            FlightDetail depart = new FlightDetail();
        //            for (int i = 0; i < airfare.depFlight.Count; i++)
        //            {
        //                depart = new FlightDetail();
        //                airportdepart = new Airport(_namingServices, true, "en");
        //                airportdepart.code = airfare.depFlight[i].depCity.code;
        //                depart.depCity = airportdepart;
        //                airportreturn = new Airport(_namingServices, true, "en");
        //                airportreturn.code = airfare.depFlight[i].arrCity.code;
        //                depart.arrCity = airportreturn;
        //                airlinedepart = new Airline(_namingServices, "en");
        //                airlinedepart.code = airfare.depFlight[i].airline.code;
        //                depart.airline = airlinedepart;
        //                airlinereturn = new Airline(_namingServices, "en");
        //                airlinereturn.code = airfare.depFlight[i].operatedAirline.code;
        //                depart.operatedAirline = airlinereturn;
        //                depart.flightNumber = airfare.depFlight[i].flightNumber;
        //                depart.departureDateTime = airfare.depFlight[i].departureDateTime;
        //                depart.arrivalDateTime = airfare.depFlight[i].arrivalDateTime;
        //                depart.Seq = airfare.depFlight[i].Seq;
        //                emailM.emailAirfare.depFlight.Add(depart);
        //            }

        //            Airport airportdepart1 = new Airport(_namingServices, true, "en");
        //            Airline airlinedepart1 = new Airline(_namingServices, "en");
        //            Airport airportreturn1 = new Airport(_namingServices, true, "en");
        //            Airline airlinereturn1 = new Airline(_namingServices, "en");
        //            emailM.emailAirfare.retFlight = new List<FlightDetail>();
        //            FlightDetail returnflight = new FlightDetail();
        //            for (int i = 0; i < airfare.retFlight.Count; i++)
        //            {
        //                returnflight = new FlightDetail();
        //                airportdepart1 = new Airport(_namingServices, true, "en");
        //                airportdepart1.code = airfare.retFlight[i].depCity.code;
        //                returnflight.depCity = airportdepart1;
        //                airportreturn1 = new Airport(_namingServices, true, "en");
        //                airportreturn1.code = airfare.retFlight[i].arrCity.code;
        //                returnflight.arrCity = airportreturn1;
        //                airlinedepart1 = new Airline(_namingServices, "en");
        //                airlinedepart1.code = airfare.retFlight[i].airline.code;
        //                returnflight.airline = airlinedepart1;
        //                airlinereturn1 = new Airline(_namingServices, "en");
        //                airlinereturn1.code = airfare.retFlight[i].operatedAirline.code;
        //                returnflight.operatedAirline = airlinereturn1;
        //                returnflight.flightNumber = airfare.retFlight[i].flightNumber;
        //                returnflight.departureDateTime = airfare.retFlight[i].departureDateTime;
        //                returnflight.arrivalDateTime = airfare.retFlight[i].arrivalDateTime;
        //                returnflight.Seq = airfare.retFlight[i].Seq;
        //                emailM.emailAirfare.retFlight.Add(returnflight);
        //            }

        //            emailM.emailAirfare.adtFare = new Fare();
        //            emailM.emailAirfare.adtFare.baggages = new List<Baggage>();
        //            Baggage adtbaggaes = new Baggage();
        //            for (int i = 0; i < airfare.adtFare.baggages.Count; i++)
        //            {
        //                adtbaggaes = new Baggage();
        //                adtbaggaes.baggageNo = airfare.adtFare.baggages[i].baggageNo;
        //                adtbaggaes.baggageUnit = airfare.adtFare.baggages[i].baggageUnit;
        //                emailM.emailAirfare.adtFare.baggages.Add(adtbaggaes);
        //            }

        //            emailM.emailAirfare.chdFare = new Fare();
        //            emailM.emailAirfare.chdFare.baggages = new List<Baggage>();
        //            Baggage chdbaggaes = new Baggage();
        //            if (airfare.chdFare != null)
        //            {
        //                for (int i = 0; i < airfare.chdFare.baggages.Count; i++)
        //                {
        //                    chdbaggaes = new Baggage();
        //                    chdbaggaes.baggageNo = airfare.chdFare.baggages[i].baggageNo;
        //                    chdbaggaes.baggageUnit = airfare.chdFare.baggages[i].baggageUnit;
        //                    emailM.emailAirfare.chdFare.baggages.Add(chdbaggaes);
        //                }
        //            }

        //            emailM.emailAirfare.infFare = new Fare();
        //            emailM.emailAirfare.infFare.baggages = new List<Baggage>();
        //            Baggage infbaggaes = new Baggage();
        //            if (airfare.infFare != null)
        //            {
        //                for (int i = 0; i < airfare.infFare.baggages.Count; i++)
        //                {
        //                    infbaggaes = new Baggage();
        //                    infbaggaes.baggageNo = airfare.infFare.baggages[i].baggageNo;
        //                    infbaggaes.baggageUnit = airfare.infFare.baggages[i].baggageUnit;
        //                    emailM.emailAirfare.infFare.baggages.Add(infbaggaes);
        //                }
        //            }

        //            emailM.emailInformation = new Information();
        //            emailM.emailInformation.Address = info.Address;
        //            emailM.emailInformation.Name = info.Name;
        //            emailM.emailInformation.TelNo = info.TelNo;
        //            emailM.emailInformation.Email = info.Email;
        //            controller.ViewData.Model = emailM;
        //            var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, "FlightReportSendEmail");
        //            var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, writer);
        //            viewResult.View.Render(viewContext, writer);
        //            viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);
        //            sETemplate = writer.GetStringBuilder().ToString();
        //            mailMsgSmtpClient.Body = sETemplate;

        //            SmtpClient client = new SmtpClient();
        //            client.Host = "smtp-mail.outlook.com";
        //            client.Port = 587;
        //            client.EnableSsl = true;
        //            client.UseDefaultCredentials = false;
        //            client.DeliveryMethod = SmtpDeliveryMethod.Network;
        //            client.Credentials = new System.Net.NetworkCredential("saharat.1234@outlook.com", "saharat1234");
        //            client.Send(mailMsgSmtpClient);
        //        }
        //    }
        //    catch (Exception exc)
        //    {
        //    }
        //    //return View(airfare);
        //    return RedirectToAction("FlightReportList", "Report");
        //}

        //public void BuildEmailTemplateFlight(Guid regID)
        //{
        //    string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/Views/Report/") + "FlightReportSendEmail" + ".cshtml");
        //    var regInfo = db.FlightBookings.Where(x => x.BookingOID == regID).FirstOrDefault();
        //    var url = "http://localhost:61038/" + "Admin/ConfirmStaff?regId=" + regID;
        //    body = body.Replace("@ViewBag.ConfirmationLink", url);
        //    body = body.ToString();
        //    BuildEmailTemplateSend("จ่ายเงินด้วยนะครับคุณ " + regInfo.Firstname + " เลขการจองของคุณคือ : " + regInfo.PNR, body, regInfo.Email);
        //}

        //public static void BuildEmailTemplateSend(string subjectText, string bodyText, string sendTo)
        //{
        //    string from, to, bcc, cc, subject, body;
        //    from = "saharat.1234@outlook.com";
        //    to = sendTo.Trim();
        //    bcc = "";
        //    cc = "";
        //    subject = subjectText;
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append(bodyText);
        //    body = sb.ToString();
        //    MailMessage mail = new MailMessage();
        //    mail.From = new MailAddress(from);
        //    mail.To.Add(new MailAddress(to));
        //    if (!string.IsNullOrEmpty(bcc))
        //    {
        //        mail.Bcc.Add(new MailAddress(bcc));
        //    }
        //    if (!string.IsNullOrEmpty(cc))
        //    {
        //        mail.CC.Add(new MailAddress(cc));
        //    }
        //    mail.Subject = subject;
        //    mail.Body = body;
        //    mail.IsBodyHtml = true;
        //    SendEmail(mail);
        //}

        //public static void SendEmail(MailMessage mail)
        //{
        //    SmtpClient client = new SmtpClient();
        //    client.Host = "smtp-mail.outlook.com";
        //    client.Port = 587;
        //    client.EnableSsl = true;
        //    client.UseDefaultCredentials = false;
        //    client.DeliveryMethod = SmtpDeliveryMethod.Network;
        //    client.Credentials = new System.Net.NetworkCredential("saharat.1234@outlook.com", "saharat1234");
        //    try
        //    {
        //        client.Send(mail);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public ActionResult FlightReportDetailEdit(Guid id)
        {
            AirFare airfare = _reportServices.GetByID(id);
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
                ID = "0",
                Name = "Waiting for payment"
            });
            cList.Add(new
            {
                ID = "1",
                Name = "Paid"
            });
            cList.Add(new
            {
                ID = "2",
                Name = "Fail"
            });
            cList.Add(new
            {
                ID = "3",
                Name = "Refunding"
            });
            cList.Add(new
            {
                ID = "4",
                Name = "Refunded"
            });
            var StatusPaymentList = new SelectList(cList, "ID", "Name", "");
            ViewData["StatusPaymentList"] = StatusPaymentList;

            cList = new List<object>();
            cList.Add(new
            {
                ID = "0",
                Name = "Unknown"
            });
            cList.Add(new
            {
                ID = "1",
                Name = "Credit/Debit Card"
            });
            cList.Add(new
            {
                ID = "2",
                Name = "Installment By K-Bank Credit Card"
            });
            cList.Add(new
            {
                ID = "3",
                Name = "Promptpay QR Code"
            });
            cList.Add(new
            {
                ID = "4",
                Name = "Bank-Transfer"
            });
            cList.Add(new
            {
                ID = "5",
                Name = "Chillpay"
            });
            var PaymentMethod = new SelectList(cList, "ID", "Name", "");
            ViewData["PaymentMethodList"] = PaymentMethod;

            cList = new List<object>();
            cList.Add(new
            {
                ID = "0",
                Name = "New"
            });
            cList.Add(new
            {
                ID = "1",
                Name = "Confirmed"
            });
            cList.Add(new
            {
                ID = "2",
                Name = "Fail"
            });
            cList.Add(new
            {
                ID = "3",
                Name = "Cancelled"
            });
            cList.Add(new
            {
                ID = "4",
                Name = "Ticketed"
            });
            cList.Add(new
            {
                ID = "5",
                Name = "Refunded Ticket"
            });
            cList.Add(new
            {
                ID = "6",
                Name = "Reissue Ticket"
            });
            var Statusbooking = new SelectList(cList, "ID", "Name", "");
            ViewData["StatusBookingList"] = Statusbooking;
            
            return View(airfare);
        }

        public ActionResult FlightReportRefundDetail(Guid id)
        {
            AirFare airfare = _reportServices.GetByID(id);
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
            if (airfare.refund.status == 0)
            {
                airfare.refund.dueDateOfRefund = DateTime.Now;
                airfare.refund.refundGMDate = DateTime.Now;
                airfare.refund.refundCreateDate = DateTime.Now;
            }
            return View(airfare);
        }

        public ActionResult FlightReportReissueDetail(Guid id)
        {
            AirFare airfare = _reportServices.GetByID(id);
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

            if (airfare.reissue.status == 0)
            {
                airfare.reissue.reissueCreateDate = DateTime.Now;
            }
            return View(airfare);
        }

        [HttpPost]
        public ActionResult FlightReportDetailsave(AirFare model)
        {
            AirFare getairfare = _reportServices.GetByID(new Guid(model.bookingOID));
            //getairfare.TKTL = model.TKTL;
            getairfare.paymentMethod = model.paymentMethod;
            getairfare.statusPayment = model.statusPayment;
            getairfare.statusBooking = model.statusBooking;
            getairfare.note = model.note;
            getairfare.contactInfo.title = model.contactInfo.title;
            getairfare.contactInfo.firstname = model.contactInfo.firstname;
            getairfare.contactInfo.middlename = model.contactInfo.middlename;
            getairfare.contactInfo.lastname = model.contactInfo.lastname;
            getairfare.contactInfo.email = model.contactInfo.email;
            getairfare.contactInfo.telNo = model.contactInfo.telNo;
            getairfare.uuid = model.uuid;
            //getairfare.bookingOID = model.bookingOID;
            getairfare.userID = model.userID;

            //getairfare.grandTotal = model.grandTotal;
            //for (int i = 0; i < getairfare.adtPaxs.Count; i++)
            //{
            //    getairfare.adtPaxs[i].title = model.adtPaxs[i].title;
            //}

            //for (int i = 0; i < getairfare.chdPaxs.Count; i++)
            //{
            //    getairfare.chdPaxs[i].title = model.chdPaxs[i].title;
            //}

            //for (int i = 0; i < getairfare.infPaxs.Count; i++)
            //{
            //    getairfare.infPaxs[i].title = model.infPaxs[i].title;
            //}
            //getairfare.adtFare.sellingBaseFare = model.adtFare.sellingBaseFare;
            //if (getairfare.chdFare != null)
            //{
            //    getairfare.chdFare.sellingBaseFare = model.chdFare.sellingBaseFare;
            //}
            //if (getairfare.infFare != null)
            //{
            //    getairfare.infFare.sellingBaseFare = model.infFare.sellingBaseFare;
            //}
            _reportServices.SaveOrUpdate(getairfare);
            _crmServices.TriggerFlight(getairfare.uuid, getairfare.bookingOID, getairfare.userID);
           
            return RedirectToAction("FlightReportDetail" + "/" + new Guid(model.bookingOID), new { save = "t" });
        }

        [HttpPost]
        public ActionResult FlightReportRefundDetailsave(AirFare model, String dueDateOfRefund, String refundCreateDate, String refundGMDate)
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
            AirFare airfare = _reportServices.GetByID(new Guid(model.bookingOID));
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
                    if (airfare.adtPaxs[i].StatusRefund == false)
                    {
                        airfare.adtPaxs[i].netRefund = model.adtPaxs[i].netRefund;
                        airfare.adtPaxs[i].agentRefund = model.adtPaxs[i].agentRefund;
                        airfare.adtPaxs[i].refundAmount = model.adtPaxs[i].refundAmount;
                        airfare.adtPaxs[i].tickNoRefund = model.adtPaxs[i].tickNoRefund;
                        airfare.adtPaxs[i].remarkRefund = model.adtPaxs[i].remarkRefund;
                        airfare.adtPaxs[i].StatusRefund = model.adtPaxs[i].StatusRefund;
                    }
                }
            }

            if (airfare.chdPaxs != null && airfare.chdPaxs.Count > 0)
            {
                for (int i = 0; i < airfare.chdPaxs.Count; i++)
                {
                    if (airfare.chdPaxs[i].StatusRefund == false)
                    {
                        airfare.chdPaxs[i].netRefund = model.chdPaxs[i].netRefund;
                        airfare.chdPaxs[i].agentRefund = model.chdPaxs[i].agentRefund;
                        airfare.chdPaxs[i].refundAmount = model.chdPaxs[i].refundAmount;
                        airfare.chdPaxs[i].tickNoRefund = model.chdPaxs[i].tickNoRefund;
                        airfare.chdPaxs[i].remarkRefund = model.chdPaxs[i].remarkRefund;
                        airfare.chdPaxs[i].StatusRefund = model.chdPaxs[i].StatusRefund;
                    }
                }
            }


            if (airfare.infPaxs != null && airfare.infPaxs.Count > 0)
            {
                for (int i = 0; i < airfare.infPaxs.Count; i++)
                {
                    if (airfare.infPaxs[i].StatusRefund == false)
                    {
                        airfare.infPaxs[i].netRefund = model.infPaxs[i].netRefund;
                        airfare.infPaxs[i].agentRefund = model.infPaxs[i].agentRefund;
                        airfare.infPaxs[i].refundAmount = model.infPaxs[i].refundAmount;
                        airfare.infPaxs[i].tickNoRefund = model.infPaxs[i].tickNoRefund;
                        airfare.infPaxs[i].remarkRefund = model.infPaxs[i].remarkRefund;
                        airfare.infPaxs[i].StatusRefund = model.infPaxs[i].StatusRefund;
                    }
                }
            }

            _reportServices.SaveOrUpdateRefund(airfare);
            return RedirectToAction("FlightReportDetail" + "/" + new Guid(model.bookingOID), new { save = "t" });
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

            AirFare airfare = _reportServices.GetByID(new Guid(model.bookingOID));
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
                    airfare.adtPaxs[i].StatusReissue = model.adtPaxs[i].StatusReissue;
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
                    airfare.chdPaxs[i].StatusReissue = model.chdPaxs[i].StatusReissue;
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
                    airfare.infPaxs[i].StatusReissue = model.infPaxs[i].StatusReissue;
                }
            }

            _reportServices.SaveOrUpdateReissue(airfare);
            return RedirectToAction("FlightReportDetail" + "/" + new Guid(model.bookingOID), new { save = "t" });
        }
        //public ActionResult FlightReportDetail(Guid id)
        //{
        //    AirFare airfare = _reportServices.GetByID(id);
        //    return View(airfare);
        //    //FlightBooking flightbook = _reportServices.GetByID(id);
        //    //if (flightbook == null)
        //    //{
        //    //    flightbook = new FlightBooking();
        //    //    flightbook.BookingOID = Guid.NewGuid();

        //    //    //admin.Password = "";
        //    //}
        //    //FlightBookingBaggage baggage = _reportServices.GetByID(id);
        //    //if (baggage == null)
        //    //{
        //    //    baggage = new FlightBookingBaggage();
        //    //    baggage.FlightBookingBaggageOID = Guid.NewGuid();

        //    //    //admin.Password = "";
        //    //}
        //}
    }
}