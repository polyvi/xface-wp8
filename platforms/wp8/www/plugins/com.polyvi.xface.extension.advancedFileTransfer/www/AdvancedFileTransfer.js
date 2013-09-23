cordova.define("com.polyvi.xface.extension.advancedFileTransfer.AdvancedFileTransfer", function(require, exports, module) {
/*
 Copyright 2012-2013, Polyvi Inc. (http://www.xface3.com)
 This program is distributed under the terms of the GNU General Public License.

 This file is part of xFace.

 xFace is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation, either version 3 of the License, or
 (at your option) any later version.

 xFace is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with xFace.  If not, see <http://www.gnu.org/licenses/>.
 */

 /**
 * 该模块提供条文件下载功能
 * @module AdvancedFileTransfer
 * @main AdvancedFileTransfer
 */

	var require = cordova.require;
	var exec = require('cordova/exec');
	var DirectoryEntry = require('org.apache.cordova.core.file.DirectoryEntry');
    var FileEntry = require('org.apache.cordova.core.file.FileEntry');

	var AdvancedFileTransfer = function(source, target, isUpload){
		this.source = source;
		this.target = target;
	    this.isUpload = isUpload || false;
	    this.onprogress = null;
	};

/**
 * 开始文件下载（Android）<br/>
  @example
      var source = "http://www.polyvi.net:8012/develop/TrafficStats/chmdecoder.zip";
      var target = "/mnt/sdcard/fileloadfile/chmdecoder.zip";
      function download(){
        var fileTransfer = new AdvancedFileTransfer(source, target);
        fileTransfer.download(downloadSuccess, downloadFail);
      };
      function downloadSuccess(result){
        if(result.status == "unfinished"){
          var res = "completeSize:" + result.loaded;
          document.getElementById('status').innerText = "downloading";
          document.getElementById('result').innerText = res;
        }else{
          var res = "file: " + result.name + " has been downloaded";
          document.getElementById('status').innerText = "download successfully";
          document.getElementById('result').innerText = res;
        }
      };
      function downloadFail(result){
        var res = "result code " + result.code;
        document.getElementById('status').innerText = "fail";
        document.getElementById('result').innerText = res;
      };
 */
	AdvancedFileTransfer.prototype.download = function(successCallback, errorCallback){
		var win = function(result) {
	        var entry = null;
	        if (result.isDirectory) {
	            entry = new DirectoryEntry();
	        }
	        else if (result.isFile) {
	            entry = new FileEntry();
	        }
	        entry.isDirectory = result.isDirectory;
	        entry.isFile = result.isFile;
	        entry.name = result.name;
	        entry.fullPath = result.fullPath;
	        successCallback(entry);
	    };
		exec(successCallback, errorCallback, "AdvancedFileTransfer", "download", [this.source, this.target]);
	};

/**
 * 暂停文件传输（Android）<br/>
  @example
      function pause(){
        var fileTransfer = new AdvancedFileTransfer(source, target);
        fileTransfer.pause();
      };
 */
	AdvancedFileTransfer.prototype.pause = function(){
		exec(null, null, "AdvancedFileTransfer", "pause", [this.source]);
	};

/**
 * 取消文件传输任务（Android）<br/>
  @example
      function cancel(){
        var fileTransfer = new AdvancedFileTransfer(source, target);
        fileTransfer.cancel();
      };
 */
	AdvancedFileTransfer.prototype.cancel = function(){
		exec(null, null, "AdvancedFileTransfer", "cancel", [this.source, this.target]);
	};

	module.exports = AdvancedFileTransfer;
});