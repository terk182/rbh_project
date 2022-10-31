using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BL.Utilities
{
    public class EmailUtilities
    {
        private static readonly ILog Log =
                       LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static void sendMail(string to, string subject, string body, string cc, string bcc)
        {
            //var sClient = new SmtpClient("smtp.office365.com");
            //var sClient = new SmtpClient("smtp.office365.com");
            var message = new MailMessage();
           // sClient.Port = 587;
            //sClient.EnableSsl = true;
            //sClient.UseDefaultCredentials = false;
            //sClient.Credentials = new NetworkCredential("rsvn@gogojii.com", "Gon02ep!y");
           
            message.Body = body;
            message.IsBodyHtml = true;
            message.From = new MailAddress(ConfigurationManager.AppSettings["Mail.FROM"].ToString()); //new MailAddress("rsvn@gogojii.com");
            message.Subject = subject;
            string[] toMails = to.Replace(",", ";").Replace(" ", "").Split(';');
            foreach (var t in toMails)
            {
                Log.Debug("to:" + t);
                message.To.Add(new MailAddress(t));
            }

            if (!String.IsNullOrEmpty(cc))
            {
                string[] ccMails = cc.Replace(",", ";").Replace(" ", "").Split(';');
                foreach (var c in ccMails)
                {
                    message.CC.Add(new MailAddress(c));
                }
            }
            
            string bcc_aisoft= ConfigurationManager.AppSettings["Mail.BCCAISOFT"]!=null?ConfigurationManager.AppSettings["Mail.BCCAISOFT"].ToString():"";
            if (!String.IsNullOrEmpty(bcc))
            {
                bcc += ";" + bcc_aisoft;
            }
            else
            {
                bcc = bcc_aisoft;
            }
            if (!String.IsNullOrEmpty(bcc))
            {
                string[] bccMails = bcc.Replace(",", ";").Replace(" ", "").Split(';');
                foreach (var b in bccMails)
                {
                    message.Bcc.Add(new MailAddress(b));
                }
            }

            try
            {
                Log.Debug("before send");
                var sClient = new SmtpClient();
                sClient.Send(message);
                Log.Debug("after send");
            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }
        }
        public static void sendMail(string to, string subject, string body, string cc, string bcc, Attachment attachFile)
        {
            //var sClient = new SmtpClient("smtp.office365.com");
            //var sClient = new SmtpClient("smtp.office365.com");
            var message = new MailMessage();
            // sClient.Port = 587;
            //sClient.EnableSsl = true;
            //sClient.UseDefaultCredentials = false;
            //sClient.Credentials = new NetworkCredential("rsvn@gogojii.com", "Gon02ep!y");

            message.Body = body;
            message.IsBodyHtml = true;
            message.From = new MailAddress(ConfigurationManager.AppSettings["Mail.FROM"].ToString()); //new MailAddress("rsvn@gogojii.com");
            message.Subject = subject;
            string[] toMails = to.Replace(",", ";").Replace(" ", "").Split(';');
            foreach (var t in toMails)
            {
                Log.Debug("to:" + t);
                message.To.Add(new MailAddress(t));
            }

            if (!String.IsNullOrEmpty(cc))
            {
                string[] ccMails = cc.Replace(",", ";").Replace(" ", "").Split(';');
                foreach (var c in ccMails)
                {
                    message.CC.Add(new MailAddress(c));
                }
            }

            string bcc_aisoft = ConfigurationManager.AppSettings["Mail.BCCAISOFT"] != null ? ConfigurationManager.AppSettings["Mail.BCCAISOFT"].ToString() : "";
            if (!String.IsNullOrEmpty(bcc))
            {
                bcc += ";" + bcc_aisoft;
            }
            else
            {
                bcc = bcc_aisoft;
            }
            if (!String.IsNullOrEmpty(bcc))
            {
                string[] bccMails = bcc.Replace(",", ";").Replace(" ", "").Split(';');
                foreach (var b in bccMails)
                {
                    message.Bcc.Add(new MailAddress(b));
                }
            }

            try
            {
                if (attachFile != null)
                {
                    message.Attachments.Add(attachFile);
                }
                Log.Debug("before send");
                var sClient = new SmtpClient();
                sClient.Send(message);
                Log.Debug("after send");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
        public static bool SendMailReturnStatus(string to, string subject, string body, string cc, string bcc)
        {
            //var sClient = new SmtpClient("smtp.office365.com");
            //var sClient = new SmtpClient("smtp.office365.com");
            var message = new MailMessage();
            // sClient.Port = 587;
            //sClient.EnableSsl = true;
            //sClient.UseDefaultCredentials = false;
            //sClient.Credentials = new NetworkCredential("rsvn@gogojii.com", "Gon02ep!y");

            message.Body = body;
            message.IsBodyHtml = true;
            message.From = new MailAddress(ConfigurationManager.AppSettings["Mail.FROM"].ToString()); //MailAddress("rsvn@gogojii.com");
            message.Subject = subject;
            string[] toMails = to.Replace(",", ";").Replace(" ", "").Split(';');
            foreach (var t in toMails)
            {
                Log.Debug("to:" + t);
                message.To.Add(new MailAddress(t));
            }

            if (!String.IsNullOrEmpty(cc))
            {
                string[] ccMails = cc.Replace(",", ";").Replace(" ", "").Split(';');
                foreach (var c in ccMails)
                {
                    message.CC.Add(new MailAddress(c));
                }
            }
            string bcc_aisoft = ConfigurationManager.AppSettings["Mail.BCCAISOFT"] != null ? ConfigurationManager.AppSettings["Mail.BCCAISOFT"].ToString() : "";
            if (!String.IsNullOrEmpty(bcc))
            {
                bcc += ";" + bcc_aisoft;
            }
            else
            {
                bcc = bcc_aisoft;
            }
            if (!String.IsNullOrEmpty(bcc))
            {
                string[] bccMails = bcc.Replace(",", ";").Replace(" ", "").Split(';');
                foreach (var b in bccMails)
                {
                    message.Bcc.Add(new MailAddress(b));
                }
            }
            try
            {
                Log.Debug("before send");
                var sClient = new SmtpClient();
                sClient.Send(message);                
                Log.Debug("after send");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }
        public static bool SendMailReturnStatus(string to, string subject, string body, string cc, string bcc, Attachment attachFile)
        {
            //var sClient = new SmtpClient("smtp.office365.com");
            //var sClient = new SmtpClient("smtp.office365.com");
            var message = new MailMessage();
            // sClient.Port = 587;
            //sClient.EnableSsl = true;
            //sClient.UseDefaultCredentials = false;
            //sClient.Credentials = new NetworkCredential("rsvn@gogojii.com", "Gon02ep!y");

            message.Body = body;
            message.IsBodyHtml = true;
            message.From = new MailAddress(ConfigurationManager.AppSettings["Mail.FROM"].ToString()); //new MailAddress("rsvn@gogojii.com");
            message.Subject = subject;
            string[] toMails = to.Replace(",", ";").Replace(" ", "").Split(';');
            foreach (var t in toMails)
            {
                Log.Debug("to:" + t);
                message.To.Add(new MailAddress(t));
            }

            if (!String.IsNullOrEmpty(cc))
            {
                string[] ccMails = cc.Replace(",", ";").Replace(" ", "").Split(';');
                foreach (var c in ccMails)
                {
                    message.CC.Add(new MailAddress(c));
                }
            }
            string bcc_aisoft = ConfigurationManager.AppSettings["Mail.BCCAISOFT"] != null ? ConfigurationManager.AppSettings["Mail.BCCAISOFT"].ToString() : "";
            if (!String.IsNullOrEmpty(bcc))
            {
                bcc += ";" + bcc_aisoft;
            }
            else
            {
                bcc = bcc_aisoft;
            }
            if (!String.IsNullOrEmpty(bcc))
            {
                string[] bccMails = bcc.Replace(",", ";").Replace(" ", "").Split(';');
                foreach (var b in bccMails)
                {
                    message.Bcc.Add(new MailAddress(b));
                }
            }
            try
            {
                if (attachFile != null)
                {
                    message.Attachments.Add(attachFile);
                }
                Log.Debug("before send");
                var sClient = new SmtpClient();
                sClient.Send(message);
                Log.Debug("after send");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        public static string captureHtml(string url)
        {
            Log.Debug("url:"+ url);
            //ตัวอนุญาตให้ใช้ Https
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

            request.ContentType = "text/html";
            request.Method = "GET";
            request.Proxy.Credentials = CredentialCache.DefaultCredentials;
            string CaptureValue = string.Empty;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    CaptureValue = sr.ReadToEnd();
                    // Close and clean up the StreamReader
                    sr.Close();
                }
            }
            else
            {
                CaptureValue = "";
            }

            return CaptureValue;
        }
    }
}
