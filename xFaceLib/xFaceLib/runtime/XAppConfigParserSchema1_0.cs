using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System.Xml.Linq;
using System.Diagnostics;

namespace xFaceLib.runtime
{
    public class XAppConfigParserSchema1_0 : XAppConfigParser
    {

        public XAppConfigParserSchema1_0(string appXml)
            : base(appXml)
        { }

        public override XAppInfo parseConfig()
        {
            base.parseConfig();
            parseDistributionTag();
            parseComponentsTag();
            parseAccessTag();
            return this.appInfo;
        }

        /// <summary>
        /// 解析app元素标签，以及子节点的属性和值
        /// </summary>
        protected override void parseAppTag()
        {
            XElement appElement = getAppElement();
            //id
            appInfo.AppId = getElementAttributeValue(appElement, "id");
        }

        /// <summary>
        /// 解析description元素标签，以及子节点的属性和值
        /// </summary>
        protected override void parseDescriptionTag()
        {
            XElement appElement = getAppElement();
            XElement descriptionElement = getChildElement(appElement ,"description");

            //type
            XElement typeElement = getChildElement(descriptionElement ,"type");
            appInfo.Type = getElementValue(typeElement);

            //running_mode
            XElement running_modeElement = getChildElement(descriptionElement ,"running_mode");
            appInfo.RunningMode = getElementAttributeValue(running_modeElement, "value");

            //entry
            XElement entryElement =getChildElement(descriptionElement ,"entry");
            appInfo.Entry = getElementAttributeValue(entryElement, "src");

            //icon
            XElement iconElement = getChildElement(descriptionElement ,"icon");
            appInfo.Icon = getElementAttributeValue(iconElement, "src");
            appInfo.IconBgColor = getElementAttributeValue(iconElement, "background-color");

            //version
            XElement versionElement = getChildElement(descriptionElement ,"version");
            appInfo.Version = getElementValue(versionElement);

            //name
            XElement nameElement = getChildElement(descriptionElement ,"name");
            appInfo.Name = getElementValue(nameElement);

            //display
            parseDisplayElement(getChildElement(descriptionElement ,"display"));

            //runtime
            XElement runtimeElement =getChildElement(descriptionElement ,"runtime");
            appInfo.EngineType = getElementValue(runtimeElement);

            //copyright
            parseCopyRightElement(getChildElement(descriptionElement ,"copyright"));

            XElement remoteElement = getChildElement(descriptionElement, "remote-pkg");
            appInfo.PrefRemotePkg = getElementValue(remoteElement);
        }

        /// <summary>
        /// 解析distribution元素标签，以及子节点的属性和值
        /// </summary>
        protected void parseDistributionTag()
        {
            XElement appElement = getAppElement();
            XElement distributionElement =getChildElement(appElement ,"distribution");

            //package
            parsePackageElement(getChildElement(distributionElement, "package"));
            //channel
            parseChannelElement(getChildElement(distributionElement, "channel"));
        }

        /// <summary>
        /// 解析display元素标签
        /// </summary>
        protected void parseDisplayElement(XElement displayElement)
        {

            appInfo.DisplayMode = getElementAttributeValue(displayElement, "type");
            XElement widthElemet = getChildElement(displayElement, "width");
            string width = getElementValue(widthElemet);
            if (null == width)
            {
                appInfo.Width = 0;
            }
            else
            {
                appInfo.Width = int.Parse(width);
            }

            XElement heightElement = getChildElement(displayElement, "height");
            string height = getElementValue(heightElement);
            if (null == height)
            {
                appInfo.Height = 0;
            }
            else
            {
                appInfo.Height = int.Parse(height);
            }
        }

        /// <summary>
        /// 解析copyRight元素标签
        /// </summary>
        protected void parseCopyRightElement(XElement copyRightElement)
        {
            // FIXME:由于author,license还未使用，相应的配置信息暂时不解析
        }

        /// <summary>
        /// 解析package元素标签
        /// </summary>
        protected void parsePackageElement(XElement packageElement)
        {
        //    <package>
        //        <singlefile>false</singlefile>
        //        <encrypt>false</encrypt>
        //    </package>
            XElement singlefileElement = getChildElement(packageElement, "singlefile");
            string singlefile_value = getElementValue(singlefileElement);
            appInfo.IsSingleFileUsed = Convert.ToBoolean(singlefile_value);

            XElement encryptElement = getChildElement(packageElement, "encrypt");
            string encrypt_value = getElementValue(encryptElement);
            appInfo.IsEncrypted = Convert.ToBoolean(encrypt_value);
        }

        /// <summary>
        /// 解析channel元素标签
        /// </summary>
        protected void parseChannelElement(XElement channelElement)
        {
            //<channel id="cupmp_1000">
            //    <name>中联正式渠道</name>
            //</channel>
            appInfo.ChannelId = getElementAttributeValue(channelElement, "id");
            appInfo.ChannelName = getElementValue(channelElement);
        }

        /// <summary>
        /// 解析组件 components 标签
        /// </summary>
        protected void parseComponentsTag()
        {
            XElement appElement = getAppElement();

            XElement componentsElement = getChildElement(appElement, "components");
            parseComponentsElement(componentsElement);
        }

        /// <summary>
        /// 解析组件 component 标签
        /// </summary>
        /// <param name="componentsElement"></param>
        protected void parseComponentsElement(XElement componentsElement)
        { 
            //<components>
            //    <component version="2.0.3" isUpdated="true">calendar</component>
            //    ...
            //</components>

            //TODO 以后加入组件信息配置 解析app 配置的组件信息列表
            XElement componentElement = getChildElement(componentsElement, "component");
            string version = getElementAttributeValue(componentElement, "version");
            string isUpdated = getElementAttributeValue(componentElement, "isUpdated");
            string componentName = getElementValue(componentElement);  
        }

        /// <summary>
        /// 获取app节点的XElement对象
        /// </summary>
        /// <returns>返回app节点 XElement对象</returns>
        protected XElement getAppElement()
        {
            XElement root = this.doc.Root;
            return getChildElement(root, "app");
        }

        protected void parseAccessTag()
        {
            //<access origin="http://google.com" />
            //<access origin="http://*.google.com" />

            XElement appElement = getAppElement();
            var accessList = from results in appElement.Descendants("access")
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
