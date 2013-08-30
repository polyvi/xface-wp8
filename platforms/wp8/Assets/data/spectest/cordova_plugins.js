cordova.define('cordova/plugin_list', function(require, exports, module) {
module.exports = [
    {
        "file": "plugins\\org.apache.cordova.core.device\\www\\device.js",
        "id": "org.apache.cordova.core.device.device",
        "clobbers": [
            "device"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.network-information\\www\\network.js",
        "id": "org.apache.cordova.core.network-information.network",
        "clobbers": [
            "navigator.connection",
            "navigator.network.connection"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.network-information\\www\\Connection.js",
        "id": "org.apache.cordova.core.network-information.Connection",
        "clobbers": [
            "Connection"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.battery-status\\www\\battery.js",
        "id": "org.apache.cordova.core.battery-status.battery",
        "clobbers": [
            "navigator.battery"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.device-motion\\www\\Acceleration.js",
        "id": "org.apache.cordova.core.device-motion.Acceleration",
        "clobbers": [
            "Acceleration"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.device-motion\\www\\accelerometer.js",
        "id": "org.apache.cordova.core.device-motion.accelerometer",
        "clobbers": [
            "navigator.accelerometer"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.device-orientation\\www\\CompassError.js",
        "id": "org.apache.cordova.core.device-orientation.CompassError",
        "clobbers": [
            "CompassError"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.device-orientation\\www\\CompassHeading.js",
        "id": "org.apache.cordova.core.device-orientation.CompassHeading",
        "clobbers": [
            "CompassHeading"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.device-orientation\\www\\compass.js",
        "id": "org.apache.cordova.core.device-orientation.compass",
        "clobbers": [
            "navigator.compass"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.camera\\www\\CameraConstants.js",
        "id": "org.apache.cordova.core.camera.Camera",
        "clobbers": [
            "Camera"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.camera\\www\\CameraPopoverOptions.js",
        "id": "org.apache.cordova.core.camera.CameraPopoverOptions",
        "clobbers": [
            "CameraPopoverOptions"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.camera\\www\\Camera.js",
        "id": "org.apache.cordova.core.camera.camera",
        "clobbers": [
            "navigator.camera"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.camera\\www\\CameraPopoverHandle.js",
        "id": "org.apache.cordova.core.camera.CameraPopoverHandle",
        "clobbers": [
            "CameraPopoverHandle"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.media-capture\\www\\CaptureAudioOptions.js",
        "id": "org.apache.cordova.core.media-capture.CaptureAudioOptions",
        "clobbers": [
            "CaptureAudioOptions"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.media-capture\\www\\CaptureImageOptions.js",
        "id": "org.apache.cordova.core.media-capture.CaptureImageOptions",
        "clobbers": [
            "CaptureImageOptions"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.media-capture\\www\\CaptureVideoOptions.js",
        "id": "org.apache.cordova.core.media-capture.CaptureVideoOptions",
        "clobbers": [
            "CaptureVideoOptions"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.media-capture\\www\\CaptureError.js",
        "id": "org.apache.cordova.core.media-capture.CaptureError",
        "clobbers": [
            "CaptureError"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.media-capture\\www\\MediaFileData.js",
        "id": "org.apache.cordova.core.media-capture.MediaFileData",
        "clobbers": [
            "MediaFileData"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.media-capture\\www\\MediaFile.js",
        "id": "org.apache.cordova.core.media-capture.MediaFile",
        "clobbers": [
            "MediaFile"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.media-capture\\www\\capture.js",
        "id": "org.apache.cordova.core.media-capture.capture",
        "clobbers": [
            "navigator.device.capture"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.AudioHandler\\www\\MediaError.js",
        "id": "org.apache.cordova.core.AudioHandler.MediaError",
        "clobbers": [
            "window.MediaError"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.AudioHandler\\www\\Media.js",
        "id": "org.apache.cordova.core.AudioHandler.Media",
        "clobbers": [
            "window.Media"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.file-transfer\\www\\FileTransferError.js",
        "id": "org.apache.cordova.core.file-transfer.FileTransferError",
        "clobbers": [
            "window.FileTransferError"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.file-transfer\\www\\FileTransfer.js",
        "id": "org.apache.cordova.core.file-transfer.FileTransfer",
        "clobbers": [
            "window.FileTransfer"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.file-transfer\\www\\wp\\FileTransfer.js",
        "id": "org.apache.cordova.core.file-transfer.FileTransfer1",
        "clobbers": [
            "window.FileTransfer"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.vibration\\www\\vibration.js",
        "id": "org.apache.cordova.core.vibration.notification",
        "merges": [
            "navigator.notification"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.dialogs\\www\\notification.js",
        "id": "org.apache.cordova.core.dialogs.notification",
        "merges": [
            "navigator.notification"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.contacts\\www\\contacts.js",
        "id": "org.apache.cordova.core.contacts.contacts",
        "clobbers": [
            "navigator.contacts"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.contacts\\www\\Contact.js",
        "id": "org.apache.cordova.core.contacts.Contact",
        "clobbers": [
            "Contact"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.contacts\\www\\ContactAddress.js",
        "id": "org.apache.cordova.core.contacts.ContactAddress",
        "clobbers": [
            "ContactAddress"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.contacts\\www\\ContactError.js",
        "id": "org.apache.cordova.core.contacts.ContactError",
        "clobbers": [
            "ContactError"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.contacts\\www\\ContactField.js",
        "id": "org.apache.cordova.core.contacts.ContactField",
        "clobbers": [
            "ContactField"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.contacts\\www\\ContactFindOptions.js",
        "id": "org.apache.cordova.core.contacts.ContactFindOptions",
        "clobbers": [
            "ContactFindOptions"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.contacts\\www\\ContactName.js",
        "id": "org.apache.cordova.core.contacts.ContactName",
        "clobbers": [
            "ContactName"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.contacts\\www\\ContactOrganization.js",
        "id": "org.apache.cordova.core.contacts.ContactOrganization",
        "clobbers": [
            "ContactOrganization"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.globalization\\www\\GlobalizationError.js",
        "id": "org.apache.cordova.core.globalization.GlobalizationError",
        "clobbers": [
            "window.GlobalizationError"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.globalization\\www\\globalization.js",
        "id": "org.apache.cordova.core.globalization.globalization",
        "clobbers": [
            "navigator.globalization"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.splashscreen\\www\\splashscreen.js",
        "id": "org.apache.cordova.core.splashscreen.SplashScreen",
        "clobbers": [
            "navigator.splashscreen"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.inappbrowser\\www\\InAppBrowser.js",
        "id": "org.apache.cordova.core.inappbrowser.InAppBrowser",
        "clobbers": [
            "window.open"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.console\\www\\console-via-logger.js",
        "id": "org.apache.cordova.core.console.console",
        "clobbers": [
            "console"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.console\\www\\logger.js",
        "id": "org.apache.cordova.core.console.logger",
        "clobbers": [
            "cordova.logger"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.file\\www\\DirectoryEntry.js",
        "id": "org.apache.cordova.core.file.DirectoryEntry",
        "clobbers": [
            "window.DirectoryEntry"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.file\\www\\DirectoryReader.js",
        "id": "org.apache.cordova.core.file.DirectoryReader",
        "clobbers": [
            "window.DirectoryReader"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.file\\www\\Entry.js",
        "id": "org.apache.cordova.core.file.Entry",
        "clobbers": [
            "window.Entry"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.file\\www\\File.js",
        "id": "org.apache.cordova.core.file.File",
        "clobbers": [
            "window.File"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.file\\www\\FileEntry.js",
        "id": "org.apache.cordova.core.file.FileEntry",
        "clobbers": [
            "window.FileEntry"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.file\\www\\FileError.js",
        "id": "org.apache.cordova.core.file.FileError",
        "clobbers": [
            "window.FileError"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.file\\www\\FileReader.js",
        "id": "org.apache.cordova.core.file.FileReader",
        "clobbers": [
            "window.FileReader"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.file\\www\\FileSystem.js",
        "id": "org.apache.cordova.core.file.FileSystem",
        "clobbers": [
            "window.FileSystem"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.file\\www\\FileUploadOptions.js",
        "id": "org.apache.cordova.core.file.FileUploadOptions",
        "clobbers": [
            "window.FileUploadOptions"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.file\\www\\FileUploadResult.js",
        "id": "org.apache.cordova.core.file.FileUploadResult",
        "clobbers": [
            "window.FileUploadResult"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.file\\www\\FileWriter.js",
        "id": "org.apache.cordova.core.file.FileWriter",
        "clobbers": [
            "window.FileWriter"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.file\\www\\Flags.js",
        "id": "org.apache.cordova.core.file.Flags",
        "clobbers": [
            "window.Flags"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.file\\www\\LocalFileSystem.js",
        "id": "org.apache.cordova.core.file.LocalFileSystem",
        "clobbers": [
            "window.LocalFileSystem"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.file\\www\\Metadata.js",
        "id": "org.apache.cordova.core.file.Metadata",
        "clobbers": [
            "window.Metadata"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.file\\www\\ProgressEvent.js",
        "id": "org.apache.cordova.core.file.ProgressEvent",
        "clobbers": [
            "window.ProgressEvent"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.file\\www\\requestFileSystem.js",
        "id": "org.apache.cordova.core.file.requestFileSystem",
        "clobbers": [
            "window.requestFileSystem"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.file\\www\\resolveLocalFileSystemURI.js",
        "id": "org.apache.cordova.core.file.resolveLocalFileSystemURI",
        "clobbers": [
            "window.resolveLocalFileSystemURI"
        ]
    },
    {
        "file": "plugins\\org.apache.cordova.core.file\\www\\wp\\FileUploadOptions.js",
        "id": "org.apache.cordova.core.file.FileUploadOptions1",
        "merges": [
            "window.FileUploadOptions"
        ]
    },
    {
        "file": "plugins\\com.polyvi.xface.extension.ams\\www\\ams.js",
        "id": "com.polyvi.xface.extension.ams.AMS",
        "clobbers": [
            "window.xFace.AMS"
        ]
    },
    {
        "file": "plugins\\com.polyvi.xface.extension.ams\\www\\AmsError.js",
        "id": "com.polyvi.xface.extension.ams.AmsError",
        "clobbers": [
            "AmsError"
        ]
    },
    {
        "file": "plugins\\com.polyvi.xface.extension.ams\\www\\AmsOperationType.js",
        "id": "com.polyvi.xface.extension.ams.AmsOperationType",
        "clobbers": [
            "AmsOperationType"
        ]
    },
    {
        "file": "plugins\\com.polyvi.xface.extension.ams\\www\\AmsState.js",
        "id": "com.polyvi.xface.extension.ams.AmsState",
        "clobbers": [
            "AmsState"
        ]
    },
    {
        "file": "plugins/com.polyvi.xface.core.xapp/www/app.js",
        "id": "com.polyvi.xface.core.xapp.app",
        "clobbers": [
            "xFace.app"
        ]
    },
    {
        "file": "plugins\\com.polyvi.xface.extension.telephony\\www\\Telephony.js",
        "id": "com.polyvi.xface.extension.telephony.Telephony",
        "clobbers": [
            "window.xFace.Telephony"
        ]
    },
    {
        "file": "plugins\\com.polyvi.xface.extension.zip\\www\\Zip.js",
        "id": "com.polyvi.xface.extension.zip.Zip",
        "clobbers": [
            "window.xFace.Zip"
        ]
    },
    {
        "file": "plugins\\com.polyvi.xface.extension.zip\\www\\ZipError.js",
        "id": "com.polyvi.xface.extension.zip.ZipError",
        "clobbers": [
            "ZipError"
        ]
    },
    {
        "file": "plugins\\com.polyvi.xface.extension.zip\\www\\ZipOptions.js",
        "id": "com.polyvi.xface.extension.zip.ZipOptions",
        "clobbers": [
            "ZipOptions"
        ]
    },
	{
        "file": "plugins\\com.polyvi.xface.extension.push\\www\\PushNotification.js",
        "id": "com.polyvi.xface.extension.push.PushNotification",
        "clobbers": [
            "window.xFace.PushNotification"
        ]
    },
    {
        "file": "plugins\\com.polyvi.xface.extension.app\\www\\app.js",
        "id": "com.polyvi.xface.extension.app.app",
        "clobbers": [
            "navigator.app"
        ]
    },
    {
        "file": "plugins\\com.polyvi.xface.extension.app\\www\\wp8\\app.js",
        "id": "com.polyvi.xface.extension.app.app1",
        "merges": [
            "navigator.app"
        ]
    },
	{
        "file": "plugins\\com.polyvi.xface.extension.message\\www\\Message.js",
        "id": "com.polyvi.xface.extension.message.Message",
        "clobbers": [
            "window.xFace.Message"
        ]
    },
    {
        "file": "plugins\\com.polyvi.xface.extension.message\\www\\MessageTypes.js",
        "id": "com.polyvi.xface.extension.message.MessageTypes",
        "clobbers": [
            "window.xFace.MessageTypes"
        ]
    },
    {
        "file": "plugins\\com.polyvi.xface.extension.message\\www\\Messaging.js",
        "id": "com.polyvi.xface.extension.message.Messaging",
        "clobbers": [
            "window.xFace.Messaging"
        ]
    },
    {
        "file": "plugins\\com.polyvi.xface.extension.calendar\\www\\Calendar.js",
        "id": "com.polyvi.xface.extension.calendar.Calendar",
        "clobbers": [
            "window.xFace.ui.Calendar"
        ]
    },
    {
        "file": "plugins\\com.polyvi.xface.plugin.zbar\\www\\BarcodeScanner.js",
        "id": "com.polyvi.xface.plugin.zbar.BarcodeScanner",
        "clobbers": [
            "window.xFace.BarcodeScanner"
        ]
    },
    {
        "file": "plugins\\com.polyvi.xface.plugin.security\\www\\Security.js",
        "id": "com.polyvi.xface.plugin.security.Security",
        "clobbers": [
            "window.xFace.Security"
        ]
    },
    {
        "file": "plugins\\com.polyvi.xface.plugin.security\\www\\SecurityOptions.js",
        "id": "com.polyvi.xface.plugin.security.SecurityOptions",
        "clobbers": [
            "SecurityOptions"
        ]
    }
]
});