using System;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using xFaceLib.Log;

namespace xFaceLib.runtime
{
    public class XAppConfigParserFactory
    {
        public static XAppConfigParser CreateAppConfigParser(string appXml)
        {
            string schemaValue = GetSchemaValue(appXml);

            if (null == schemaValue)
            {
                return new XAppConfigParserW3C(appXml);
            }
            else if (schemaValue.Equals("1.0"))
            {
                return new XAppConfigParserSchema1_0(appXml);
            }
            else
            {
                //FIXME:存在其他版本的app.xml
                return null;
            }
        }

        public static XAppConfigParser CreateAppConfigParser(Stream xmlStream)
        {
            XDocument doc = null;
            try
            {
                doc = XDocument.Load(xmlStream);
            }
            catch (XmlException ex)
            {
                XLog.WriteError("can't parse app.xml stream with exception!" + ex.Message);
                return null;
            }
            return CreateAppConfigParser(doc.ToString());
        }

        private static string GetSchemaValue(string appXml)
        {
            XDocument doc = null;
            try
            {
                doc = XDocument.Parse(appXml);
            }
            catch (XmlException ex)
            {
                XLog.WriteError("can't parse app.xml with exception!" + ex.Message);
                //FIXME： 解析不了的app.xml 返回一个错误的 schema 值
                return "error";
            }
            if (null == doc.Root.Attribute("schema"))
            {
                return null;
            }
            return doc.Root.Attribute("schema").Value;
        }
    }
}
