using System;
using System.Windows;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using Microsoft.Phone.Controls;
using System.Linq;
using xFaceLib.Util;
using WPCordovaClassLib.Cordova.JSON;
using xFaceLib.Log;

// FIXME:
// Translates DOMStorage API between JS and Isolated Storage
// Missing pieces : QUOTA_EXCEEDED_ERR  + StorageEvent  


namespace xFaceLib.runtime
{
    public class XDOMStorageHelper
    {
        public XDOMStorageHelper()
        {
            // always clear session at creation
            UserSettings["sessionStorage"] = new Dictionary<string, string>();

            if (!UserSettings.Contains("localStorage"))
            {
                UserSettings["localStorage"] = new Dictionary<string, string>();
                UserSettings.Save();
            }
            Application.Current.Exit += new EventHandler(OnAppExit);
        }

        void OnAppExit(object sender, EventArgs e)
        {
            UserSettings.Remove("sessionStorage");
            UserSettings.Save();
        }

        protected IsolatedStorageSettings UserSettings
        {
            get
            {
                return IsolatedStorageSettings.ApplicationSettings;
            }
        }

        protected Dictionary<string, string> getStorageByType(string type)
        {
            if (!UserSettings.Contains(type))
            {
                UserSettings[type] = new Dictionary<string, string>();
                UserSettings.Save();
            }
            return UserSettings[type] as Dictionary<string, string>;
        }

        public void InjectScript(WebBrowser webBrowser)
        {
            string script = @"(function(win,doc) {
            {
                var DOMStorage = function(type) {
                    if(type == 'sessionStorage') {
                        this._type = type;
                    }
                    Object.defineProperty( this, 'length', {
                        configurable: true,
                        get: function(){ return this.getLength(); }
                    });
                };

                DOMStorage.prototype = {
                    _type:'localStorage',
                    _result:null,
                    keys:null,

                    onResult:function(key,valueStr) {
                        if(!this.keys) {
                            this.keys = [];
                        }
                        this._result = valueStr;
                    },

                    onKeysChanged:function(jsonKeys) {
                        this.keys = JSON.parse(jsonKeys);

                        var key;
                        for(var n = 0,len = this.keys.length; n < len; n++) {
                            key = this.keys[n];
                            if(!this.hasOwnProperty(key)) {
                                Object.defineProperty( this, key, {
                                    configurable: true,
                                    get: function(){ return this.getItem(key); },
                                    set: function(val){ return this.setItem(key,val); }
                                });
                            }
                        }

                    },

                    initialize:function() {
                        window.external.Notify('DOMStorage/' + this._type + '/load/keys');
                    },

                    getLength:function() {
                        if(!this.keys) {
                            this.initialize();
                        }
                        return this.keys.length;
                    },

                    key:function(n) {
                        if(!this.keys) {
                            this.initialize();
                        }

                        if(n >= this.keys.length) {
                            return null;
                        } else {
                            return this.keys[n];
                        }
                    },

                    getItem:function(key) {
                        if(!this.keys) {
                            this.initialize();
                        }

                        var retVal = null;
                        window.external.Notify('DOMStorage/' + this._type + '/get/' + key);
                        if(this._result) {
                            retVal = window.unescape(decodeURIComponent(this._result));
                            this._result = null;
                        }
                        return retVal;
                    },

                    setItem:function(key,value) {
                        if(!this.keys) {
                            this.initialize();
                        }
                        window.external.Notify('DOMStorage/' + this._type + '/set/' + key + '/' + encodeURIComponent(window.escape(value)));
                    },

                    removeItem:function(key) {
                        if(!this.keys) {
                            this.initialize();
                        }
                        var index = this.keys.indexOf(key);
                        if(index > -1) {
                            this.keys.splice(index,1);
                            window.external.Notify('DOMStorage/' + this._type + '/remove/' + key);
                            delete this[key];
                        }
                    },

                    clear:function() {
                        if(!this.keys) {
                            this.initialize();
                        }

                        for(var n=0,len=this.keys.length; n < len;n++) {
                            delete this[this.keys[n]];
                        }
                        this.keys = [];
                        window.external.Notify('DOMStorage/' + this._type + '/clear/');
                    }
                };

                Object.defineProperty(window, 'localStorage', {
                    writable: false,
                    configurable: false,
                    value: new DOMStorage('localStorage')
                });
                window.localStorage.initialize();

                Object.defineProperty(window, 'sessionStorage', {
                    writable: false,
                    configurable: false,
                    value: new DOMStorage('sessionStorage')
                });
                window.sessionStorage.initialize();
            }

            })(window, document);";
            XSafeBrowserScriptInvoker scriptInvoker = new XSafeBrowserScriptInvoker();
            if (!scriptInvoker.Exec(webBrowser, "execScript", new string[] { script }))
            {
                XLog.WriteError("execScript XDOMStorageHelper failde!");
            }

        }

        //<appId,<key,value>>
        public void HandleStorageCommand(string commandStr, WebBrowser webBrowser)
        {

            string[] split = commandStr.Split('/');
            if (split.Length > 3)
            {
                string api = split[0];
                string type = split[1]; // localStorage || sessionStorage
                string command = split[2];
                string param = split[3]; //key

                Dictionary<string, string> currentStorage = getStorageByType(type);
                XSafeBrowserScriptInvoker scriptInvoker = new XSafeBrowserScriptInvoker();

                switch (command)
                {
                    case "get":
                        {
                            if (currentStorage.Keys.Contains(param))
                            {
                                string value = currentStorage[param];
                                string jsString = "window." + type + ".onResult('" + param + "','" + value + "');";
                                scriptInvoker.Exec(webBrowser, "execScript", new string[] { jsString });
                            }
                            else
                            {
                                string jsString = "window." + type + ".onResult('" + param + "');";
                                scriptInvoker.Exec(webBrowser, "execScript", new string[] { jsString });
                            }
                        }
                        break;
                    case "load":
                        {
                            string[] keys = currentStorage.Keys.ToArray();
                            string jsonString = JsonHelper.Serialize(keys);
                            string callbackJS = "window." + type + ".onKeysChanged('" + jsonString + "');";
                            scriptInvoker.Exec(webBrowser, "execScript", new string[] { callbackJS });
                        }
                        break;
                    case "set":
                        {
                            // value should exist
                            if (split.Length > 4)
                            {
                                currentStorage[param] = split[4];//value
                                UserSettings.Save();
                                string[] keys = currentStorage.Keys.ToArray();
                                string jsonString = JsonHelper.Serialize(keys);
                                string callbackJS = "window." + type + ".onKeysChanged('" + jsonString + "');";
                                scriptInvoker.Exec(webBrowser, "execScript", new string[] { callbackJS });
                            }
                        }
                        break;
                    case "remove":
                        currentStorage.Remove(param);
                        UserSettings.Save();
                        break;
                    case "clear":
                        currentStorage = new Dictionary<string, string>();
                        UserSettings[type] = currentStorage;
                        UserSettings.Save();
                        break;
                }

            }

        }
    }
}
