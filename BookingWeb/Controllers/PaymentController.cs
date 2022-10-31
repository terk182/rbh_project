using BL;
using BL.Utilities;
using DataModel;
using TGBookingWeb.Models;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace TGBookingWeb.Controllers
{
    public class PaymentController : Controller
    {
        private static readonly ILog Log =
                 LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IFlightSearchServices _flightSearchServices;
        private readonly IFlightBookingServices _flightBookingServices;
        private readonly IFlightReportServices _flightReportServices;
        private readonly IKBankChargeServices _kbankChargeServices;
        private readonly IPaymentServices _paymentServices;
        private readonly ICRMServices _crmServices;
        private readonly IRunningNumberServices _runningNumberServices;
        private string mode = "";

        public PaymentController(IFlightReportServices flightReportServices,
            IFlightSearchServices flightSearchServices,
            IFlightBookingServices flightBookingServices,
            IKBankChargeServices kbankChargeServices,
            IPaymentServices paymentServices,
            ICRMServices crmServices,
            IRunningNumberServices runningNumberServices)
        {
            this._flightSearchServices = flightSearchServices;
            this._flightReportServices = flightReportServices;
            this._flightBookingServices = flightBookingServices;
            this._kbankChargeServices = kbankChargeServices;
          
            this._paymentServices = paymentServices;
            this._crmServices = crmServices;
            this._runningNumberServices = runningNumberServices;
            ViewBag.KBankButtonLink = ConfigurationManager.AppSettings["KBANK.Button.URL"];
            ViewBag.KBankMerchantID = ConfigurationManager.AppSettings["KBANK.MerchantID"];
            ViewBag.KBankSmartPayMerchantID = ConfigurationManager.AppSettings["KBANK.SmartPayMerchantID"];
            ViewBag.KBankSmartPayID = ConfigurationManager.AppSettings["KBANK.SmartPayID"];
            ViewBag.KBankAPIKEY = ConfigurationManager.AppSettings["KBANK.APIKEY"];

            if (System.Web.HttpContext.Current.Session["ssMode"] != null)
            {
                mode = (string)System.Web.HttpContext.Current.Session["ssMode"];
            }
            else
            {
                mode = "";
            }
            ViewBag.Mode = mode;
            if (!String.IsNullOrEmpty(System.Web.HttpContext.Current.Request["lang"]))
            {
                Localize.SetLang(System.Web.HttpContext.Current.Request["lang"]);
            }
            if (!String.IsNullOrEmpty(System.Web.HttpContext.Current.Request["currency"]))
            {
                Currency.SetCurrency(System.Web.HttpContext.Current.Request["currency"]);
            }
        }
        // GET: Payment

        public ActionResult Pay(Guid id)
        {
            if (Request["mode"] == "app")
            {
                Session.Add("ssMode", "app");
            }
            else
            {
                Session.Add("ssMode", "");
            }
            var flight = this._flightReportServices.GetByID(id);
            if (flight != null)
            {
                if (flight.statusPayment != 0)
                {
                    return RedirectToAction("SystemProblem", "Home", new { msg = "Please check your booking status" });
                }
                if (DateTime.Now > flight.TKTL)
                {
                    return RedirectToAction("SystemProblem", "Home", new { msg = "Payment Timeout" });
                }
                return RedirectToAction("PaymentFlight", new { id = 3, fid = id });

            }
           
            return View();
        }

        public ActionResult Test()
        {
            return View();
        }
        public ActionResult PaymentFlight(string id, Guid fid)
        {
            BL.Entities.RobinhoodFare.AirFare airfare = this._flightReportServices.GetByID(fid);
            ViewBag.PNR = airfare.PNR;
            Session.Add("PaymentPrice", airfare.totalFare.ToString("N2").Replace(",", ""));

            if (String.IsNullOrEmpty(id))
            {
                id = "1";
            }

            ViewBag.PaymentID = id;
            return View(airfare);
        }

        
        //Kbank Full
        public ActionResult Payment1(string price, string product, Guid pid)
        {
            ViewBag.SubmitURL = Url.Action("InitialKBank", new { product = product, pid = pid, method = "F" });
            ViewBag.Price = price;
            return PartialView();
        }

        //installment with kbank
        public ActionResult Payment2(string price, string product, Guid pid, string month)
        {
            if (String.IsNullOrEmpty(month))
            {
                month = "3";
            }
            ViewBag.Month = month;
            ViewBag.SubmitURL = Url.Action("InitialKBank", new { product = product, pid = pid, method = "I" });
            Dictionary<int, string> plans = new Dictionary<int, string>();
            decimal dPrice = Convert.ToDecimal(price);
            var payment = _paymentServices.GetAll();
            decimal interestRate = payment.KbankInstallValue.GetValueOrDefault();
            bool isPct = payment.IspercentKbankInstall.GetValueOrDefault();
            string installmentDetail = "{0} {1} x ฿{2} ({3} ฿{4})";
            decimal interest = 0;
            decimal total = 0;
            decimal monthly = 0;
            List<int> iplan = new List<int>() { 3, 6, 10 };
            foreach (var i in iplan)
            {
                if (dPrice >= (i * 1000))
                {
                    if (isPct)
                    {
                        interest = dPrice * interestRate * 0.01M * i;
                    }
                    else
                    {
                        interest = interestRate * i;
                    }
                    total = dPrice + interest;
                    monthly = decimal.Round(total / i, 2, MidpointRounding.AwayFromZero);
                    total = monthly * i;

                    plans.Add(i, String.Format(installmentDetail, i, Localize.Show("MONTHS"), monthly.ToString("N2"), Localize.Show("TOTAL"), total.ToString("N2")));

                    if (month == i.ToString())
                    {
                        ViewBag.Price = total.ToString("N2").Replace(",", "");
                    }
                }
            }
            return PartialView(plans);
        }

        //Promptpay QR
        public ActionResult Payment3(string price, string product, Guid pid)
        {
            ViewBag.SubmitURL = Url.Action("QRPay", new { product = product, pid = pid });
            string orderNo = "";
            if (Session["QROrder_" + pid.ToString()] != null)
            {
                orderNo = (string)Session["QROrder_" + pid.ToString()];
            }
            else
            {
                string robinhoodID = "";
                if (product == "A")
                {
                    BL.Entities.RobinhoodFare.AirFare airfare = this._flightReportServices.GetByID(pid);
                    robinhoodID = airfare.RobinhoodID;
                }
                
                KBankQROrder order = new KBankQROrder();
                order.currency = "THB";
                order.amount = price;
                order.description = "Gogojii Booking - " + robinhoodID;
                order.source_type = "qr";
                order.reference_order = DateTime.Now.ToString("yyMMddHHmmss") + robinhoodID;

                var o = order.createOrder();
                orderNo = o.id;
                Session["QROrder_" + pid.ToString()] = orderNo;
            }
            ViewBag.OrderNo = orderNo;
            ViewBag.Price = price;
            return PartialView();
        }

        //Bank transfer
        public ActionResult Payment4(string price, string product, Guid pid)
        {
            if (product == "A")
            {
                BL.Entities.RobinhoodFare.AirFare airfare = this._flightReportServices.GetByID(pid);
                ViewBag.Deadline = airfare.TKTL.ToString("ddd dd MMM yyyy, HH:mm");
            }
            ViewBag.SubmitURL = Url.Action("Transfer", new { product = product, pid = pid });
            ViewBag.Price = price;
            return PartialView();
        }

        //chill-pay
        public ActionResult Payment5(string price, string product, Guid pid)
        {

            PaymentReference payRef = new PaymentReference();
            payRef.PaymentRefID = Guid.NewGuid();
            payRef.CreateDateTime = DateTime.Now;
            payRef.BookingID = pid;
            int payID = this._runningNumberServices.GetNumber("CHILLPAY" + DateTime.Today.ToString("yyMMdd"));
            
            payRef.PaymentType = "CHILLPAY";
            payRef.Product = product;

            if (product == "A")
            {
                BL.Entities.RobinhoodFare.AirFare airfare = this._flightReportServices.GetByID(pid);
                ViewBag.description = DateTime.Now.ToString("yyMMddHHmmss") + "_" + product + "_" + airfare.RobinhoodID;
                payRef.PaymentRefOrderNo = airfare.RobinhoodID + payID.ToString().PadLeft(4, '0');
                
            }
            
            ViewBag.orderno = payRef.PaymentRefOrderNo;

            this._paymentServices.SaveOrUpdatePaymentReference(payRef);

            ViewBag.url = ConfigurationManager.AppSettings["CHILLPAY.GATEWAYURL"];
            ViewBag.merchantid = ConfigurationManager.AppSettings["CHILLPAY.MERCHANT"];
            try
            {
                ViewBag.amount = Convert.ToDouble(price).ToString("N2").Replace(",","").Replace(".", "");
            }
            catch(Exception ex)
            {
                Log.Error("amount error", ex);
                ViewBag.amount = price;
            }
            
            ViewBag.customerid = pid;
            ViewBag.mobileno = "";
            ViewBag.clientip = Request.UserHostAddress;
            ViewBag.routeno = "1";
            ViewBag.currency = "764";
            ViewBag.apikey = ConfigurationManager.AppSettings["CHILLPAY.APIKEY"];

            return PartialView();
        }

        public ActionResult ChillpayBG()
        {
            Log.Debug("TransactionId=" + Request["TransactionId"]);
            Log.Debug("Amount=" + Request["Amount"]);
            Log.Debug("OrderNo=" + Request["OrderNo"]);
            Log.Debug("CustomerId=" + Request["CustomerId"]);
            Log.Debug("BankCode=" + Request["BankCode"]);
            Log.Debug("PaymentDate=" + Request["PaymentDate"]);
            Log.Debug("PaymentStatus=" + Request["PaymentStatus"]);
            Log.Debug("BankRefCode=" + Request["BankRefCode"]);
            Log.Debug("CurrentDate=" + Request["CurrentDate"]);
            Log.Debug("CurrentTime=" + Request["CurrentTime"]);
            Log.Debug("PaymentDescription=" + Request["PaymentDescription"]);
            Log.Debug("CreditCardToken=" + Request["CreditCardToken"]);
            Log.Debug("Currency=" + Request["Currency"]);
            Log.Debug("CheckSum=" + Request["CheckSum"]);

            ChillPayBackground bg = new ChillPayBackground();
            bg.ChillPayBackgroundID = Guid.NewGuid();
            bg.ResponseType = "B";
            bg.CreateDateTime = DateTime.Now;
            bg.TransactionId = Request["TransactionId"];
            bg.Amount = Convert.ToDecimal(Request["Amount"]);
            bg.OrderNo = Request["OrderNo"];
            bg.CustomerId = Request["CustomerId"];
            bg.BankCode = Request["BankCode"];
            bg.PaymentDate = Request["PaymentDate"];
            bg.PaymentStatus = Request["PaymentStatus"];
            bg.BankRefCode = Request["BankRefCode"];
            bg.CurrentDate = Request["CurrentDate"];
            bg.CurrentTime = Request["CurrentTime"];
            bg.PaymentDescription = Request["PaymentDescription"];
            bg.CreditCardToken = Request["CreditCardToken"];
            bg.Currency = Request["Currency"];
            bg.CheckSum = Request["CheckSum"];
            this._paymentServices.SaveOrUpdateChillPayBackground(bg);

            Log.Debug("order " + bg.OrderNo);

            PaymentReference payRef = this._paymentServices.GetPaymentReferenceByOrderNo(bg.OrderNo, "CHILLPAY");
            DateTime payDate = new DateTime(int.Parse(bg.PaymentDate.Substring(0, 4)), int.Parse(bg.PaymentDate.Substring(4, 2)), int.Parse(bg.PaymentDate.Substring(6, 2)), int.Parse(bg.PaymentDate.Substring(8, 2)), int.Parse(bg.PaymentDate.Substring(10, 2)), int.Parse(bg.PaymentDate.Substring(12, 2)));
            if (bg.PaymentStatus == "0")
            {
                Log.Debug("payment success");
                if (payRef != null)
                {
                    Log.Debug("pay ref not null");
                    updatePaymentSuccess(payRef.Product, payRef.BookingID.Value, 5, bg.TransactionId, payDate);//5=chillpay
                }
                else
                {
                    Log.Debug("pay ref null");
                }
            }
            else
            {
                Log.Debug("payment fail ");
                if (payRef != null)
                {
                    Log.Debug("pay ref not null");

                    if (payRef.Product == "A")
                    {
                        Log.Debug("update payment");
                        _flightBookingServices.UpdatePayment(payRef.BookingID.Value, 2, 5, 0, 0, payDate);
                        BL.Entities.RobinhoodFare.AirFare airfare = this._flightReportServices.GetByID(payRef.BookingID.Value);
                        _crmServices.TriggerFlight(airfare.uuid, payRef.BookingID.Value.ToString(), airfare.userID);
                    }
                    
                }
                else
                {
                    Log.Debug("pay ref null");
                }
            }

            return View();
        }

        public ActionResult ChillpayResult()
        {
            Log.Debug("respCode=" + Request["respCode"]);
            Log.Debug("status=" + Request["status"]);
            Log.Debug("transNo=" + Request["transNo"]);
            Log.Debug("orderNo=" + Request["orderNo"]);


            ChillPayResult result = new ChillPayResult();
            result.ChillPayResultID = Guid.NewGuid();
            result.CreateDateTime = DateTime.Now;
            result.respCode = Request["respCode"];
            result.status = Request["status"];
            result.transNo = Request["transNo"];
            result.orderNo = Request["orderNo"];

            bool success = false;
            bool doInquiry = false; 
            PaymentReference payRef = new PaymentReference();

            if (result.orderNo != null && result.transNo != null)
            {
                this._paymentServices.SaveOrUpdateChillPayResult(result);
                payRef = this._paymentServices.GetPaymentReferenceByOrderNo(result.orderNo, "CHILLPAY");
               
                ChillPayBackground bg = this._paymentServices.GetChillPayBackgroundByOrderNo(result.orderNo);

                if (bg != null && bg.OrderNo == result.orderNo)
                {
                    Log.Debug("found bg");
                    if (bg.PaymentStatus == "0")
                    {
                        Log.Debug("bg success");
                        success = true;
                    }
                    else
                    {
                        Log.Debug("bg not success");
                        doInquiry = true;
                    } 
                }
                else
                {
                    Log.Debug("inquiry");
                    doInquiry = true;
                }
                
                if(doInquiry)
                {
                    Log.Debug("start inquiry");
                    bool bInquirySuccess = sendInquiry(result.transNo);

                    if(bInquirySuccess)
                    {
                        Log.Debug("inquiry sucess");
                        updatePaymentSuccess(payRef.Product, payRef.BookingID.Value, 5, result.transNo, result.CreateDateTime.Value);//5=chillpay
                        success = true;
                    }
                    else
                    {
                        Log.Debug("inquiry fail");
                        success = false;
                    }
                }
            }

            if(success)
            {
                if (payRef.Product == "A")
                {
                    var book = this._flightReportServices.GetByID(new Guid(payRef.BookingID.Value.ToString()));
                    mode = book.sourceBy == 1 ? "web" : "app";
                    if (mode == "app")
                    {
                        return RedirectToAction("Success", "Payment", new { bookingKeyReference = payRef.BookingID.Value.ToString(), product = "A", paymentMethod = 5 });
                    }
                    return RedirectToAction("Voucher", "Flight", new { id = payRef.BookingID.Value.ToString() });
                }
                
            }
            else
            {
                if (payRef.Product == "A")
                {
                    var book = this._flightReportServices.GetByID(new Guid(payRef.BookingID.Value.ToString()));
                    mode = book.sourceBy == 1 ? "web" : "app";
                }
               
                if (mode == "app")
                {
                    return RedirectToAction("Fail", "Payment", new { bookingKeyReference = payRef.BookingID.Value.ToString(), product = payRef.Product, paymentMethod = 5 });
                }
                return RedirectToAction("SystemProblem", "Home", new { msg = "Payment Fail" });
            }
            return View();
        }

        private void updatePaymentSuccess(string product, Guid id, int paymentMethod, string TransNo, DateTime paymentDate)
        {
            string _langCode = Localize.GetLang();
            Log.Debug("_langCode=" + _langCode);
            if (product == "A")
            {
                Log.Debug("update payment");
                _flightBookingServices.UpdatePayment(id, 1, paymentMethod, 0, 0, paymentDate);
                Log.Debug("update status");
                _flightBookingServices.UpdateBookingStatus(id, 1);
                Log.Debug("before send mail");
                BL.Entities.RobinhoodFare.AirFare airfare = this._flightReportServices.GetByID(id);
                _crmServices.TriggerFlight(airfare.uuid, id.ToString(), airfare.userID);
                Log.Debug("airfare.PNR=" + airfare.PNR);
                //for NFT
                if (airfare.Wallet_Address != null && airfare.Wallet_Address.Length > 0)
                {
                    NFTRequest request = new NFTRequest();
                    //request.iditem = Convert.ToInt32(DateTime.Now.ToString("ddHHmmss"));
                    request.to = airfare.Wallet_Address;
                    request.metadata = String.Format("{0}Transaction/Data/?transactionID={1}", ConfigurationManager.AppSettings["Main.URL"].ToString(), airfare.bookingOID);
                    string requestJson = JsonConvert.SerializeObject(request);
                   
                    string url = ConfigurationManager.AppSettings["NFT.Mint"].ToString();
                    string json = BL.Utilities.HttpUtility.postJSON(url, requestJson, "");
                    Log.Debug(requestJson);
                    Log.Debug(json);
                    NFTResponse nftresponse = new NFTResponse();
                    if (String.IsNullOrEmpty(json) == false)
                    {
                        json = json.Replace("@", "");

                        nftresponse = JsonConvert.DeserializeObject<NFTResponse>(json);

                        string OfficeID = ConfigurationManager.AppSettings["OfficeID"];
                        List<string> remarks = new List<string>();
                        remarks.Add("PAYMENT BY: BANKTRANSFER");
                        remarks.Add("NFT FEE : " + airfare.NFTFee.ToString("N2"));
                        remarks.Add("NFT Hash:" + nftresponse.Hash);
                        string errMsg = "";
                        _flightSearchServices.RetrieveAndAddRemark(airfare.PNR, OfficeID, "AISOFT", remarks,false, ref errMsg);
                        _flightBookingServices.UpdateTransaction_Hash(id, nftresponse.Hash);
                    }
                }
                _flightReportServices.SendBookingEmail(id, "Booking Confirmation - " + airfare.RobinhoodID, _langCode);
            }
            
            
        }

        private bool sendInquiry(string transactionId)
        {
            bool success = false;
            try
            {
                string merchantCode = ConfigurationManager.AppSettings["CHILLPAY.MERCHANT"];
                string apiKey = ConfigurationManager.AppSettings["CHILLPAY.APIKEY"];
                string md5SecretKey = ConfigurationManager.AppSettings["CHILLPAY.MD5KEY"];

                string phraseKey = merchantCode + transactionId + apiKey + md5SecretKey;
                string checkSum = "";
                using (MD5 md5 = MD5.Create())
                {
                    byte[] retVal = md5.ComputeHash(Encoding.UTF8.GetBytes(phraseKey));

                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < retVal.Length; i++)
                    {
                        sb.Append(retVal[i].ToString("x2"));
                    }
                    checkSum = sb.ToString();
                    Log.Debug("checkSum=" + checkSum);
                }

                ChillPayInquiry chillPayInquiry = new ChillPayInquiry();
                chillPayInquiry.ChillPayInquiryID = Guid.NewGuid();
                chillPayInquiry.CreateDateTime = DateTime.Now;
                chillPayInquiry.MerchantCode = merchantCode;
                chillPayInquiry.ApiKey = apiKey;
                chillPayInquiry.CheckSum = checkSum;
                chillPayInquiry.TransactionId = transactionId;
                this._paymentServices.SaveOrUpdateChillPayInquiry(chillPayInquiry);

                SimpleHTTP simpleHttp = new SimpleHTTP();
                simpleHttp.url = ConfigurationManager.AppSettings["CHILLPAY.INQUIRYURL"];

                PostData data = new PostData();
                data.name = "MerchantCode";
                data.value = merchantCode;
                simpleHttp.postDatas.Add(data);

                data = new PostData();
                data.name = "TransactionId";
                data.value = transactionId;
                simpleHttp.postDatas.Add(data);

                data = new PostData();
                data.name = "ApiKey";
                data.value = apiKey;
                simpleHttp.postDatas.Add(data);

                data = new PostData();
                data.name = "CheckSum";
                data.value = checkSum;
                simpleHttp.postDatas.Add(data);

                string res = simpleHttp.Post();
                Log.Debug("res=" + res);

                try
                {
                    ChillPayBackground bg = JsonConvert.DeserializeObject<ChillPayBackground>(res);
                    bg.ChillPayBackgroundID = Guid.NewGuid();
                    bg.CreateDateTime = DateTime.Now;
                    bg.ResponseType = "I";
                    this._paymentServices.SaveOrUpdateChillPayBackground(bg);

                    chillPayInquiry.ChillPayBackgroundID = bg.ChillPayBackgroundID;
                    chillPayInquiry.Status = (bg.PaymentStatus == "0") ? true : false;
                    this._paymentServices.SaveOrUpdateChillPayInquiry(chillPayInquiry);

                    if (chillPayInquiry.Status.Value)
                    {
                        Log.Debug("success inquiry");
                        success = true;
                    }
                }
                catch (Exception ex1)
                {
                    Log.Error("bind bg fail", ex1);
                }

            }
            catch (Exception ex)
            {
                Log.Error("sendInquiry fail", ex);
            }

            return success;
        }

        public ActionResult InitialKBank(string product, Guid pid, string method)
        {

            KBankChargeModel chargeModel = new KBankChargeModel();
            chargeModel.currency = "THB";
            chargeModel.source_type = "card";
            chargeModel.mode = "token";
            chargeModel.ref1 = product;
            chargeModel.ref2 = pid.ToString();
            chargeModel.token = Request["token"];

            if (product == "A")
            {
                BL.Entities.RobinhoodFare.AirFare airfare = this._flightReportServices.GetByID(pid);
                chargeModel.amount = airfare.grandTotal.ToString("N2").Replace(",", "");
                chargeModel.description = "Flight Booking - " + airfare.RobinhoodID;
                chargeModel.reference_order = DateTime.Now.ToString("yyMMddHHmmss") + airfare.RobinhoodID;
            }
            
            Log.Debug("charge = " + chargeModel.amount);
            Log.Debug("lang=" + Localize.GetLang());
            decimal totalPay = Convert.ToDecimal(chargeModel.amount);
            decimal monthly = 0;
            int iMonth = 0;
            if (method == "I")
            {
                string month = Request["ggMonth"];
                var payment = _paymentServices.GetAll();
                decimal interestRate = payment.KbankInstallValue.GetValueOrDefault();
                bool isPct = payment.IspercentKbankInstall.GetValueOrDefault();
                decimal interest = 0;
                decimal dPrice = Convert.ToDecimal(chargeModel.amount);
                iMonth = int.Parse(month);
                if (isPct)
                {
                    interest = dPrice * interestRate * 0.01M * iMonth;
                }
                else
                {
                    interest = interestRate * iMonth;
                }
                totalPay = dPrice + interest;
                monthly = decimal.Round(totalPay / iMonth, 2, MidpointRounding.AwayFromZero);
                totalPay = monthly * iMonth;

                chargeModel.amount = totalPay.ToString("N2").Replace(",", "");


                chargeModel.additional_data = new KBankAdditionalData();
                chargeModel.additional_data.mid = ConfigurationManager.AppSettings["KBANK.SmartPayMerchantID"];
                chargeModel.additional_data.smartpay_id = ConfigurationManager.AppSettings["KBANK.SmartPayID"];
                chargeModel.additional_data.term = month;
            }

            if (product == "A")
            {
                _flightBookingServices.UpdatePayment(pid, 0, method == "I" ? 2 : 1, iMonth, monthly, DateTime.Now);
                _flightBookingServices.UpdateFinalPrice(pid, totalPay);
            } 
            

            var response = chargeModel.chargeAPI();
            KBankCharge kc = new KBankCharge();
            kc.ChargeID = response.id;
            kc.message = response.json;
            kc.ref1 = chargeModel.ref1;
            kc.ref2 = chargeModel.ref2;
            kc.status = response.status;
            kc.transaction_state = response.transaction_state;
            kc.mode = mode;
            _kbankChargeServices.Save(kc);

            if (response.transaction_state == "Authorized")
            {
                string _langCode = Localize.GetLang();
                Log.Debug("_langCode=" + _langCode);
                if (product == "A")
                {
                    _flightBookingServices.UpdatePayment(pid, 1, 0, 0, 0, DateTime.Now);
                    _flightBookingServices.UpdateBookingStatus(pid, 1);
                    BL.Entities.RobinhoodFare.AirFare airfare = this._flightReportServices.GetByID(new Guid(kc.ref2));
                    _crmServices.TriggerFlight(airfare.uuid, kc.ref2, airfare.userID);
                    Log.Debug("airfare.PNR=" + airfare.PNR);
                    Log.Debug("lang=" + Localize.GetLang());
                    //for NFT
                    if (airfare.Wallet_Address != null && airfare.Wallet_Address.Length > 0)
                    {
                        NFTRequest request = new NFTRequest();
                        //request.iditem = Convert.ToInt32(DateTime.Now.ToString("ddHHmmss"));
                        request.to = airfare.Wallet_Address;
                        request.metadata = JsonConvert.SerializeObject(request);//String.Format("{0}Transaction/Data/?transactionID={1}", ConfigurationManager.AppSettings["Main.URL"].ToString(), airfare.bookingOID);
                        string requestJson = JsonConvert.SerializeObject(request);
                       
                        string url = ConfigurationManager.AppSettings["NFT.Mint"].ToString();
                        string json = BL.Utilities.HttpUtility.postJSON(url, requestJson, "");
                        Log.Debug(requestJson);
                        Log.Debug(json);
                        NFTResponse nftresponse = new NFTResponse();
                        if (String.IsNullOrEmpty(json) == false)
                        {
                            json = json.Replace("@", "");

                            nftresponse = JsonConvert.DeserializeObject<NFTResponse>(json);

                            string OfficeID = ConfigurationManager.AppSettings["OfficeID"];
                            List<string> remarks = new List<string>();
                            remarks.Add("PAYMENT BY: BANKTRANSFER");
                            remarks.Add("NFT FEE : " + airfare.NFTFee.ToString("N2"));
                            remarks.Add("NFT Hash:" + nftresponse.Hash);
                            string errMsg = "";
                            _flightSearchServices.RetrieveAndAddRemark(airfare.PNR, OfficeID, "AISOFT", remarks,false, ref errMsg);
                            _flightBookingServices.UpdateTransaction_Hash(pid, nftresponse.Hash);
                        }
                    }
                    this._flightReportServices.SendBookingEmail(pid, "Booking Confirmation - " + airfare.RobinhoodID, _langCode);
                    if (mode == "app")
                    {
                        return RedirectToAction("Success", "Payment", new { bookingKeyReference = pid, product = "A", paymentMethod = 1 });
                    }
                    return RedirectToAction("Voucher", "Flight", new { id = pid });
                }
                
            }
            else if (response.transaction_state == "Pre-Authorized")
            {
                return Redirect(response.redirect_url);
            }
            else
            {
                //Fail
                if (product == "A")
                {
                    _flightBookingServices.UpdatePayment(pid, 2, 0, 0, 0, DateTime.Now);
                    BL.Entities.RobinhoodFare.AirFare airfare = this._flightReportServices.GetByID(new Guid(kc.ref2));
                    _crmServices.TriggerFlight(airfare.uuid, kc.ref2, airfare.userID);
                }
                
            }
            if (mode == "app")
            {
                return RedirectToAction("Fail", "Payment", new { bookingKeyReference = pid, product = product, paymentMethod = 1 });
            }
            return RedirectToAction("SystemProblem", "Home", new { msg = "Payment Fail" });
        }

        public ActionResult QRPay(string product, Guid pid)
        {
            Log.Debug("QRPay");
            KBankQRInquiry inquiry = new KBankQRInquiry();
            inquiry.charge_id = Request["chargeId"];

            decimal total = 0;
            if (product == "A")
            {
                BL.Entities.RobinhoodFare.AirFare airfare = this._flightReportServices.GetByID(pid);
                total = airfare.grandTotal;
            }
            

            if (product == "A")
            {
                _flightBookingServices.UpdatePayment(pid, 0, 3, 0, 0, DateTime.Now);
                _flightBookingServices.UpdateFinalPrice(pid, total);
            }
            

            var response = inquiry.chargeAPI();
            KBankCharge kc = new KBankCharge();
            kc.ChargeID = response.id;
            kc.message = response.json;
            kc.ref1 = product + "-QR";
            kc.ref2 = pid.ToString();
            kc.status = response.status;
            kc.transaction_state = response.transaction_state;
            kc.mode = mode;
            _kbankChargeServices.Save(kc);

            if (response.transaction_state == "Authorized")
            {
                string _langCode = Localize.GetLang();
                Log.Debug("_langCode=" + _langCode);
                if (product == "A")
                {
                    _flightBookingServices.UpdatePayment(pid, 1, 0, 0, 0, DateTime.Now);
                    _flightBookingServices.UpdateBookingStatus(pid, 1);
                    BL.Entities.RobinhoodFare.AirFare airfare = this._flightReportServices.GetByID(new Guid(kc.ref2));
                    _crmServices.TriggerFlight(airfare.uuid, kc.ref2, airfare.userID);
                    Log.Debug("airfare.PNR="+ airfare.PNR);
                    //for NFT
                    if (airfare.Wallet_Address != null && airfare.Wallet_Address.Length > 0)
                    {
                        NFTRequest request = new NFTRequest();
                        //request.iditem = Convert.ToInt32(DateTime.Now.ToString("ddHHmmss"));
                        request.to = airfare.Wallet_Address;
                        request.metadata = String.Format("{0}Transaction/Data/?transactionID={1}", ConfigurationManager.AppSettings["Main.URL"].ToString(), airfare.bookingOID);
                        string requestJson = JsonConvert.SerializeObject(request);
                       
                        string url = ConfigurationManager.AppSettings["NFT.Mint"].ToString();
                        string json = BL.Utilities.HttpUtility.postJSON(url, requestJson, "");
                        Log.Debug(requestJson);
                        Log.Debug(json);
                        NFTResponse nftresponse = new NFTResponse();
                        if (String.IsNullOrEmpty(json) == false)
                        {
                            json = json.Replace("@", "");

                            nftresponse = JsonConvert.DeserializeObject<NFTResponse>(json);

                            string OfficeID = ConfigurationManager.AppSettings["OfficeID"];
                            List<string> remarks = new List<string>();
                            remarks.Add("PAYMENT BY: BANKTRANSFER");
                            remarks.Add("NFT FEE : " + airfare.NFTFee.ToString("N2"));
                            remarks.Add("NFT Hash:" + nftresponse.Hash);
                            string errMsg = "";
                            _flightSearchServices.RetrieveAndAddRemark(airfare.PNR, OfficeID, "AISOFT", remarks,false, ref errMsg);
                            _flightBookingServices.UpdateTransaction_Hash(pid, nftresponse.Hash);
                        }
                    }
                    _flightReportServices.SendBookingEmail(pid, "Booking Confirmation - " + airfare.RobinhoodID, _langCode);
                    if (mode == "app")
                    {
                        return RedirectToAction("Success", "Payment", new { bookingKeyReference = pid, product = "A", paymentMethod = 1 });
                    }
                    return RedirectToAction("Voucher", "Flight", new { id = pid });
                }
                
            }
            else
            {
                //Fail
                if (product == "A")
                {
                    _flightBookingServices.UpdatePayment(pid, 2, 0, 0, 0, DateTime.Now);
                    BL.Entities.RobinhoodFare.AirFare airfare = this._flightReportServices.GetByID(new Guid(kc.ref2));
                    _crmServices.TriggerFlight(airfare.uuid, kc.ref2, airfare.userID);
                }
                
            }
            if (mode == "app")
            {
                return RedirectToAction("Fail", "Payment", new { bookingKeyReference = pid, product = product, paymentMethod = 1 });
            }
            return RedirectToAction("SystemProblem", "Home", new { msg = "Payment Fail" });
        }
        public ActionResult Transfer(string product, Guid pid)
        {

            if (product == "A")
            {
                BL.Entities.RobinhoodFare.AirFare airfare = this._flightReportServices.GetByID(pid);
                _flightBookingServices.UpdatePayment(pid, 0, 4, 0, 0, DateTime.Now);
                _flightBookingServices.UpdateFinalPrice(pid, airfare.grandTotal);
                if (mode == "app")
                {
                    return RedirectToAction("Success", "Payment", new { bookingKeyReference = pid, product = "A", paymentMethod = 4 });
                }
                _crmServices.TriggerFlight(airfare.uuid, pid.ToString(), airfare.userID);
                //for NFT
                if (airfare.Wallet_Address != null && airfare.Wallet_Address.Length > 0)
                {
                    NFTRequest request = new NFTRequest();
                    //request.iditem = Convert.ToInt32(DateTime.Now.ToString("ddHHmmss"));
                    request.to = airfare.Wallet_Address;
                    request.metadata = String.Format("{0}Transaction/Data/?transactionID={1}", ConfigurationManager.AppSettings["Main.URL"].ToString(), airfare.bookingOID);
                    string requestJson = JsonConvert.SerializeObject(request);

                    string url = ConfigurationManager.AppSettings["NFT.Mint"].ToString();
                    string json = BL.Utilities.HttpUtility.postJSON(url, requestJson, "");
                    Log.Debug(requestJson);
                    Log.Debug(json);
                    NFTResponse response = new NFTResponse();
                    if (String.IsNullOrEmpty(json) == false)
                    {
                        json = json.Replace("@", "");

                        response = JsonConvert.DeserializeObject<NFTResponse>(json);

                        string OfficeID = ConfigurationManager.AppSettings["OfficeID"];
                        List<string> remarks = new List<string>();                        
                        remarks.Add("PAYMENT BY: BANKTRANSFER");
                        remarks.Add("NFT FEE : " + airfare.NFTFee.ToString("N2"));
                        remarks.Add("NFT Hash:"+ response.Hash);
                        string errMsg = "";
                        _flightSearchServices.RetrieveAndAddRemark(airfare.PNR, OfficeID, "AISOFT", remarks,false, ref errMsg);
                        _flightBookingServices.UpdateTransaction_Hash(pid, response.Hash);
                    }
                }

                _flightReportServices.SendBookingEmail(pid, "Booking Confirmation - " + airfare.RobinhoodID, Localize.GetLang());
                return RedirectToAction("Voucher", "Flight", new { id = pid });
            }
            if (mode == "app")
            {
                return RedirectToAction("Fail", "Payment", new { bookingKeyReference = pid, product = "A", paymentMethod = 4 });
            }
            return RedirectToAction("SystemProblem", "Home", new { msg = "Payment Fail" });
        }
        public ActionResult KbankCallBack()
        {
            Log.Debug("kbank call back");
            string chargeID = Request["objectId"];
            var kc = _kbankChargeServices.GetCharge(chargeID);
            mode = kc.mode;
            if (Request["status"] == "true")
            {
                if (kc.ref1 == "A")
                {
                    //_flightBookingServices.UpdatePayment(new Guid(kc.ref2), 1, 1);
                    if (mode == "app")
                    {
                        return RedirectToAction("Success", "Payment", new { bookingKeyReference = kc.ref2, product = "A", paymentMethod = 1 });
                    }
                    return RedirectToAction("Voucher", "Flight", new { id = kc.ref2 });
                }
                
            }
            else
            {
                if (kc.ref1 == "A")
                {
                    _flightBookingServices.UpdatePayment(new Guid(kc.ref2), 2, 0, 0, 0,DateTime.Now);
                }
                
            }
            if (mode == "app")
            {
                return RedirectToAction("Fail", "Payment", new { bookingKeyReference = kc.ref2, product = kc.ref1, paymentMethod = 1 });
            }
            return RedirectToAction("SystemProblem", "Home", new { msg = "Payment Fail" });
        }

        [HttpPost]
        public ActionResult KbankNotify(KBankNotifyResponse response)
        {
            Log.Debug("KbankNotify");
            string chargeID = response.Id;
            var kc = _kbankChargeServices.GetCharge(chargeID);
            if (response.Status == "success")
            {
                Log.Debug("success "+ kc.ref1);
                string _langCode = Localize.GetLang();
                Log.Debug("_langCode=" + _langCode);
                if (kc.ref1 == "A")
                {
                    Log.Debug("update payment");
                    _flightBookingServices.UpdatePayment(new Guid(kc.ref2), 1, 0, 0, 0, DateTime.Now);
                    Log.Debug("update status");
                    _flightBookingServices.UpdateBookingStatus(new Guid(kc.ref2), 1);
                    Log.Debug("before send mail");
                    BL.Entities.RobinhoodFare.AirFare airfare = this._flightReportServices.GetByID(new Guid(kc.ref2));
                    //for NFT
                    if (airfare.Wallet_Address != null && airfare.Wallet_Address.Length > 0)
                    {
                        NFTRequest request = new NFTRequest();
                        //request.iditem = Convert.ToInt32(DateTime.Now.ToString("ddHHmmss"));
                        request.to = airfare.Wallet_Address;
                        request.metadata = String.Format("{0}Transaction/Data/?transactionID={1}", ConfigurationManager.AppSettings["Main.URL"].ToString(), airfare.bookingOID);
                        string requestJson = JsonConvert.SerializeObject(request);

                        string url = ConfigurationManager.AppSettings["NFT.Mint"].ToString();
                        string json = BL.Utilities.HttpUtility.postJSON(url, requestJson, "");
                        Log.Debug(requestJson);
                        Log.Debug(json);
                        NFTResponse nftresponse = new NFTResponse();
                        if (String.IsNullOrEmpty(json) == false)
                        {
                            json = json.Replace("@", "");

                            nftresponse = JsonConvert.DeserializeObject<NFTResponse>(json);

                            string OfficeID = ConfigurationManager.AppSettings["OfficeID"];
                            List<string> remarks = new List<string>();
                            remarks.Add("PAYMENT BY: BANKTRANSFER");
                            remarks.Add("NFT FEE : " + airfare.NFTFee.ToString("N2"));
                            remarks.Add("NFT Hash:" + nftresponse.Hash);
                            string errMsg = "";
                            _flightSearchServices.RetrieveAndAddRemark(airfare.PNR, OfficeID, "AISOFT", remarks,false, ref errMsg);
                            _flightBookingServices.UpdateTransaction_Hash(new Guid(kc.ref2), nftresponse.Hash);
                        }
                    }
                    _crmServices.TriggerFlight(airfare.uuid, kc.ref2, airfare.userID);
                    _flightReportServices.SendBookingEmail(new Guid(kc.ref2), "Booking Confirmation - " + airfare.RobinhoodID, _langCode);
                }
                
            }
            else
            {
                Log.Debug("not success " + kc.ref1);
                if (kc.ref1 == "A")
                {
                    Log.Debug("update payment");
                    _flightBookingServices.UpdatePayment(new Guid(kc.ref2), 2, 0, 0, 0, DateTime.Now);
                    BL.Entities.RobinhoodFare.AirFare airfare = this._flightReportServices.GetByID(new Guid(kc.ref2));
                    _crmServices.TriggerFlight(airfare.uuid, kc.ref2, airfare.userID);
                }
                
            }
            return PartialView();
        }

        public ActionResult KbankQRCallBack()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult KbankQRNotify(KBankNotifyResponse response)
        {
            return PartialView();
        }
        public ActionResult PaySolutionPostBack()
        {
            return View();
        }
        public ActionResult PaySolutionReturn()
        {
            return View();
        }
        public ActionResult Success()
        {
            return View();
        }
        public ActionResult Fail()
        {
            return View();
        }
    }
}