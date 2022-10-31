using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.LionAir_CreateSession
{
    public class CreateSessionReq
    {       
        public string GetCreateSessionRequest()
        {
            DateTime dtNow = DateTime.Now;
            string sMessage = "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">";
            sMessage += "<soap:Header>";
            sMessage += "<MessageHeader xmlns=\"http://www.ebxml.org/namespaces/messageHeader\">";
            sMessage += "<CPAId>SL</CPAId>";
            sMessage += "<ConversationId>"+Guid.NewGuid()+"</ConversationId>";
            sMessage += "<Service>Create</Service>";
            sMessage += "<Action>CreateSession</Action>";
            sMessage += "<MessageData>";
            sMessage += "<MessageId>mid:" + dtNow.ToString("HH:mm:ss.ffff")+"</MessageId>";
            sMessage += "<Timestamp>"+ dtNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss") +"</Timestamp>";
            sMessage += "</MessageData>";
            sMessage += "</MessageHeader>";
            sMessage += "<Security xmlns=\"http://schemas.xmlsoap.org/ws/2002/12/secext\">";
            sMessage += "<UsernameToken>";
            sMessage += "<Username>"+ ConfigurationManager.AppSettings["LionAir.Username"].ToString() + "</Username>";//
            sMessage += "<Password>" + ConfigurationManager.AppSettings["LionAir.Password"].ToString() + "</Password>";//
            sMessage += "<Organization xmlns=\"\">SL</Organization>";
            sMessage += "</UsernameToken>";
            sMessage += "<BinarySecurityToken/>";
            sMessage += "</Security>";
            sMessage += "</soap:Header>";
            sMessage += "<soap:Body>";
            sMessage += "<Logon xmlns=\"http://www.vedaleon.com/webservices\"/>";
            sMessage += "</soap:Body>";
            sMessage += "</soap:Envelope>";
            return sMessage;
        }
    }
}
