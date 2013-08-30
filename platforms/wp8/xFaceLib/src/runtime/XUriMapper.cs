using System;
using System.Windows.Navigation;
using xFaceLib.Log;
using System.Xml.Linq;

namespace xFaceLib.runtime
{
    public class XUriMapper :UriMapperBase
    {
        public override Uri MapUri(Uri uri)
        {
            string tempUri = System.Net.HttpUtility.UrlDecode(uri.ToString());

            //验证Uri，是否为已注册的关联,
            //uri格式如："/Protocol?encodedLaunchUri=xface://?startpage=index.html;data=..."
            if (tempUri.StartsWith("/Protocol?"))
            {
                XElement appElement = XDocument.Load("WMAppManifest.xml").Root.Element("App");
                XElement extElement = appElement.Element("Extensions");
                XElement protocolElement = extElement.Element("Protocol");
                if (protocolElement != null)
                {
                    string name = protocolElement.Attribute("Name").Value;

                    int index = tempUri.IndexOf(name);
                    if (index > 0)
                    {
                        string tempMsg = tempUri.Substring(index);
                        index = tempMsg.IndexOf("?");
                        if (index > 0)
                        {
                            string uriMsg = tempMsg.Substring(index + 1);
                            return new Uri("/xFaceLib;component/xFacePage.xaml?Msg=" + uriMsg, UriKind.Relative);
                        }
                        else
                        {
                            return new Uri("/xFaceLib;component/xFacePage.xaml", UriKind.Relative);
                        }
                    }
                }
            }

            return uri;
        }
    }

}
