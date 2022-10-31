using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace GogojiiWeb.Models
{
    public class PaypalModel
    {

        public string SUBMIT_URL = "https://www.sandbox.paypal.com/cgi-bin/webscr?cmd=_express-checkout&useraction=commit&token=";
        public string TOKEN = "";
        public string sanboxURL = "https://api-3t.sandbox.paypal.com/nvp";
        public string prodURL = "https://api-3t.paypal.com/nvp";

        public string PAYPAL_URL = "https://api-3t.sandbox.paypal.com/nvp";
        public string USER = "";
        public string PWD = "";
        public string SIGNATURE = "";
        public string PAYPALMODE = "";
        public string BNCODE = "";

        private string _frontURL = "";
        private string _pnr = "";
        private decimal _amount = 0;
        private bool _isTest;
        private string _invoiceNo = "";
        public PaypalModel(string frontURL, decimal amount, string pnr, string invoiceNo, bool isTest)
        {
            _frontURL = frontURL;
            _amount = amount;
            _pnr = pnr;
            _isTest = isTest;
            _invoiceNo = invoiceNo;
            processPaypal();
        }
        private void processPaypal()
        {
            try
            {
                bool bToken = getPaypalToken();
                if (bToken)
                {
                    if (PAYPALMODE.ToLower() == "production")
                    {
                        SUBMIT_URL = "https://www.paypal.com/cgi-bin/webscr?cmd=_express-checkout&useraction=commit&token=";

                    }
                    else
                    {
                        SUBMIT_URL = "https://www.sandbox.paypal.com/cgi-bin/webscr?cmd=_express-checkout&useraction=commit&token=";
                    }

                    SUBMIT_URL = SUBMIT_URL + TOKEN;
                    //Response.Redirect(, true);
                }
            }
            catch (Exception ex)
            {

            }
        }
        private bool getPaypalToken()
        {
            bool res = false;
            try
            {
                HttpWebRequest HttpWReq = null;
                HttpWebResponse HttpWRes = null;
                StreamReader streamRead = null;

                //string frontURL = ConfigurationManager.AppSettings["HIB.BOOKING.URL"].ToString();
                

                ConfigPaymentMode();


                string VERSION = "109.0";
                string PAYMENTREQUEST_0_PAYMENTACTION = "Sale";
                string PAYMENTREQUEST_0_AMT = "";

                PAYMENTREQUEST_0_AMT = _amount.ToString("N2").Replace(",", "");


                string RETURNURL = _frontURL + "/PaypalExpressStatusWithBNCode";
                string CANCELURL = _frontURL + "/PayPalCancel";
                string METHOD = "SetExpressCheckout";
                string PAYMENTREQUEST_0_CURRENCYCODE = "THB";
                string L_PAYMENTREQUEST_0_NAME0 = _invoiceNo;
                string L_PAYMENTREQUEST_0_QTY0 = "1";
                string L_PAYMENTREQUEST_0_AMT0 = PAYMENTREQUEST_0_AMT;
                string PAYMENTREQUEST_0_ITEMAMT = PAYMENTREQUEST_0_AMT;
                string BRANDNAME = "Gogojii";
                
                string PAYMENTREQUEST_0_DESC = Truncate("BOOKING REF - " + _pnr, 124);

                string sparam = String.Format("USER={0}&PWD={1}&SIGNATURE={2}&VERSION={3}&PAYMENTREQUEST_0_PAYMENTACTION={4}&PAYMENTREQUEST_0_AMT={5}&RETURNURL={6}&CANCELURL={7}&METHOD={8}&PAYMENTREQUEST_0_CURRENCYCODE={9}&L_PAYMENTREQUEST_0_NAME0={10}&L_PAYMENTREQUEST_0_QTY0={11}&L_PAYMENTREQUEST_0_AMT0={12}&PAYMENTREQUEST_0_ITEMAMT={13}&BRANDNAME={14}&PAYMENTREQUEST_0_DESC={15}&NOSHIPPING=1&ADDROVERRIDE=0&SOLUTIONTYPE=Sole&LANDINGPAGE=Billing",
                    USER, PWD, SIGNATURE, VERSION, PAYMENTREQUEST_0_PAYMENTACTION, PAYMENTREQUEST_0_AMT, RETURNURL, CANCELURL, METHOD, PAYMENTREQUEST_0_CURRENCYCODE, L_PAYMENTREQUEST_0_NAME0, L_PAYMENTREQUEST_0_QTY0, L_PAYMENTREQUEST_0_AMT0, PAYMENTREQUEST_0_ITEMAMT, BRANDNAME, PAYMENTREQUEST_0_DESC);



                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                Byte[] bytesData = encoding.GetBytes(sparam);

                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls; // comparable to modern browsers


                HttpWReq = (HttpWebRequest)WebRequest.Create(PAYPAL_URL);
                HttpWReq.ContentType = "text/xml";
                HttpWReq.ContentLength = sparam.Length;
                HttpWReq.Method = "POST";
                HttpWReq.Timeout = 200000;

                System.IO.Stream StreamData = HttpWReq.GetRequestStream();
                StreamData.Write(bytesData, 0, bytesData.Length);
                StreamData.Close();

                IAsyncResult asyncResult = (IAsyncResult)HttpWReq.BeginGetResponse(null, null);
                asyncResult.AsyncWaitHandle.WaitOne();
                HttpWRes = (HttpWebResponse)HttpWReq.EndGetResponse(asyncResult);

                Stream responseStream = responseStream = HttpWRes.GetResponseStream();
                StreamReader Reader = new StreamReader(responseStream, Encoding.Default);
                string sResponse = Reader.ReadToEnd();


                if (sResponse != null && sResponse != "")
                {
                    sResponse = System.Web.HttpUtility.UrlDecode(sResponse);

                    string[] arrParam = sResponse.Split('&');
                    for (int i = 0; i < arrParam.Length; i++)
                    {
                        if (arrParam[i].IndexOf("TOKEN") != -1)
                        {
                            string[] arrToken = arrParam[i].Split('=');
                            TOKEN = arrToken[1];
                            //Session.Add("ssPaypalToken_" + booking.AgentCode, TOKEN);
                            res = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return res;
        }

        public string Truncate(string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
        }

        private void ConfigPaymentMode()
        {

            if (_isTest)
            {
                PAYPALMODE = "SANDBOX";
                USER = "contact-facilitator_api1.asia1click.com";
                PWD = "VN6WW6L46H6L3DCY";
                SIGNATURE = "AHt1zhqIhOnDbLEsIeNuqeqQW4ZoAEgWxFGFGPNOLvlJ0-SNqNtOetD1";
                BNCODE = "";
            }
            else
            {
                PAYPALMODE = "production";
                USER = "contact_api1.asia1click.com";
                PWD = "C3LJ3676GLT78E6A";
                SIGNATURE = "A1YnOitfUHG5uGJgyVbnIdZ28IXEA3pCgb6WujVa--oq3ZG949G6FhaD";
                BNCODE = "";
            }


            if (PAYPALMODE.ToLower() == "production")
            {
                PAYPAL_URL = prodURL;

            }
            else
            {
                PAYPAL_URL = sanboxURL;

            }


        }

        private string Encrypt(string stringToEncrypt)
        {
            byte[] inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
            byte[] rgbIV = { 0x21, 0x43, 0x56, 0x87, 0x10, 0xfd, 0xea, 0x1c };
            byte[] key = { };
            try
            {
                key = System.Text.Encoding.UTF8.GetBytes("A0D1nX0Q");
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, rgbIV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private string Decrypt(string EncryptedText)
        {
            byte[] inputByteArray = new byte[EncryptedText.Length + 1];
            byte[] rgbIV = { 0x21, 0x43, 0x56, 0x87, 0x10, 0xfd, 0xea, 0x1c };
            byte[] key = { };

            try
            {
                key = System.Text.Encoding.UTF8.GetBytes("A0D1nX0Q");
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(EncryptedText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, rgbIV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                return encoding.GetString(ms.ToArray());
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public bool checkResponseFromTlsServer(string tlsResponse, NameValueCollection coll)
        {
            string token = "", PayerID = "";
            string[] arr1 = coll.AllKeys;
            for (int loop1 = 0; loop1 < arr1.Length; loop1++)
            {
                string[] arr2 = coll.GetValues(arr1[loop1]);
                for (int loop2 = 0; loop2 < arr2.Length; loop2++)
                {
                    if (arr1[loop1] == "PayerID")
                    {
                        PayerID = arr2[loop2];
                    }
                    if (arr1[loop1] == "token")
                    {
                        token = arr2[loop2];
                    }
                }
            }

            if (token == "")//|| PayerID == ""
            {
                return false;
            }

            string sResponse = Decrypt(HttpUtility.UrlDecode(tlsResponse).Replace(" ", "+"));

            bool res = false;
            string rsTOKEN = "";
            string rsACK = "";
            try
            {
                if (sResponse != null && sResponse != "")
                {
                    sResponse = System.Web.HttpUtility.UrlDecode(sResponse);
                    string[] arrParam = sResponse.Split('&');
                    for (int i = 0; i < arrParam.Length; i++)
                    {
                        if (arrParam[i].IndexOf("ACK") != -1)
                        {
                            string[] arrACK = arrParam[i].Split('=');
                            rsACK = arrACK[1];
                        }
                        if (arrParam[i].IndexOf("TOKEN") != -1)
                        {
                            string[] arrToken = arrParam[i].Split('=');
                            rsTOKEN = arrToken[1];
                        }
                    }

                    if (rsACK == "Success" && rsTOKEN == token)
                    {
                        res = true;
                    }
                    else
                    {
                    }
                    return res;

                }
            }
            catch (Exception ex)
            {
            }
            return res;
        }

        public bool doPaypalPayment(NameValueCollection coll)
        {
            string token = "", PayerID = "";
            string[] arr1 = coll.AllKeys;
            for (int loop1 = 0; loop1 < arr1.Length; loop1++)
            {
                string[] arr2 = coll.GetValues(arr1[loop1]);
                for (int loop2 = 0; loop2 < arr2.Length; loop2++)
                {
                    if (arr1[loop1] == "PayerID")
                    {
                        PayerID = arr2[loop2];
                    }
                    if (arr1[loop1] == "token")
                    {
                        token = arr2[loop2];
                    }
                }
            }

            if (token == "")//|| PayerID == ""
            {
                return false;
            }

            bool res = false;
            try
            {
                HttpWebRequest HttpWReq = null;
                HttpWebResponse HttpWRes = null;
                StreamReader streamRead = null;

                string sanboxURL = "https://api-3t.sandbox.paypal.com/nvp";
                string prodURL = "https://api-3t.paypal.com/nvp";

                string PAYPAL_URL = "https://api-3t.sandbox.paypal.com/nvp";
                ConfigPaymentMode();
                if (PAYPALMODE.ToLower() == "production")
                {
                    PAYPAL_URL = prodURL;

                }
                else
                {
                    PAYPAL_URL = sanboxURL;

                }


                string VERSION = "109.0";
                string PAYMENTREQUEST_0_PAYMENTACTION = "Sale";
                string PAYMENTREQUEST_0_AMT = _amount.ToString("N2").Replace(",", "");
                string PAYMENTREQUEST_0_ITEMAMT = PAYMENTREQUEST_0_AMT;
                string L_PAYMENTREQUEST_0_NAME0 = _pnr;
                string L_PAYMENTREQUEST_0_QTY0 = "1";
                string L_PAYMENTREQUEST_0_AMT0 = PAYMENTREQUEST_0_AMT;
                string METHOD = "DoExpressCheckoutPayment";
                string PAYERID = PayerID;
                string TOKEN = token;

                string PAYMENTREQUEST_0_CURRENCYCODE = "THB";

                string sparam = String.Format("USER={0}&PWD={1}&SIGNATURE={2}&VERSION={3}&PAYMENTREQUEST_0_PAYMENTACTION={4}&PAYMENTREQUEST_0_AMT={5}&PAYERID={6}&TOKEN={7}&METHOD={8}&PAYMENTREQUEST_0_CURRENCYCODE={9}",
                    USER, PWD, SIGNATURE, VERSION, PAYMENTREQUEST_0_PAYMENTACTION, PAYMENTREQUEST_0_AMT, PAYERID, TOKEN, METHOD, PAYMENTREQUEST_0_CURRENCYCODE, L_PAYMENTREQUEST_0_NAME0, L_PAYMENTREQUEST_0_QTY0, L_PAYMENTREQUEST_0_AMT0, PAYMENTREQUEST_0_ITEMAMT);
                if (BNCODE.Length > 0)
                {
                    sparam = String.Format("{0}&BUTTONSOURCE={1}", sparam, BNCODE);
                }

                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                Byte[] bytesData = encoding.GetBytes(sparam);

                HttpWReq = (HttpWebRequest)WebRequest.Create(PAYPAL_URL);
                HttpWReq.ContentType = "text/xml";
                HttpWReq.ContentLength = sparam.Length;
                HttpWReq.Method = "POST";
                HttpWReq.Timeout = 200000;

                System.IO.Stream StreamData = HttpWReq.GetRequestStream();
                StreamData.Write(bytesData, 0, bytesData.Length);
                StreamData.Close();

                IAsyncResult asyncResult = (IAsyncResult)HttpWReq.BeginGetResponse(null, null);
                asyncResult.AsyncWaitHandle.WaitOne();
                HttpWRes = (HttpWebResponse)HttpWReq.EndGetResponse(asyncResult);

                Stream responseStream = responseStream = HttpWRes.GetResponseStream();
                StreamReader Reader = new StreamReader(responseStream, Encoding.Default);
                string sResponse = Reader.ReadToEnd();

                string rsTOKEN = "";
                string rsACK = "";
                if (sResponse != null && sResponse != "")
                {
                    sResponse = System.Web.HttpUtility.UrlDecode(sResponse);
                    string[] arrParam = sResponse.Split('&');
                    for (int i = 0; i < arrParam.Length; i++)
                    {
                        if (arrParam[i].IndexOf("ACK") != -1)
                        {
                            string[] arrACK = arrParam[i].Split('=');
                            rsACK = arrACK[1];
                        }
                        if (arrParam[i].IndexOf("TOKEN") != -1)
                        {
                            string[] arrToken = arrParam[i].Split('=');
                            rsTOKEN = arrToken[1];
                        }
                    }

                    if (rsACK == "Success" && rsTOKEN == TOKEN)
                    {
                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return res;
        }
    }
}




/*
 NVP/SOAP Sandbox API Credentials
Username:
contact-facilitator_api1.asia1click.com
Password:
VN6WW6L46H6L3DCY
Signature:
AHt1zhqIhOnDbLEsIeNuqeqQW4ZoAEgWxFGFGPNOLvlJ0-SNqNtOetD1

    =========================================
    ชื่อผู้ใช้ API
contact_api1.asia1click.com
ซ่อน
รหัสผ่าน API
C3LJ3676GLT78E6A
ซ่อน
ลายเซ็น
A1YnOitfUHG5uGJgyVbnIdZ28IXEA3pCgb6WujVa--oq3ZG949G6FhaD
ซ่อน
     */
