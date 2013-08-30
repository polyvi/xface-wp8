using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using xFaceLib.Log;

namespace xFaceLib.runtime
{
    public class XWhitelist
    {
        protected List<string> AllowedDomains;
        protected bool AllowAllDomains = false;
        protected static string[] AllowedSchemes = { "http", "https", "ftp", "ftps" };

        public XWhitelist(List<string> Domains)
        {
            //初始化AllowedDomains
            AllowedDomains = new List<string>();
            if (null == Domains || 0 == Domains.Count)
            {
                // no config, allow all
                AllowAllDomains = true;
            }
            else
            {
                foreach (string access in Domains)
                {
                    AddWhiteListEntry(access, false);
                }
            }
        }

        /**   
         
         An access request is granted for a given URI if there exists an item inside the access-request list such that:

            - The URI's scheme component is the same as scheme; and
            - if subdomains is false or if the URI's host component is not a domain name (as defined in [RFC1034]), the URI's host component is the same as host; or
            - if subdomains is true, the URI's host component is either the same as host, or is a subdomain of host (as defined in [RFC1034]); and
            - the URI's port component is the same as port.
         
         **/

        public bool URLIsAllowed(string url)
        {
            // easy case first
            if (this.AllowAllDomains)
            {
                return true;
            }
            else
            {
                // start simple
                Uri uri = new Uri(url, UriKind.RelativeOrAbsolute);
                if (uri.IsAbsoluteUri)
                {
                    if (this.SchemeIsAllowed(uri.Scheme))
                    {
                        // additional test because our pattern will always have a trailing '/'
                        string matchUrl = url;
                        if (uri.PathAndQuery == "/")
                        {
                            matchUrl = url + "/";
                        }
                        foreach (string pattern in AllowedDomains)
                        {
                            if (Regex.IsMatch(matchUrl, pattern))
                            {
                                // make sure it is at the start, and not part of the query string
                                // special case :: http://some.other.domain/page.html?x=1&g=http://build.apache.org/
                                if (Regex.IsMatch(uri.Scheme + "://" + uri.Host + "/", pattern) ||
                                     (!Regex.IsMatch(uri.PathAndQuery, pattern)))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        protected bool SchemeIsAllowed(string scheme)
        {
            scheme = scheme.ToLower();
            return AllowedSchemes.Contains(scheme);
        }

        protected void AddWhiteListEntry(string origin, bool allowSubdomains)
        {

            if (origin == "*")
            {
                AllowAllDomains = true;
            }

            if (AllowAllDomains)
            {
                return;
            }

            string hostMatchingRegex = "";
            string hostName;

            try
            {

                Uri uri = new Uri(origin.Replace("*", "replaced-text"), UriKind.Absolute);

                string tempHostName = uri.Host.Replace("replaced-text", "*");
                // starts with wildcard match - we make the first '.' optional (so '*.org.apache.cordova' will match 'org.apache.cordova')
                if (tempHostName.StartsWith("*."))
                {    //"(\\s{0}|*.)"
                    hostName = @"\w*.*" + tempHostName.Substring(2).Replace(".", @"\.").Replace("*", @"\w*");
                }
                else
                {
                    hostName = tempHostName.Replace(".", @"\.").Replace("*", @"\w*");
                }
                //  "^https?://"
                hostMatchingRegex = uri.Scheme + "://" + hostName + uri.PathAndQuery;
                AllowedDomains.Add(hostMatchingRegex);

            }
            catch (UriFormatException)
            {
                XLog.WriteError("Invalid Whitelist entry (probably missing the protocol):: " + origin);
            }

        }
    }
}
