describe('AdvancedFileTransfer (xFace.AdvancedFileTransfer)', function () {
	var download_source = "http://apollo.polyvi.com/develop/FileTransfer/test.rar";
	var download_target = "test.rar";
	var upload_source = "test_upload2.rar";
	var upload_target = "http://polyvi.net:8091/mi/UploadServer";

    var getMalformedUrl = function() {
        if (device.platform.match(/Android/i)) {
            // bad protocol causes a MalformedUrlException on Android
            return "httpssss://example.com";
        } else {
            // iOS doesn't care about protocol, space in hostname causes error
            return "httpssss://exa mple.com";
        }
    };
    // deletes file, if it exists, then invokes callback
    var deleteFile = function(fileName, callback) {
        callback = callback || function() {};
        var spy = jasmine.createSpy().andCallFake(callback);
        root.getFile(fileName, {create: false},
            // remove file system entry
            function(entry) {
                entry.remove(spy, spy);
            },
            // doesn't exist
            spy);
        waitsFor(function() { return spy.wasCalled; }, Tests.TEST_TIMEOUT);
    };

    it("should exist", function() {
        expect(xFace.AdvancedFileTransfer).toBeDefined();
    });

    it("should exist and be constructable", function() {
        var aft = new xFace.AdvancedFileTransfer(download_source,download_target);
        expect(aft).toBeDefined();
    });

    it("should contain a download function", function() {
        var download_aft = new xFace.AdvancedFileTransfer(download_source,download_target);
        expect(typeof download_aft.download).toBe('function');
    });

    it("should contain a pause function", function() {
        var aft = new xFace.AdvancedFileTransfer(download_source,download_target);
        expect(typeof aft.pause).toBe('function');
    });

    it("should contain a cancel function", function() {
        var aft = new xFace.AdvancedFileTransfer(download_source,download_target);
        expect(typeof aft.cancel).toBe('function');
    });

    describe('download method', function() {
        var fail = createDoNotCallSpy('downloadFail');
        var lastProgressEvent = null;
        var downloadWin = jasmine.createSpy().andCallFake(function(entry) {
            expect(entry.name).toBe(download_target);
            expect(lastProgressEvent.loaded).toBeGreaterThan(1);
        });

        it("should be able to download a file using http", function() {
            this.after(function() {
                deleteFile(download_target);
            });
            runs(function() {
                var aft = new xFace.AdvancedFileTransfer(download_source,download_target);
                aft.onprogress = function(e) {
                    lastProgressEvent = e;
                };
                aft.download(downloadWin, fail);
            });
            waitsForAny(downloadWin, fail, 10000);
         });

        it("should be able to download a file using http after pause", function() {
            this.after(function() {
                deleteFile(download_target);
            });
            runs(function() {
                var download_win = createDoNotCallSpy('download_win');
                var download_fail = createDoNotCallSpy('download_fail');
                var fail = createDoNotCallSpy('downloadFail');
                var aft = new xFace.AdvancedFileTransfer(download_source,download_target);
                aft.onprogress = function(e) {
                    lastProgressEvent = e;
                };
                aft.download(download_win, download_fail);
                aft.pause();
                aft.download(downloadWin, fail);
            });
            waitsForAny(downloadWin, fail, 10000);
         });

        it("should be stopped by pause() ", function() {
            var aft = new xFace.AdvancedFileTransfer(download_source,download_target);
            var downloadWin = createDoNotCallSpy('downloadWin');
            var downloadFail = jasmine.createSpy().andCallFake(function(e) {
            });
            this.after(function() {
                deleteFile(download_target+".temp");
            });
            runs(function() {
                aft.pause(); // should be a no-op.
                aft.download(downloadWin, downloadFail);
                aft.pause();
                aft.pause(); // should be a no-op.
            });
         });

        it("should be stopped by cancel()", function() {
            var aft = new xFace.AdvancedFileTransfer(download_source,download_target);
            var downloadWin = createDoNotCallSpy('downloadWin');
            var downloadFail = jasmine.createSpy().andCallFake(function(e) {
            });
            this.after(function() {
                deleteFile(download_target+".temp");
            });
            runs(function() {
                aft.cancel(); // should be a no-op.
                aft.download(downloadWin, downloadFail);
                aft.cancel();
                aft.cancel(); // should be a no-op.
            });
         });

        it("should get http status on failure", function() {
            var downloadWin = createDoNotCallSpy('downloadWin');
            var remoteFile = download_source + "/404";
            var downloadFail = jasmine.createSpy().andCallFake(function(error) {
                expect(error.code).toBe(FileTransferError.CONNECTION_ERR);
            });
            this.after(function() {
                deleteFile(download_target);
            });
            runs(function() {
                var aft = new xFace.AdvancedFileTransfer(remoteFile,download_target);
                aft.download(downloadWin, downloadFail);
            });
            waitsForAny(downloadWin, downloadFail);
        });

        it("should handle malformed urls", function() {
            var downloadWin = createDoNotCallSpy('downloadWin');
            var remoteFile = getMalformedUrl();
            var downloadFail = jasmine.createSpy().andCallFake(function(error) {
                // Note: Android needs the bad protocol to be added to the access list
                // <access origin=".*"/> won't match because ^https?:// is prepended to the regex
                // The bad protocol must begin with http to avoid automatic prefix
                expect(error.code).toBe(FileTransferError.INVALID_URL_ERR);
            });
            this.after(function() {
                deleteFile(download_target);
            });
            runs(function() {
                var aft = new xFace.AdvancedFileTransfer(remoteFile,download_target);
                aft.download(downloadWin, downloadFail);
            });
            waitsForAny(downloadWin, downloadFail);
        });

        it("should handle unknown host", function() {
            var downloadWin = createDoNotCallSpy('downloadWin');
            var remoteFile = "http://192.168.3.123/index.html";
            var localFileName = remoteFile.substring(remoteFile.lastIndexOf('/')+1);
            var downloadFail = jasmine.createSpy().andCallFake(function(error) {
                expect(error.code).toBe(FileTransferError.CONNECTION_ERR);
            });
            runs(function() {
                var aft = new xFace.AdvancedFileTransfer(remoteFile,localFileName);
                aft.download(downloadWin, downloadFail);
            });
           // iOS 的网络链接超时为60秒，故等待时间要大于60秒
            waitsForAny(downloadWin, downloadFail, 62000);
        });

        it("should handle bad file path", function() {
            var downloadWin = createDoNotCallSpy('downloadWin');
            var badFilePath = "c:\\54321";
            var downloadFail = jasmine.createSpy().andCallFake(function(error) {
            expect(error.code).toBe(FileTransferError.FILE_NOT_FOUND_ERR);
            });
            runs(function() {
                var aft = new xFace.AdvancedFileTransfer(download_source,badFilePath);
                aft.download(downloadWin, downloadFail);
            });
            waitsForAny(downloadWin, downloadFail);
        });
    });
    if (isAndroid()) {
        describe('upload method', function() {
            var fail = createDoNotCallSpy('uploadFail');
            var lastProgressEvent = null;
            var uploadWin = jasmine.createSpy().andCallFake(function(entry) {
                expect(lastProgressEvent.loaded).toBeGreaterThan(1);
            });

            it("should be able to upload a file (iOS not support upload function now! please ignore!!!)", function() {
                runs(function() {
                    var aft = new xFace.AdvancedFileTransfer(upload_source,upload_target,true);
                    aft.onprogress = function(e) {
                        lastProgressEvent = e;
                    };
                    aft.upload(uploadWin, fail);
                });
                waitsForAny(uploadWin, fail,90000);
            });

            it("should be able to upload a file using http after pause(iOS not support upload function now! please ignore!!!)", function() {
                runs(function() {
                    var upload_win = createDoNotCallSpy('upload_win');
                    var upload_fail = createDoNotCallSpy('upload_fail');
                    var aft = new xFace.AdvancedFileTransfer(upload_source,upload_target,true);
                    aft.onprogress = function(e) {
                        lastProgressEvent = e;
                    };
                    aft.upload(upload_win, upload_fail);
                    aft.pause();
                    aft.upload(uploadWin, fail);
                });
                waitsForAny(uploadWin, fail,90000);
            });

            it("should be stopped by pause()(iOS not support upload function now! please ignore!!!)", function() {
                var aft = new xFace.AdvancedFileTransfer(upload_source,upload_target,true);
                var uploadWin = createDoNotCallSpy('uploadWin');
                var uploadFail = jasmine.createSpy().andCallFake(function(e) {
                });
                runs(function() {
                    aft.pause(); // should be a no-op.
                    aft.upload(uploadWin, uploadFail);
                    aft.pause();
                    aft.pause(); // should be a no-op.
                });
            });

            it("should be stopped by cancel() (iOS not support upload function now! please ignore!!!)", function() {
                var aft = new xFace.AdvancedFileTransfer(upload_source,upload_target,true);
                var uploadWin = createDoNotCallSpy('uploadWin');
                var uploadFail = jasmine.createSpy().andCallFake(function(e) {
                });
                runs(function() {
                    aft.cancel(); // should be a no-op.
                    aft.upload(uploadWin, uploadFail);
                    aft.cancel();
                    aft.cancel(); // should be a no-op.
                });
            });

            it("should get http status on failure(iOS not support upload function now! please ignore!!!)", function() {
                var uploadWin = createDoNotCallSpy('uploadWin');
                var remoteFile = upload_source + "/404";
                var uploadFail = jasmine.createSpy().andCallFake(function(error) {
                    expect(error.code).toBe(FileTransferError.INVALID_URL_ERR);
                });
                runs(function() {
                    var aft = new xFace.AdvancedFileTransfer(upload_source,remoteFile,true);
                    aft.upload(uploadWin, uploadFail);
                });
                waitsForAny(uploadWin, uploadFail);
            });

            it("should handle malformed urls(iOS not support upload function now! please ignore!!!)", function() {
                var uploadWin = createDoNotCallSpy('uploadWin');
                var remoteFile = getMalformedUrl();
                var uploadFail = jasmine.createSpy().andCallFake(function(error) {
                    // Note: Android needs the bad protocol to be added to the access list
                    // <access origin=".*"/> won't match because ^https?:// is prepended to the regex
                    // The bad protocol must begin with http to avoid automatic prefix
                    expect(error.code).toBe(FileTransferError.INVALID_URL_ERR);
                });
                runs(function() {
                    var aft = new xFace.AdvancedFileTransfer(upload_source,remoteFile,true);
                    aft.upload(uploadWin, uploadFail);
                });
                waitsForAny(uploadWin, uploadFail);
            });

            it("should handle unknown host(iOS not support upload function now! please ignore!!!)", function() {
                var uploadWin = createDoNotCallSpy('uploadWin');
                var remoteFile = "http://192.168.3.123/index.html";
                var uploadFail = jasmine.createSpy().andCallFake(function(error) {
                    expect(error.code).toBe(FileTransferError.INVALID_URL_ERR);
                });
                runs(function() {
                    var aft = new xFace.AdvancedFileTransfer(upload_source,remoteFile,true);
                    aft.upload(uploadWin, uploadFail);
                });
                waitsForAny(uploadWin, uploadFail,90000);
            });

            it("should handle missing file(iOS not support upload function now! please ignore!!!)", function() {
                var uploadWin = createDoNotCallSpy('uploadWin');
                var localFileName = "does_not_exist.txt";
                var uploadFail = jasmine.createSpy().andCallFake(function(error) {
                    expect(error.code).toBe(FileTransferError.FILE_NOT_FOUND_ERR);
                });
                runs(function() {
                    var aft = new xFace.AdvancedFileTransfer(localFileName,upload_target,true);
                    aft.upload(uploadWin, uploadFail);
                });
                waitsForAny(uploadWin, uploadFail);
            });

            it("should handle bad file path(iOS not support upload function now! please ignore!!!)", function() {
                var uploadWin = createDoNotCallSpy('uploadWin');
                var uploadFail = jasmine.createSpy().andCallFake(function(error) {
                    expect(error.code).toBe(FileTransferError.FILE_NOT_FOUND_ERR);

                });
                runs(function() {
                    var aft = new xFace.AdvancedFileTransfer("c:/path.txt",upload_target,true);
                    aft.upload(uploadWin, uploadFail);
                });
                waitsForAny(uploadWin, uploadFail);
            });
        });
    }
});