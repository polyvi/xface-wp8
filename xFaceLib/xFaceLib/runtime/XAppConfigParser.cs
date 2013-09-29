using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace xFaceLib.runtime
{
    public class XAppConfigParser
    {
        /// <summary>
        /// 记录应用配置信息
        /// </summary>
        protected XAppInfo appInfo;


        //保存了 app.xml 解析内容的XDocument对象
        protected XDocument doc;

        /// <summary>
        /// app.xml解析器的构造方法
        /// </summary>
        /// <param name="appXml">待解析的app.xml内容</param>
        public XAppConfigParser(string appXml)
        {
            this.doc = XDocument.Parse(appXml);
            this.appInfo = new XAppInfo();
        }

        /// <summary>
        /// 解析app.xml的公共入口
        /// </summary>
        /// <returns>解析完成且appId存在返回appInfo对象，否则返回null</returns>
        public virtual XAppInfo parseConfig()
        {
            parseAppTag();
            parseDescriptionTag();

            if (false == checkTagVerify())
            {
                return null;
            }
            return this.appInfo;
        }

        /// <summary>
        /// 由子类重写 解析<app></app>标签
        /// </summary>
        protected virtual void parseAppTag()
        {}

        /// <summary>
        /// 由子类重写 解析<description></description>标签
        /// </summary>
        protected virtual void parseDescriptionTag()
        {}

        /// <summary>
        /// 获取标签对节点的孩子节点
        /// </summary>
        /// <param name="parent">父节点</param>
        /// <param name="childName">孩子节点的名字</param>
        /// <returns>父节点和子节点存在返回子节点XElement对象，否则返回null</returns>
        protected XElement getChildElement(XElement parent, string childName)
        {
            if (null == parent)
            {
                return null;
            }
            return parent.Element(childName);
        }

        /// <summary>
        /// 获取标签对节点的值
        /// 如<tag>value</tag> 返回value
        /// </summary>
        /// <param name="element">带获取标签对节点值的 XElement对象</param>
        /// <returns>XElement对象非空返回其值，否则返回null</returns>
        protected string getElementValue(XElement element)
        {
            if(null == element)
            {
                return null;
            }
            return element.Value;
        }

        /// <summary>
        /// 获取标签对节点的属性值
        /// 如<tag>attribute = value</tag> 返回value
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        protected string getElementAttributeValue(XElement element, string attribute)
        {
            if(null == element)
            {
                return null;
            }

            if (null == element.Attribute(attribute))
            {
                return null;
            }

            return element.Attribute(attribute).Value;
        }

        /// <summary>
        /// 检查解析的appxml关键值是否有效
        /// </summary>
        /// <returns>appId 非空返回true,否则返回false</returns>
        protected bool checkTagVerify()
        {
            if (null == this.appInfo.AppId || string.IsNullOrEmpty(this.appInfo.Entry))
            {
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    System.Windows.MessageBox.Show("Failed to get app config from app.xml, please set app id and content properly!");

                });
                return false;
            }
            return true;
        }
    }
}
