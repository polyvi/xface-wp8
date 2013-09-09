using System;
using xFaceLib.runtime;
using xFaceLib.ams;
using System.Collections.Generic;
using xFaceLib.Util;
using xFaceLib.Log;
using xFaceLib.extensions.ams;

namespace WPCordovaClassLib.Cordova.Commands
{
    public class AMS : BaseCommand
    {
        private XAms ams;

        public void init(XAppManagement appManagement)
        {
            this.ams = new XAmsImpl(appManagement);
        }

        public void installApplication(string options)
        {
            string packagePath = JSON.JsonHelper.Deserialize<string[]>(options)[0];
            string callbackId = JSON.JsonHelper.Deserialize<string[]>(options)[1];
            XAppInstallListener listener = new XAppInstallListener();
            EventHandler<PluginResult> DispatchPluginResult = delegate(object sender, PluginResult result)
            {
                DispatchCommandResult(result, callbackId);
            };
            listener.DispatchPluginResult += DispatchPluginResult;
            ams.InstallApp(packagePath, listener);
        }

        public void updateApplication(string options)
        {
            string packagePath = JSON.JsonHelper.Deserialize<string[]>(options)[0];
            string callbackId = JSON.JsonHelper.Deserialize<string[]>(options)[1];
            XAppInstallListener listener = new XAppInstallListener();
            EventHandler<PluginResult> DispatchPluginResult = delegate(object sender, PluginResult result)
            {
                DispatchCommandResult(result, callbackId);
            };
            listener.DispatchPluginResult += DispatchPluginResult;
            ams.UpdateApp(packagePath, listener);
        }

        public void uninstallApplication(string options)
        {
            string appid = JSON.JsonHelper.Deserialize<string[]>(options)[0];
            string callbackId = JSON.JsonHelper.Deserialize<string[]>(options)[1];
            XAppInstallListener listener = new XAppInstallListener();
            EventHandler<PluginResult> DispatchPluginResult = delegate(object sender, PluginResult result)
            {
                DispatchCommandResult(result, callbackId);
            };
            listener.DispatchPluginResult += DispatchPluginResult;
            ams.UninstallApp(appid, listener);
        }

        public void startApplication(string options)
        {
            string appid = JSON.JsonHelper.Deserialize<string[]>(options)[0];
            string appparams = JSON.JsonHelper.Deserialize<string[]>(options)[1];
            bool status = ams.StartApp(appid, appparams);
            if (status)
            {
                DispatchCommandResult(new PluginResult(PluginResult.Status.OK, appid));
            }
            else
            {
                DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, appid));
            }
        }

        public void listInstalledApplications(string options)
        {
            XApplicationList appList = ams.GetAppList();
            List<XApplication>.Enumerator appIterator = appList.GetEnumerator();
            String callbackArguments = "";
            while (appIterator.MoveNext())
            {
                XApplication tempApp = (XApplication)appIterator.Current;
                if (tempApp.AppInfo.AppId != appList.DefaultAppId)
                {
                    if (callbackArguments.Length > 0)
                        callbackArguments += ",";
                    XAppInfo info = tempApp.AppInfo;
                    String callbackArgument = String.Format("\"appid\":\"{0}\",\"name\":\"{1}\",\"version\":\"{2}\",\"type\":\"{3}\",\"width\":\"{4}\",\"height\":\"{5}\",\"icon\":\"{6}\",\"icon_background_color\":\"{7}\"",
               info.AppId, info.Name, info.Version, info.Type, info.Width, info.Height, tempApp.GetAppIconUrl(), info.IconBgColor);
                    callbackArguments += "{" + callbackArgument + "}";
                }
            }
            callbackArguments = "[" + callbackArguments + "]";
            XLog.WriteInfo("listInstalledApplications::" + callbackArguments);
            DispatchCommandResult(new PluginResult(PluginResult.Status.OK, callbackArguments));
        }

        public void listPresetAppPackages(string options)
        {
            String[] presetApps = ams.GetPresetAppPackages();
            if (presetApps != null)
            {
                int count = presetApps.Length;
                for (int i = 0; i < count; i++)
                {
                    presetApps[i] = XConstant.PRE_SET_APP_PACKAGE_DIR_NAME + "\\" + presetApps[i];
                }
                XLog.WriteInfo("listPresetAppPackages::" + string.Join("", presetApps));
            }
            DispatchCommandResult(new PluginResult(PluginResult.Status.OK, presetApps));
        }

        public void getStartAppInfo(string options)
        {
            XApplicationList appList = ams.GetAppList();
            XApplication defaultapp = appList.GetDefaultApp();
            XAppInfo info = defaultapp.AppInfo;
            string callbackArguments = String.Format("\"appid\":\"{0}\",\"name\":\"{1}\",\"version\":\"{2}\",\"type\":\"{3}\",\"width\":\"{4}\",\"height\":\"{5}\",\"icon\":\"{6}\",\"icon_background_color\":\"{7}\"",
                info.AppId, info.Name, info.Version, info.Type, info.Width, info.Height, defaultapp.GetAppIconUrl(), info.IconBgColor);
            callbackArguments = "{" + callbackArguments + "}";
            XLog.WriteInfo("getStartAppInfo::" + callbackArguments);
            DispatchCommandResult(new PluginResult(PluginResult.Status.OK, callbackArguments));
        }
    }
}