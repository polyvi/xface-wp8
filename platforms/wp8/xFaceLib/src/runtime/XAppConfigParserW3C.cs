using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using xFaceLib.runtime;
using xFaceLib.Log;

namespace xFaceLib.runtime
{
    //解析符合 http://www.w3.org/ns/widgets 规范的app.xml
    /*
    <?xml version='1.0' encoding='UTF-8'?>
	<widget id="xapp" version="2.0" xmlns="http://www.w3.org/ns/widgets">
		<name short="myApp">myApp</name>
		<icon src="icon.png" />
		<content encoding="UTF-8" src="index.html" />
		<preference name="type" readonly="true" value="xapp" />
		<preference name="mode" readonly="true" value="local" />
		<preference name="engine" readonly="true" value="3.1.0" />
		<description>
			A sample widget to demonstrate some of the possibilities.
		</description>
        <access origin="http://*.google.com" />
		<author email="foo-bar@polyvi.com/" href="http://polyvi.com/">polyvi</author>

		<license> Copyright 2012-2013, Polyvi Inc. </license>

	</widget>
     */
    public class XAppConfigParserW3C : XAppConfigParser
    {
        public XAppConfigParserW3C(string appXml)
            : base(appXml)
        { }

        public override XAppInfo parseConfig()
        {
            ParseAllTag();
            if (false == checkTagVerify())
            {
                return null;
            }
            return this.appInfo;
        }

        /// <summary>
        /// 解析含有namespace的xml;获取XElement 需要拼接 namespace
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected string Name(string name)
        {
            if ( string.IsNullOrEmpty(doc.Root.GetDefaultNamespace().ToString()) )
            {
                return name;
            }
            return "{" + doc.Root.GetDefaultNamespace().ToString() + "}" + name;
        }

        private void ParseAllTag()
        {
            //widget
            XElement widgetElement = this.doc.Root;

            if (null == widgetElement || !widgetElement.Name.LocalName.Equals("widget"))
            {
                XLog.WriteError("Invaild app.xml!");
                return;
            }

            this.appInfo.AppId = getElementAttributeValue(widgetElement, "id");
            this.appInfo.Version = getElementAttributeValue(widgetElement, "version");

            //name
            XElement nameElement = getChildElement(widgetElement, Name("name"));
            this.appInfo.Name = getElementValue(nameElement);

            //content
            XElement entryElement = getChildElement(widgetElement, Name("content"));
            this.appInfo.Entry = getElementAttributeValue(entryElement, "src");

            //icon
            XElement iconElement = getChildElement(widgetElement,Name("icon"));
            this.appInfo.Icon = getElementAttributeValue(iconElement,"src");
            this.appInfo.IconBgColor = getElementAttributeValue(iconElement, "background-color");

            //type \ mode \ engine
            this.appInfo.Type = ParsePrefValue(widgetElement, "type");
            this.appInfo.RunningMode = ParsePrefValue(widgetElement, "mode");
            this.appInfo.EngineVersion = ParsePrefValue(widgetElement, "engine");
            this.appInfo.PrefRemotePkg = ParsePrefValue(widgetElement, "remote-pkg");

            //access
            parseAccessTag();

            //description

            // FIXME:由于author,license还未使用，相应的配置信息暂时不解析
            //author

            //license

        }

        /// <summary>
        /// 解析widgetElement 标签中preference元素对应name的value
        /// </summary>
        /// <param name="widgetElement"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private String ParsePrefValue(XElement widgetElement, String name)
        {
            foreach (XElement itemnode in widgetElement.Descendants( Name("preference") ))
            {
                string attrName = itemnode.Attribute("name").Value;
                if (attrName.Equals(name))
                {
                    return itemnode.Attribute("value").Value;
                }
            }
            return null;
        }

        private void parseAccessTag()
        {
            //<access origin="http://google.com" />
            //<access origin="http://*.google.com" />

            var accessList = from results in this.doc.Root.Descendants("access")
                             select new
                             {
                                 origin = (string)results.Attribute("origin"),
                             };
            appInfo.AccessDomains = new List<string>();
            foreach (var accessElem in accessList)
            {
                appInfo.AccessDomains.Add(accessElem.origin);
            }
        }
    }
}
