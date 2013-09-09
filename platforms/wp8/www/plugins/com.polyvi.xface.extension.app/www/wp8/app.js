cordova.define("com.polyvi.xface.extension.app.app1", function(require, exports, module) {
/**
 * @module app
 */
var argscheck = require('cordova/argscheck'),
    exec = require('cordova/exec');
var app = {};


app.exitApp = function(){
    //closeApplication
    exec(null, null, "App", "exitApp", []);
};

app.backHistory = function(successCallback, errorCallback){
    argscheck.checkArgs('FF', 'App.backHistory', arguments);
    exec(successCallback, errorCallback, "App", "backHistory", []);
};
module.exports = app;
});