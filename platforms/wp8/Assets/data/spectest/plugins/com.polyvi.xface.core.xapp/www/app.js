cordova.define("com.polyvi.xface.core.xapp.app", function(require, exports, module) {

/**
* 该类提供一系列基础api，用于进行app通信以及监听app的启动、关闭事件<br/>
* @class App
* @module App
*/
var channel = require('cordova/channel');
/**
* 当前应用收到其它应用发送的消息时，该事件被触发<br/>
* 注意：只支持主应用与普通应用之间进行通信
* @example
      function handler(data) {
          console.log("Received message: " + data);
      }
      cordova.app.addEventListener("message", handler);
* @event message
* @param {String} data 其它应用发送的数据
*/
var message = channel.create("message");
/**
* 当一个应用启动时，该事件被触发<br/>
* 注意：只有主应用能够监听该事件
* @example
      function handler() {
          console.log("One app has started!");
      }
      cordova.app.addEventListener("start", handler);
* @event start
*/
var start = channel.create("start");
/**
* 当一个应用关闭时，该事件被触发<br/>
* 注意：只有主应用能够监听该事件
* @example
      function handler() {
          console.log("One app has closed!");
      }
      cordova.app.addEventListener("start", handler);
* @event close
*/
var close = channel.create("close");

var app =
{
/**
* 注册应用相关的事件监听器
* @example
      function handler(data) {
	      console.log("Received message: " + data);
      }
      cordova.app.addEventListener("message", handler);
* @method addEventListener
* @param {String} evt 事件类型，仅支持"message", "start", "close"
* @param {Function} handler 事件触发时的回调函数
* @param {String} handler.data 当注册的事件为"message"事件时有效，用于接收应用之间通信时传递的数据
*/
addEventListener:function(evt, handler){
    var e = evt.toLowerCase();
    if(e == "message"){
        message.subscribe(handler);
    }else if(e == "start"){
        start.subscribe(handler);
    }else if(e == "close"){
        close.subscribe(handler);
    }
},

/**
* 注销应用相关的事件监听器
* @example
      function handler(data) {
          console.log("Received message: " + data);
      }
      cordova.app.addEventListener("message", handler);

      // do something ......

      cordova.app.removeEventListener("message", handler);
* @method removeEventListener
* @param {String} evt 事件类型，支持"message", "start", "close"
* @param {Function} handler 要注销的事件监听器<br/>
*  （该事件监听器通过{{#crossLink "cordova.App/addEventListener"}}{{/crossLink}}接口注册过）
*/
removeEventListener:function(evt, handler){
    var e = evt.toLowerCase();
    if(e == "message"){
        message.unsubscribe(handler);
    }else if(e == "start"){
        start.unsubscribe(handler);
    }else if(e == "close"){
        close.unsubscribe(handler);
    }
},

/**
* 引擎触发应用相关事件的入口函数
*/
fireAppEvent: function(evt, id){
    var e = evt.toLowerCase();
    if( e == "message"){
        var data = localStorage.getItem(id);
        localStorage.removeItem(id);
        message.fire(data);
    }else if(e == "start"){
        start.fire();
    }else if(e == "close"){
        close.fire();
    }
},

/**
* 向其它应用发送消息<br/>
* 注意：只支持主应用与普通应用之间进行通信
* @example
      cordova.app.sendMessage("This is the message content sent to another app!", null);
* @method sendMessage
* @param {Object} data 要发送的消息内容
*/
sendMessage:function(data){
    function toString(data)
    {
        var result;
        if( typeof data == 'string'){
            result = data;
        }else if( data !== null && typeof data == 'object'){
            result = data.toString();
        }
        return result;
    }
    function generateUniqueMsgId()
    {
        var msgId = parseInt((Math.random() * 65535), 10).toString(10);
        while(null !== localStorage.getItem(msgId))
        {
            msgId = parseInt((Math.random() * 65535), 10).toString(10);
        }
        return msgId;
    }

    var args = arguments;

    //如果是portal,则消息接收者是所有的app，如果是app，则消息接收者是portal
    var msgId = generateUniqueMsgId();
    localStorage.setItem(msgId, toString(data));
    require('xFace/extension/privateModule').execCommand("xFace_app_send_message:", [msgId]);
}

};
module.exports = app;
});