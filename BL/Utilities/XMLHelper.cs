using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace BL.Utilities
{
    [Serializable]
    public class XMLHelper
    {
        public XMLHelper()
        {
        }

        public string GetXML(Object pObject, Type type)
        {
            try
            {
                string strXML = XMLSerializeObject(pObject, type);
                return strXML;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        private string XMLSerializeObject(Object pObject, Type type)
        {
            try
            {
                string XmlizedString = null;
                MemoryStream memoryStream = new MemoryStream();
                XmlSerializer xs = new XmlSerializer(type);
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);

                xs.Serialize(xmlTextWriter, pObject);
                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
                return XmlizedString.Substring(1).Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "");
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public T DeserializeXMLFileToObject<T>(string xml)
        {
            T returnObject = default(T);
            if (string.IsNullOrEmpty(xml)) return default(T);

            try
            {

                using (System.IO.MemoryStream ms = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(xml)))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    returnObject = (T)serializer.Deserialize(ms);
                }


            }
            catch (Exception ex)
            {
                //ExceptionLogger.WriteExceptionToConsole(ex, DateTime.Now);
            }
            return returnObject;
        }

        private string UTF8ByteArrayToString(Byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            string constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        public string SerializeObjectToXML(Object pObject, Type objectType)
        {
            try
            {
                String XmlizedString = null;
                MemoryStream memoryStream = new MemoryStream();
                XmlSerializer xs = new XmlSerializer(objectType);
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.GetEncoding("ISO-8859-1"));

                xs.Serialize(xmlTextWriter, pObject);
                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                XmlizedString = ISO88591StreamReader(memoryStream.ToArray());

                return XmlizedString;
                //return XmlizedString.Replace("<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>", "");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private String ISO88591StreamReader(Byte[] characters)
        {
            Encoding encoding = Encoding.GetEncoding("ISO-8859-1");
            String constructedString = encoding.GetString(characters);
            return (constructedString);
        }
    }
}
