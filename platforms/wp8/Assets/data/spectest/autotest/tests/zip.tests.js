/*
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 *
*/

describe("zip/unzip (xFace)", function () {

        it("should exist xFace.Zip type", function() {
            expect(xFace.Zip).toBeDefined();
            expect(typeof xFace.Zip.zip).toBe("function");
            expect(typeof xFace.Zip.unzip).toBe("function");
            expect(typeof xFace.Zip.zipFiles).toBe("function");
        });

        it("should define constants for ZipError errors", function() {
            expect(ZipError).toBeDefined();
            expect(ZipError.FILE_NOT_EXIST).toBe(1);
            expect(ZipError.COMPRESS_FILE_ERROR).toBe(2);
            expect(ZipError.UNZIP_FILE_ERROR).toBe(3);
            expect(ZipError.FILE_PATH_ERROR).toBe(4);
            expect(ZipError.FILE_TYPE_ERROR).toBe(5);
        });

        describe("xFace.Zip.zip Method", function(){
            it("Source file exist destination file not exist! Success callback should be called ", function() {
                var srcFile         =  "SourceFile_1.html";
                var desFile         =  "/test/destination_1.zip";

                var SuccessFileReadCallBack =  jasmine.createSpy().andCallFake(function (entry){
                    expect(entry.fullPath == desFile).toBe(true);

                    // cleanup
                    deleteEntry("/test");
                });
                var FailFileReadCallBack = jasmine.createSpy().andCallFake(function(errorcode){});

                var ErrorCallBack   =  jasmine.createSpy().andCallFake(function(errorcode){});
                var  SuccessCallBack =  jasmine.createSpy().andCallFake(function(){
                    root.getFile(desFile,{create: false, exclusive: false}, SuccessFileReadCallBack,FailFileReadCallBack);
                });

                runs(function(){  xFace.Zip.zip(srcFile, desFile, SuccessCallBack, ErrorCallBack); });

                waitsFor(function() { return SuccessCallBack.wasCalled&&SuccessFileReadCallBack.wasCalled; }, "SuccessCallBack  never called", Tests.TEST_TIMEOUT);

                runs(function() {
                    expect(ErrorCallBack).not.toHaveBeenCalled();
                    expect(SuccessCallBack).toHaveBeenCalled();

                    expect(FailFileReadCallBack).not.toHaveBeenCalled();
                    expect(SuccessFileReadCallBack).toHaveBeenCalled();
                });
            });

            it("Source file exist destination file also exist! Success callback should be called ", function() {

                var srcFile         =  "SourceFile_2.html";
                var desFile         =  "destination_2.zip";

                var SuccessFileReadCallBack =  jasmine.createSpy().andCallFake(function (entry){
                    expect(entry.name == desFile).toBe(true);

                    // cleanup
                    deleteEntry(desFile);
                });
                var FailFileReadCallBack = jasmine.createSpy().andCallFake(function(errorcode){});

                var ErrorCallBack   =  jasmine.createSpy().andCallFake(function(errorcode){});
                var SuccessCallBack =  jasmine.createSpy().andCallFake(function(){
                    root.getFile(desFile,{create: false, exclusive: false}, SuccessFileReadCallBack,FailFileReadCallBack);
                });

                runs(function(){ xFace.Zip.zip(srcFile, desFile, SuccessCallBack, ErrorCallBack); });

                waitsFor(function() { return SuccessCallBack.wasCalled&&SuccessFileReadCallBack.wasCalled; }, "SuccessCallBack  never called", 5000 );

                runs(function() {
                   expect(ErrorCallBack).not.toHaveBeenCalled();
                   expect(SuccessCallBack).toHaveBeenCalled();

                   expect(ErrorCallBack).not.toHaveBeenCalled();
                   expect(SuccessFileReadCallBack).toHaveBeenCalled();
                });
            });

            it("Source file are not exist! Error callback should be called with errorcode = 1(FILE_NOT_EXIST)", function() {
                var srcFile          =  "sourceFileIsNotExist.html";
                var desFile          =  "sourceFileIsNotExist.zip";
                var ErrorCallBack    =  jasmine.createSpy().andCallFake(function(errorcode){
                    expect(errorcode).toBe(ZipError.FILE_NOT_EXIST);
                });
                var SuccessCallBack =  jasmine.createSpy().andCallFake(function(a){ });

                runs(function(){ xFace.Zip.zip(srcFile,desFile, SuccessCallBack,ErrorCallBack); });

                waitsFor(function() { return ErrorCallBack.wasCalled; }, "error callback never called", 5000);

                runs(function() {
                    expect(SuccessCallBack).not.toHaveBeenCalled();
                    expect(ErrorCallBack).toHaveBeenCalled();
                });
            });

            it("Source file are too big! success callback should be called ", function() {
                var srcFile          =  "big.data";
                var desFile          =  "big.zip";

                var SuccessFileReadCallBack =  jasmine.createSpy().andCallFake(function (entry){
                    expect(entry.name == desFile).toBe(true);

                    // cleanup
                    deleteEntry(desFile);
                });
                var FailFileReadCallBack = jasmine.createSpy().andCallFake(function(errorcode){});

                var ErrorCallBack    =  jasmine.createSpy().andCallFake(function(errorcode){});
                var SuccessCallBack =  jasmine.createSpy().andCallFake(function(){
                     root.getFile(desFile,{create: false, exclusive: false}, SuccessFileReadCallBack,FailFileReadCallBack);
                });



                runs(function(){ xFace.Zip.zip(srcFile,desFile, SuccessCallBack,ErrorCallBack); });


                waitsFor(function() { return SuccessCallBack.wasCalled&&SuccessFileReadCallBack.wasCalled; }, "SuccessCallBack  never called", 90000);

                runs(function() {
                    expect(ErrorCallBack).not.toHaveBeenCalled();
                    expect(SuccessCallBack).toHaveBeenCalled();

                    expect(FailFileReadCallBack).not.toHaveBeenCalled();
                    expect(SuccessFileReadCallBack).toHaveBeenCalled();
                });
            });

            it("Source file Path error! Error callback should be called with errorcode = 4(FILE_PATH_ERROR)", function() {
                var srcFile         =  "C:///\\\aaa///\\\aaa\\";
                var desFile         =  "srcPathWrong";
                var ErrorCallBack   =  jasmine.createSpy().andCallFake(function(errorcode){
                    expect(errorcode).toBe(ZipError.FILE_PATH_ERROR);
                });
                var  SuccessCallBack =  jasmine.createSpy().andCallFake(function(){ });

                runs(function(){ xFace.Zip.zip(srcFile, desFile, SuccessCallBack, ErrorCallBack); });

                waitsFor(function() { return ErrorCallBack.wasCalled; }, "ErrorCallBack  never called", 5000 );

                runs(function() {
                   expect(SuccessCallBack).not.toHaveBeenCalled();
                   expect(ErrorCallBack).toHaveBeenCalled();
                });
            });

            it("Destination file Path error! Error callback should be called with errorcode = 4(FILE_PATH_ERROR)", function() {
                var srcFile         =  "desPathWrong";
                var desFile         =  "C:///\\\aaa///\\\aaa\\";
                var ErrorCallBack   =  jasmine.createSpy().andCallFake(function(errorcode){
                    expect(errorcode).toBe(ZipError.FILE_PATH_ERROR);
                });
                var  SuccessCallBack =  jasmine.createSpy().andCallFake(function(){ });

                runs(function(){ xFace.Zip.zip(srcFile, desFile, SuccessCallBack, ErrorCallBack); });

                waitsFor(function() { return ErrorCallBack.wasCalled; }, "ErrorCallBack  never called", 5000 );

                runs(function() {
                   expect(SuccessCallBack).not.toHaveBeenCalled();
                   expect(ErrorCallBack).toHaveBeenCalled();
                });
            });

            it("Source file and Destination file pathes are all error!Error callback should be called with errorcode = 4(FILE_PATH_ERROR)",function(){
                var srcFile         =  "C:///\\\bbb///\\\bbb\\";
                var desFile         =  "C:///\\\aaa///\\\aaa\\";
                var ErrorCallBack   =  jasmine.createSpy().andCallFake(function(errorcode){
                    expect(errorcode).toBe(ZipError.FILE_PATH_ERROR);
                });
                var  SuccessCallBack =  jasmine.createSpy().andCallFake(function(){ });

                runs(function(){ xFace.Zip.zip(srcFile, desFile, SuccessCallBack, ErrorCallBack); });

                waitsFor(function() { return ErrorCallBack.wasCalled; }, "ErrorCallBack  never called", Tests.TEST_TIMEOUT );

                runs(function() {
                   expect(SuccessCallBack).not.toHaveBeenCalled();
                   expect(ErrorCallBack).toHaveBeenCalled();
                });
            });

        });

        describe("xFace.Zip.unzip Method",function(){

            it("Source file exists and destination path is valid! Success callback should be called ", function() {
                var srcFile          =  "UnzipSourcefile.zip";
                var desFile          =  "UnzipSourcefile";

                var SuccessFileReadCallBack =  jasmine.createSpy().andCallFake(function (entry){
                    expect(entry.name == desFile).toBe(true);

                    // cleanup
                    deleteEntry(desFile);
                });
                var FailFileReadCallBack = jasmine.createSpy().andCallFake(function(errorcode){});

                var ErrorCallBack     =  jasmine.createSpy().andCallFake(function(errorcode){ });
                var SuccessCallBack =  jasmine.createSpy().andCallFake(function(){
                     root.getDirectory(desFile,{create: false, exclusive: false}, SuccessFileReadCallBack,FailFileReadCallBack);
                });

                runs(function(){ xFace.Zip.unzip(srcFile,desFile, SuccessCallBack,ErrorCallBack); });

                waitsFor(function() { return SuccessCallBack.wasCalled&&SuccessFileReadCallBack.wasCalled; }, "SuccessCallBack  never called", 5000);

                runs(function() {
                    expect(ErrorCallBack).not.toHaveBeenCalled();
                    expect(SuccessCallBack).toHaveBeenCalled();

                    expect(FailFileReadCallBack).not.toHaveBeenCalled();
                    expect(SuccessFileReadCallBack).toHaveBeenCalled();
                });
            });

            it("Source file exists and destination path is ''! Success callback should be called ", function() {
                var srcFile          =  "theDestIsNull.zip";
                var desFile          =  "theDestIsNull.txt";

                var SuccessFileReadCallBack =  jasmine.createSpy().andCallFake(function (entry){
                    expect(entry.name == desFile).toBe(true);

                    // cleanup
                    deleteEntry(desFile);
                });
                var FailFileReadCallBack = jasmine.createSpy().andCallFake(function(errorcode){});

                var ErrorCallBack     =  jasmine.createSpy().andCallFake(function(errorcode){ });
                var SuccessCallBack =  jasmine.createSpy().andCallFake(function(){
                     root.getFile(desFile,{create: false, exclusive: false}, SuccessFileReadCallBack,FailFileReadCallBack);
                });

                runs(function(){ xFace.Zip.unzip(srcFile,"", SuccessCallBack,ErrorCallBack); });

                waitsFor(function() { return SuccessCallBack.wasCalled&&SuccessFileReadCallBack.wasCalled; }, "SuccessCallBack  never called", 5000);

                runs(function() {
                    expect(ErrorCallBack).not.toHaveBeenCalled();
                    expect(SuccessCallBack).toHaveBeenCalled();

                    expect(FailFileReadCallBack).not.toHaveBeenCalled();
                    expect(SuccessFileReadCallBack).toHaveBeenCalled();
                });
            });

            it("Source file does not exist! Error callback should be called with errorcode = 1(FILE_NOT_EXIST)", function() {
                var srcFile           =  "sourceFileIsNotExist.zip";
                var desFile           =  "";
                var ErrorCallBack     =  jasmine.createSpy().andCallFake(function(errorcode){
                     expect(errorcode).toBe(ZipError.FILE_NOT_EXIST);
                });
                var SuccessCallBack   =  jasmine.createSpy().andCallFake(function(errorcode){ });

                runs(function(){ xFace.Zip.unzip(srcFile,desFile, SuccessCallBack,ErrorCallBack); });

                waitsFor(function() { return ErrorCallBack.wasCalled; }, "ErrorCallBack  never called", 5000);

                runs(function() {
                     expect(SuccessCallBack).not.toHaveBeenCalled();
                     expect(ErrorCallBack).toHaveBeenCalled();
                });

            });


            it("Source file type error! Error callback should be called with errorcode = 5(FILE_TYPE_ERROR)", function() {
                var srcFile           =  "tesk.apk";
                var desFile           =  "";
                var ErrorCallBack     =  jasmine.createSpy().andCallFake(function(errorcode){
                    expect(errorcode).toBe(ZipError.FILE_TYPE_ERROR);
                });
                var SuccessCallBack =  jasmine.createSpy().andCallFake(function(errorcode){ });

                runs(function(){ xFace.Zip.unzip(srcFile,desFile, SuccessCallBack,ErrorCallBack); });

                waitsFor(function() { return ErrorCallBack.wasCalled; }, "ErrorCallBack  never called", 5000);

                runs(function() {
                    expect(SuccessCallBack).not.toHaveBeenCalled();
                    expect(ErrorCallBack).toHaveBeenCalled();
                });

            });

            it("Source File path error ! error callback should be called with errorcode = 4(FILE_PATH_ERROR)", function() {
                var srcFile           =  "C:///\\\aaa///\\\aaa\\test.zip";
                var desFile           =  "";
                var ErrorCallBack     =  jasmine.createSpy().andCallFake(function(errorcode){
                    expect(errorcode).toBe( ZipError.FILE_PATH_ERROR);
                });
                var SuccessCallBack =  jasmine.createSpy().andCallFake(function(){});

                runs(function(){ xFace.Zip.unzip(srcFile,desFile, SuccessCallBack,ErrorCallBack); });

                waitsFor(function() { return ErrorCallBack.wasCalled; }, "ErrorCallBack  never called", 5000 );

                runs(function() {
                    expect(SuccessCallBack).not.toHaveBeenCalled();
                    expect(ErrorCallBack).toHaveBeenCalled();
                });

            });

            it("Destination File path error! Error callback should be called with errorcode = 4(FILE_PATH_ERROR)", function() {
                var srcFile           =  "destination_2.zip";
                var desFile           =  "C:///\\\aaa///\\\aaa\\";
                var ErrorCallBack     =  jasmine.createSpy().andCallFake(function(errorcode){
                    expect(errorcode).toBe( ZipError.FILE_PATH_ERROR);
                });
                var SuccessCallBack =  jasmine.createSpy().andCallFake(function(){ });

                runs(function(){ xFace.Zip.unzip(srcFile,desFile, SuccessCallBack,ErrorCallBack); });

                waitsFor(function() { return ErrorCallBack.wasCalled; }, "ErrorCallBack  never called", 5000 );

                runs(function() {
                    expect(SuccessCallBack).not.toHaveBeenCalled();
                    expect(ErrorCallBack).toHaveBeenCalled();
                });

            });

            it("Source File and Destination File pathes all error! Error callback should be called with errorcode = 4(FILE_PATH_ERROR)", function() {
                var srcFile           =  "C:///\\\aaa///\\\aaa\\test.zip";
                var desFile           =  "C:///\\\bbb///\\\bbb\\";
                var ErrorCallBack     =  jasmine.createSpy().andCallFake(function(errorcode){
                    expect(errorcode).toBe( ZipError.FILE_PATH_ERROR);
                });
                var SuccessCallBack =  jasmine.createSpy().andCallFake(function(){ });

                runs(function(){ xFace.Zip.unzip(srcFile,desFile, SuccessCallBack,ErrorCallBack); });

                waitsFor(function() { return ErrorCallBack.wasCalled; }, "ErrorCallBack  never called", Tests.TEST_TIMEOUT );

                runs(function() {
                    expect(SuccessCallBack).not.toHaveBeenCalled();
                    expect(ErrorCallBack).toHaveBeenCalled();
                });

            });
        });

        describe("xFace.Zip.zipFiles Method", function(){

            it("compress two existed files! Success callback should be called ", function() {
                var entries         =  ["SourceFile_1.html", "SourceFile_2.html"];
                var desFile         =  "destination.zip";

                var SuccessFileReadCallBack =  jasmine.createSpy().andCallFake(function (entry){
                    expect(entry.name == desFile).toBe(true);

                    // cleanup
                    deleteEntry(desFile);
                });
                var FailFileReadCallBack = jasmine.createSpy().andCallFake(function(errorcode){});

                var ErrorCallBack     =  jasmine.createSpy().andCallFake(function(errorcode){ });
                var  SuccessCallBack =  jasmine.createSpy().andCallFake(function(){
                     root.getFile(desFile,{create: false, exclusive: false}, SuccessFileReadCallBack,FailFileReadCallBack);
                });
                runs(function(){  xFace.Zip.zipFiles(entries, desFile, SuccessCallBack, ErrorCallBack); });

                waitsFor(function() { return SuccessCallBack.wasCalled&&SuccessFileReadCallBack.wasCalled; }, "SuccessCallBack  never called", Tests.TEST_TIMEOUT);

                runs(function() {
                    expect(ErrorCallBack).not.toHaveBeenCalled();
                    expect(SuccessCallBack).toHaveBeenCalled();

                    expect(FailFileReadCallBack).not.toHaveBeenCalled();
                    expect(SuccessFileReadCallBack).toHaveBeenCalled();
                });
            });

            it("compress two existed dir! Success callback should be called ", function() {
                var entries         =  ["pre_set", "appPackage"];
                var desFile         =  "destination2.zip";

                var SuccessFileReadCallBack =  jasmine.createSpy().andCallFake(function (entry){
                    expect(entry.name == desFile).toBe(true);

                    // cleanup
                    deleteEntry(desFile);
                });
                var FailFileReadCallBack = jasmine.createSpy().andCallFake(function(errorcode){});

                var ErrorCallBack     =  jasmine.createSpy().andCallFake(function(errorcode){ });
                var  SuccessCallBack =  jasmine.createSpy().andCallFake(function(){
                     root.getFile(desFile,{create: false, exclusive: false}, SuccessFileReadCallBack,FailFileReadCallBack);
                });
                runs(function(){  xFace.Zip.zipFiles(entries, desFile, SuccessCallBack, ErrorCallBack); });

                waitsFor(function() { return SuccessCallBack.wasCalled&&SuccessFileReadCallBack.wasCalled; }, "SuccessCallBack  never called", Tests.TEST_TIMEOUT);

                runs(function() {
                    expect(ErrorCallBack).not.toHaveBeenCalled();
                    expect(SuccessCallBack).toHaveBeenCalled();

                    expect(FailFileReadCallBack).not.toHaveBeenCalled();
                    expect(SuccessFileReadCallBack).toHaveBeenCalled();
                });
            });

            it("compress a existed file and  a existed dir! Success callback should be called ", function() {
                    var entries         =  ["SourceFile_1.html", "pre_set"];
                    var desFile         =  "destination1.zip";

                    var SuccessFileReadCallBack =  jasmine.createSpy().andCallFake(function (entry){
                        expect(entry.name == desFile).toBe(true);

                        // cleanup
                        deleteEntry(desFile);
                    });
                    var FailFileReadCallBack = jasmine.createSpy().andCallFake(function(errorcode){});

                    var ErrorCallBack     =  jasmine.createSpy().andCallFake(function(errorcode){ });
                    var  SuccessCallBack =  jasmine.createSpy().andCallFake(function(){
                         root.getFile(desFile,{create: false, exclusive: false}, SuccessFileReadCallBack,FailFileReadCallBack);
                    });
                    runs(function(){  xFace.Zip.zipFiles(entries, desFile, SuccessCallBack, ErrorCallBack); });

                    waitsFor(function() { return SuccessCallBack.wasCalled&&SuccessFileReadCallBack.wasCalled; }, "SuccessCallBack  never called", Tests.TEST_TIMEOUT);

                    runs(function() {
                        expect(ErrorCallBack).not.toHaveBeenCalled();
                        expect(SuccessCallBack).toHaveBeenCalled();

                        expect(FailFileReadCallBack).not.toHaveBeenCalled();
                        expect(SuccessFileReadCallBack).toHaveBeenCalled();
                    });
                });

            it("One of source files is not exist! Error callback should be called with errorcode = 1(FILE_NOT_EXIST)", function() {
                var entries         =  ["SourceFile_1.html", "sourceFileIsNotExist.html"];
                var desFile          =  "sourceFileIsNotExist.zip";
                var ErrorCallBack    =  jasmine.createSpy().andCallFake(function(errorcode){
                    expect(errorcode).toBe(ZipError.FILE_NOT_EXIST);
                });
                var SuccessCallBack =  jasmine.createSpy().andCallFake(function(){ });

                runs(function(){ xFace.Zip.zipFiles(entries, desFile, SuccessCallBack, ErrorCallBack); });

                waitsFor(function() { return ErrorCallBack.wasCalled; }, "error callback never called", Tests.TEST_TIMEOUT);

                runs(function() {
                    expect(SuccessCallBack).not.toHaveBeenCalled();
                    expect(ErrorCallBack).toHaveBeenCalled();
                });
            });

            it("two source files are not exist! Error callback should be called with errorcode = 1(FILE_NOT_EXIST)", function() {
                var entries         =  ["sourceFileIsNotExist1.html", "sourceFileIsNotExist2.html"];
                var desFile          =  "sourceFileIsNotExist.zip";
                var ErrorCallBack    =  jasmine.createSpy().andCallFake(function(errorcode){
                    expect(errorcode).toBe(ZipError.FILE_NOT_EXIST);
                });
                var SuccessCallBack =  jasmine.createSpy().andCallFake(function(){ });

                runs(function(){ xFace.Zip.zipFiles(entries, desFile, SuccessCallBack, ErrorCallBack); });

                waitsFor(function() { return ErrorCallBack.wasCalled; }, "error callback never called", Tests.TEST_TIMEOUT);

                runs(function() {
                    expect(SuccessCallBack).not.toHaveBeenCalled();
                    expect(ErrorCallBack).toHaveBeenCalled();
                });
            });

            it("Path of one source file is invalid! Error callback should be called with errorcode = 4(FILE_PATH_ERROR)", function() {
                var entries         =  ["SourceFile_1.html", "../test"];
                var desFile         =  "srcPathWrong";
                var ErrorCallBack   =  jasmine.createSpy().andCallFake(function(errorcode){
                    expect(errorcode).toBe(ZipError.FILE_PATH_ERROR);
                });
                var  SuccessCallBack =  jasmine.createSpy().andCallFake(function(){ });

                runs(function(){ xFace.Zip.zipFiles(entries, desFile, SuccessCallBack, ErrorCallBack); });

                waitsFor(function() { return ErrorCallBack.wasCalled; }, "ErrorCallBack  never called", Tests.TEST_TIMEOUT );

                runs(function() {
                    expect(SuccessCallBack).not.toHaveBeenCalled();
                    expect(ErrorCallBack).toHaveBeenCalled();
                });
            });

            it("Path of destination file is invalid! Error callback should be called with errorcode = 4(FILE_PATH_ERROR)", function() {
                var srcFile         =  "desPathWrong";
                var desFile         =  "../test";
                var ErrorCallBack   =  jasmine.createSpy().andCallFake(function(errorcode){
                    expect(errorcode).toBe(ZipError.FILE_PATH_ERROR);
                });
                var  SuccessCallBack =  jasmine.createSpy().andCallFake(function(){ });

                runs(function(){ xFace.Zip.zip(srcFile, desFile, SuccessCallBack, ErrorCallBack); });

                waitsFor(function() { return ErrorCallBack.wasCalled; }, "ErrorCallBack  never called", Tests.TEST_TIMEOUT );

                runs(function() {
                    expect(SuccessCallBack).not.toHaveBeenCalled();
                    expect(ErrorCallBack).toHaveBeenCalled();
                });
            });
        });
});



