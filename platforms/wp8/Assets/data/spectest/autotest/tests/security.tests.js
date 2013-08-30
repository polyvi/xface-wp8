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

describe("Security", function() {
    var timeout = 90000; 
    var key = "polyvi2012";
    var plainText = "adfadfasdfasdfadsfderqwerqeradsfcxzvvzcxvzxcvtiutkgjh413241324123413412431341dsfgsnvmvbmvbnmvbmvbnmvbnv";
    var cipherText = "pNLh/fodihMybXFXUufIqp4MmDzFndS/RYq0J1mehsLMn952rQjmi6+/bpT8qQiZCMbluTGbS12XizPeRN0rvsKhPaPdcJei0mob070zgc/d4W1dwr6BYYutTrXmS2DIrcm7bnVKEJM="; 
    var tripleDESKey = "123456789012345612345678";
    var tripleDESKeyInHex = "313233343536373839303132333435363132333435363738";
    var tripleCipherText = "bg/8AhfSR9nvEzMg12eJAS9l82o8XnRPqwXIlNtHEC4NVy8ubO99jhxSvu8Mb2eDqrovy+GD+ppnlxDxwrTX0OLPrT/gMhkBZkmnQB+1KwykZzgBubQp7tsuwpibqT2Zm41gHcQn1FM=";
    var tripleCipherTextHex = "6E0FFC0217D247D9EF133320D76789012F65F36A3C5E744FAB05C894DB47102E0D572F2E6CEF7D8E1C52BEEF0C6F6783AABA2FCBE183FA9A679710F1C2B4D7D0E2CFAD3FE03219016649A7401FB52B0CA4673801B9B429EEDB2EC2989BA93D999B8D601DC427D453";

    var RSAPubKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCYU/+I0+z1aBl5X6DUUOHQ7FZpmBSDbKTtx89J"+
                    "EcB64jFCkunELT8qiKly7fzEqD03g8ALlu5XvX+bBqHFy7YPJJP0ekE2X3wjUnh2NxlqpH3/B/xm"+
                    "1ZdSlCwDIkbijhBVDjA/bu5BObhZqQmDwIxlQInL9oVz+o6FbAZCyHBd7wIDAQAB";

    var RSACipherText = "DLAclJro4BRhSye+6IvYuz3CbFKEIOvLJmlYuIxsbJMUOD8GHL+EFOJ8PXwZA+DfvILq3lTY4h0q7p1qKmu9"+
                        "MCaLdUlZsrCvap7AdnAnT8rU2WVDsMPW47pCJYPR7WipvNDwC2kbZ+crS3ZBJb9/SVC34ZfL6Oolovb9jFjQ"+
                        "xmJn+FPtv0JRfEXafoFoqAJ8NDeqw3oGqQ1VsGnSR4UQdSqyIfHpeqI+So+mi/j7xKVRxz+uFe9j/fealICD"+
                        "0tHFiLxUekeTl5Nwoz68W5Lk9y37D6iEjNWhUvkAO/+nmqmmGiewF5ua07O+ae6yX6sY+0oy0mE68jtFYwJn"+
                        "9mROCA==";

    var RSAPriKey = "MIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBAJhT/4jT7PVoGXlfoNRQ4dDsVmmY"+
                    "FINspO3Hz0kRwHriMUKS6cQtPyqIqXLt/MSoPTeDwAuW7le9f5sGocXLtg8kk/R6QTZffCNSeHY3"+
                    "GWqkff8H/GbVl1KULAMiRuKOEFUOMD9u7kE5uFmpCYPAjGVAicv2hXP6joVsBkLIcF3vAgMBAAEC"+
                    "gYBvZHWoZHmS2EZQqKqeuGr58eobG9hcZzWQoJ4nq/CarBAjw/VovUHE490uK3S9ht4FW7Yzg3LV"+
                    "/MB06Huifh6qf/X9NQA7SeZRRC8gnCQk6JuDIEVJOud5jU+9tyumJakDKodQ3Jf2zQtNr+5ZdEPl"+
                    "uwWgv9c4kmpjhAdyMuQmYQJBANn6pcgvyYaia52dnu+yBUsGkaFfwXkzFSExIbi0MXTkhEb/ER/D"+
                    "rLytukkUu5S5ecz/KBa8U4xIslZDYQbLz5ECQQCy5dutt7RsxN4+dxCWn0/1FrkWl2G329Ucewm3"+
                    "QU9CKu4D+7Kqdj+Ha3lXP8F0Etaaapi7+EfkRUpukn2ItZV/AkEAlk+I0iphxT1rCB0Q5CjWDY5S"+
                    "Df2B5JmdEG5Y2o0nLXwG2w44OLct/k2uD4cEcuITY5Dvi/4BftMCZwm/dnhEgQJACIktJSnJwxLV"+
                    "o9dchENPtlsCM9C/Sd2EWpqISSUlmfugZbJBwR5pQ5XeMUqKeXZYpP+HEBj1nS+tMH9u2/IGEwJA"+
                    "fL8mZiZXan/oBKrblAbplNcKWGRVD/3y65042PAEeghahlJMiYquV5DzZajuuT0wbJ5xQuZB01+X"+
                    "nfpFpBJ2dw==";

    var FILE_NOT_FOUND_ERR = 1;
    var PATH_ERR = 2;
    var OPERATION_ERR = 3;
    it("should exist", function() {
        expect(xFace.Security).toBeDefined();
    });

    it("should contain a encrypt function", function() {
        expect(xFace.Security.encrypt).toBeDefined();
        expect(typeof xFace.Security.encrypt == 'function').toBe(true);
    });

    describe("encrypt method", function() {
        it("success callback should be called with encrypted text", function() {
            var win = jasmine.createSpy().andCallFake(function(encryptedText) {
                expect(encryptedText).toBeDefined();
                expect(encryptedText == cipherText).toBe(true);
            }),
                fail = jasmine.createSpy();
            runs(function() {
                xFace.Security.encrypt(key, plainText, win, fail);
            });

            waitsFor(function() {
                return win.wasCalled;
            }, "win never called", Tests.TEST_TIMEOUT);

            runs(function() {
                expect(fail).not.toHaveBeenCalled();
            });
        });

        it("by 3DES algorithm and encode type String success callback should be called with encrypted text", function() {
            var win = jasmine.createSpy().andCallFake(function(encryptedText) {
                expect(encryptedText).toBeDefined();
                expect(encryptedText == tripleCipherText).toBe(true);
            }),
                fail = jasmine.createSpy();
            runs(function() {
                var options = new SecurityOptions();
                options.CryptAlgorithm = SecurityOptions.CryptAlgorithm.TRIPLE_DES;
                options.EncodeDataType = StringEncodeType.Base64;
                xFace.Security.encrypt(tripleDESKey, plainText, win, fail, options);
            });

            waitsFor(function() {
                return win.wasCalled;
            }, "win never called", Tests.TEST_TIMEOUT);

            runs(function() {
                expect(fail).not.toHaveBeenCalled();
            });
        });
        
        it("by 3DES algorithm and encode type hex success callback should be called with encrypted text", function() {
            var win = jasmine.createSpy().andCallFake(function(encryptedText) {
                expect(encryptedText).toBeDefined();
                expect(encryptedText == tripleCipherTextHex).toBe(true);
            }),
                fail = jasmine.createSpy();
            runs(function() {
                var options = new SecurityOptions();
                options.CryptAlgorithm = SecurityOptions.CryptAlgorithm.TRIPLE_DES;
                options.EncodeDataType = StringEncodeType.HEX;
                xFace.Security.encrypt(tripleDESKey, plainText, win, fail, options);
            });

            waitsFor(function() {
                return win.wasCalled;
            }, "win never called", Tests.TEST_TIMEOUT);

            runs(function() {
                expect(fail).not.toHaveBeenCalled();
            });
        });

        it("by 3DES algorithm and key&data encoded in hex success callback should be called with encrypted text", function() {
            var win = jasmine.createSpy().andCallFake(function(encryptedText) {
                expect(encryptedText).toBeDefined();
                expect(encryptedText == tripleCipherTextHex).toBe(true);
            }),
                fail = jasmine.createSpy();
            runs(function() {
                var options = new SecurityOptions();
                options.CryptAlgorithm = SecurityOptions.CryptAlgorithm.TRIPLE_DES;
                options.EncodeDataType = StringEncodeType.HEX;
                options.EncodeKeyType = StringEncodeType.HEX;
                 xFace.Security.encrypt(tripleDESKeyInHex, plainText, win, fail, options);
            });

            waitsFor(function() {
                return win.wasCalled;
            }, "win never called", Tests.TEST_TIMEOUT);

            runs(function() {
                expect(fail).not.toHaveBeenCalled();
            });

        });
        if(!isWindowsPhone())//WP8不支持 RSA
        {
            it("by RSA algorithm and encode type String success callback should be called with encrypted text", function() {

                var win = jasmine.createSpy().andCallFake(function(encryptedText) {

                    expect(encryptedText).toBeDefined();

                    expect(encryptedText == RSACipherText).toBe(true);

                }),

                    fail = jasmine.createSpy();

                runs(function() {

                    var options = new SecurityOptions();

                    options.CryptAlgorithm = SecurityOptions.CryptAlgorithm.RSA;

                    options.EncodeKeyType = StringEncodeType.Base64;

                    options.EncodeDataType = StringEncodeType.Base64;

                    xFace.Security.encrypt(RSAPubKey, plainText, win, fail, options);

                });

                waitsFor(function() {

                    return win.wasCalled;

                }, "win never called", Tests.TEST_TIMEOUT);

                runs(function() {

                    expect(fail).not.toHaveBeenCalled();

                });

            });
        }
    });
    it("should contain a encryptFile function", function() {
        expect(xFace.Security.encryptFile).toBeDefined();
        expect(typeof xFace.Security.encryptFile == 'function').toBe(true);
    });

    describe("encryptFile method", function() {
        it("success callback should be called with encrypted file path", function() {
            var win = jasmine.createSpy().andCallFake(function(encryptedFilePath) {
                expect(encryptedFilePath).toBeDefined();
                expect(encryptedFilePath).toNotBe(null);
            }),
                fail = jasmine.createSpy();
            var sourceFilePath = "encrypt_source.apk";
            var targetFilePath = "encrypt_target.apk";
            runs(function() {
                xFace.Security.encryptFile(key, sourceFilePath, targetFilePath, win, fail);
            });
            waitsFor(function() {
                return win.wasCalled || fail.wasCalled;
            }, "win never called", timeout);
            runs(function() {
                expect(fail).not.toHaveBeenCalled();
            });
        });

        it("it should be  error  with sourceFilePath empty", function() {
            var win = jasmine.createSpy(),
                fail = jasmine.createSpy().andCallFake(function(errorCode) {
                    expect(errorCode).toBeDefined();
                    expect(PATH_ERR == errorCode).toBe(true);
                });
            var sourceFilePath = "";
            var targetFilePath = "encrypt_target.apk";
            runs(function() {
                xFace.Security.encryptFile(key, sourceFilePath, targetFilePath, win, fail);
            });
            waitsFor(function() {
                return fail.wasCalled;
            }, "fail never called", Tests.TEST_TIMEOUT);
            runs(function() {
                expect(win).not.toHaveBeenCalled();
            });
        });

        it("it should be  error  with targetFilePath empty", function() {
            var win = jasmine.createSpy(),
                fail = jasmine.createSpy().andCallFake(function(errorCode) {
                    expect(errorCode).toBeDefined();
                    expect(PATH_ERR == errorCode).toBe(true);
                });
            var sourceFilePath = "encrypt_source.apk";
            var targetFilePath = "";
            runs(function() {
                xFace.Security.encryptFile(key, sourceFilePath, targetFilePath, win, fail);
            });
            waitsFor(function() {
                return fail.wasCalled;
            }, "fail never called", Tests.TEST_TIMEOUT);
            runs(function() {
                expect(win).not.toHaveBeenCalled();
            });
        });

        it("it should be  error  with sourceFile not exist", function() {
            var win = jasmine.createSpy(),
                fail = jasmine.createSpy().andCallFake(function(errorCode) {
                    expect(errorCode).toBeDefined();
                    expect(FILE_NOT_FOUND_ERR == errorCode).toBe(true);
                });
            var sourceFilePath = "not_exist.apk";
            var targetFilePath = "encrypt_target.apk";
            runs(function() {
                xFace.Security.encryptFile(key, sourceFilePath, targetFilePath, win, fail);
            });
            waitsFor(function() {
                return fail.wasCalled;
            }, "fail never called", Tests.TEST_TIMEOUT);
            runs(function() {
                expect(win).not.toHaveBeenCalled();
            });
        });

        it("it should be  error  with targetFilePath bad path", function() {
            var win = jasmine.createSpy(),
                fail = jasmine.createSpy().andCallFake(function(errorCode) {
                    expect(errorCode).toBeDefined();
                    expect(PATH_ERR == errorCode).toBe(true);
                });
            var sourceFilePath = "encrypt_source.apk";
            var targetFilePath = "c://not_exist2.apk";
            runs(function() {
                xFace.Security.encryptFile(key, sourceFilePath, targetFilePath, win, fail);
            });
            waitsFor(function() {
                return fail.wasCalled;
            }, "fail never called", Tests.TEST_TIMEOUT);
            runs(function() {
                expect(win).not.toHaveBeenCalled();
            });
        });

        if(isAndroid()) {
            it("it should be  error  with absolute sourcePath", function() {
                var win = jasmine.createSpy(),
                    fail = jasmine.createSpy().andCallFake(function(errorCode) {
                        expect(errorCode).toBeDefined();
                        expect(FILE_NOT_FOUND_ERR == errorCode).toBe(true);
                    });
                var sourceFilePath = "file:///mnt/sdcard/xFacePlayer/applications/app/workspace/encrypt_source.apk";
                var targetFilePath = "encrypt_target.apk";
                runs(function() {
                    xFace.Security.encryptFile(key, sourceFilePath, targetFilePath, win, fail);
                });
                waitsFor(function() {
                    return fail.wasCalled;
                }, "fail never called", Tests.TEST_TIMEOUT);
                runs(function() {
                    expect(win).not.toHaveBeenCalled();
                });
            });
        }
    });

    it("should contain a decrypt function", function() {
        expect(xFace.Security.decrypt).toBeDefined();
        expect(typeof xFace.Security.decrypt == 'function').toBe(true);
    });

    describe("decrypt method", function() {
        it("success callback should be called with plain text", function() {
            var win = jasmine.createSpy().andCallFake(function(decryptedText) {
                expect(decryptedText).toBeDefined();
                expect(decryptedText == plainText).toBe(true);
            }),
                fail = jasmine.createSpy();

            runs(function() {
                xFace.Security.decrypt(key, cipherText, win, fail);
            });

            waitsFor(function() {
                return win.wasCalled;
            }, "win never called", Tests.TEST_TIMEOUT);

            runs(function() {
                expect(fail).not.toHaveBeenCalled();
            });
        });

        it("by 3DES and encode type String success callback should be called with plain text", function() {
            var win = jasmine.createSpy().andCallFake(function(decryptedText) {
                expect(decryptedText).toBeDefined();
                expect(decryptedText == plainText).toBe(true);
            }),
                fail = jasmine.createSpy();

            runs(function() {
                var options = new SecurityOptions();
                options.CryptAlgorithm = SecurityOptions.CryptAlgorithm.TRIPLE_DES;
                options.EncodeDataType = StringEncodeType.Base64;
                xFace.Security.decrypt(tripleDESKey, tripleCipherText, win, fail,options);
            });

            waitsFor(function() {
                return win.wasCalled;
            }, "win never called", Tests.TEST_TIMEOUT);

            runs(function() {
                expect(fail).not.toHaveBeenCalled();
            });
        });

        it("by 3DES algorithm and encode type hex success callback should be called with encrypted text", function() {
            var win = jasmine.createSpy().andCallFake(function(encryptedText) {
                expect(encryptedText).toBeDefined();
                expect(encryptedText == plainText).toBe(true);
            }),
                fail = jasmine.createSpy();
            runs(function() {
                var options = new SecurityOptions();
                options.CryptAlgorithm = SecurityOptions.CryptAlgorithm.TRIPLE_DES;
                options.EncodeDataType = StringEncodeType.HEX;
                xFace.Security.decrypt(tripleDESKey, tripleCipherTextHex, win, fail, options);
            });

            waitsFor(function() {
                return win.wasCalled;
            }, "win never called", Tests.TEST_TIMEOUT);

            runs(function() {
                expect(fail).not.toHaveBeenCalled();
            });
        });

    if(isAndroid()) {
        it("by RSA algorithm and encode type String success callback should be called with encrypted text", function() {

            var win = jasmine.createSpy().andCallFake(function(encryptedText) {

                expect(encryptedText).toBeDefined();

                expect(encryptedText == plainText).toBe(true);

            }),

                fail = jasmine.createSpy();

            runs(function() {

                var options = new SecurityOptions();

                options.CryptAlgorithm = SecurityOptions.CryptAlgorithm.RSA;

                options.EncodeDataType = StringEncodeType.Base64;

                options.EncodeKeyType = StringEncodeType.Base64;

                xFace.Security.decrypt(RSAPriKey, RSACipherText, win, fail, options);

            });



            waitsFor(function() {

                return win.wasCalled;

            }, "win never called", Tests.TEST_TIMEOUT);



            runs(function() {

                expect(fail).not.toHaveBeenCalled();

            });

        });
    }//end of isAndroid

        it("success callback should be called when the fail callback is missing", function() {
            var win = jasmine.createSpy().andCallFake(function(decryptedText) {
                expect(decryptedText).toBeDefined();
                expect(decryptedText == plainText).toBe(true);
            });

            runs(function() {
                xFace.Security.decrypt(key, cipherText, win);
            });
            waitsFor(function() {
                return win.wasCalled;
            }, "win never called", Tests.TEST_TIMEOUT);
        });
    });

    it("should contain a decryptFile function", function() {
        expect(xFace.Security.decryptFile).toBeDefined();
        expect(typeof xFace.Security.decryptFile == 'function').toBe(true);
    });

    describe("decryptFile method", function() {
        it("success callback should be called with decrypted file path", function() {
            var win = jasmine.createSpy().andCallFake(function(decryptedFilePath) {
                expect(decryptedFilePath).toBeDefined();
                expect(decryptedFilePath).toNotBe(null);
            }),
                fail = jasmine.createSpy();
            var sourceFilePath = !isAndroid() ? "encrypt_target.apk" : "decrypt_source.apk";
            var targetFilePath = "decrypt_target.apk";
            runs(function() {
                xFace.Security.decryptFile(key, sourceFilePath, targetFilePath, win, fail);
            });
            waitsFor(function() {
               return win.wasCalled || fail.wasCalled;
            }, "win never called", timeout);
            runs(function() {
                expect(fail).not.toHaveBeenCalled();
            });
        });

        it("it should be  error  with sourceFilePath empty", function() {
            var win = jasmine.createSpy(),
                fail = jasmine.createSpy().andCallFake(function(errorCode) {
                    expect(errorCode).toBeDefined();
                    expect(PATH_ERR == errorCode).toBe(true);
                });
            var sourceFilePath = "";
            var targetFilePath = "decrypt_target.apk";
            runs(function() {
                xFace.Security.decryptFile(key, sourceFilePath, targetFilePath, win, fail);
            });
            waitsFor(function() {
                return fail.wasCalled;
            }, "fail never called", Tests.TEST_TIMEOUT);
            runs(function() {
                expect(win).not.toHaveBeenCalled();
            });
        });

        it("it should be  error  with targetFilePath empty", function() {
            var win = jasmine.createSpy(),
                fail = jasmine.createSpy().andCallFake(function(errorCode) {
                    expect(errorCode).toBeDefined();
                    expect(PATH_ERR == errorCode).toBe(true);
                });
            var sourceFilePath = "decrypt_source.apk";
            var targetFilePath = "";
            runs(function() {
                xFace.Security.decryptFile(key, sourceFilePath, targetFilePath, win, fail);
            });
            waitsFor(function() {
                return fail.wasCalled;
            }, "fail never called", Tests.TEST_TIMEOUT);
            runs(function() {
                expect(win).not.toHaveBeenCalled();
            });
        });

        it("it should be  error  with sourceFile not exist", function() {
            var win = jasmine.createSpy(),
                fail = jasmine.createSpy().andCallFake(function(errorCode) {
                    expect(errorCode).toBeDefined();
                    expect(FILE_NOT_FOUND_ERR == errorCode).toBe(true);
                });
            var sourceFilePath = "not_exist2.apk";
            var targetFilePath = "decrypt_target.apk";
            runs(function() {
                xFace.Security.decryptFile(key, sourceFilePath, targetFilePath, win, fail);
            });
            waitsFor(function() {
                return fail.wasCalled;
            }, "fail never called", Tests.TEST_TIMEOUT);
            runs(function() {
                expect(win).not.toHaveBeenCalled();
            });
        });

        it("it should be  error  with targetFilePath bad path", function() {
            var win = jasmine.createSpy(),
                fail = jasmine.createSpy().andCallFake(function(errorCode) {
                    expect(errorCode).toBeDefined();
                    expect(PATH_ERR == errorCode).toBe(true);
                });
            var sourceFilePath = "decrypt_source.apk";
            var targetFilePath = "c://not_exist2.apk";
            runs(function() {
                xFace.Security.decryptFile(key, sourceFilePath, targetFilePath, win, fail);
            });
            waitsFor(function() {
                return fail.wasCalled;
            }, "fail never called", Tests.TEST_TIMEOUT);
            runs(function() {
                expect(win).not.toHaveBeenCalled();
            });
        });

        if(isAndroid()) {
            it("it should be  error  with absolute sourcePath", function() {
                var win = jasmine.createSpy(),
                    fail = jasmine.createSpy().andCallFake(function(errorCode) {
                        expect(errorCode).toBeDefined();
                        expect(FILE_NOT_FOUND_ERR == errorCode).toBe(true);
                    });
                var sourceFilePath = "file:///mnt/sdcard/xFacePlayer/applications/app/workspace/decrypt_source.apk";
                var targetFilePath = "decrypt_target.apk";
                runs(function() {
                    xFace.Security.decryptFile(key, sourceFilePath, targetFilePath, win, fail);
                });
                waitsFor(function() {
                    return fail.wasCalled;
                }, "fail never called", Tests.TEST_TIMEOUT);
                runs(function() {
                    expect(win).not.toHaveBeenCalled();
                });
            });
        }
    });

    describe("key length test", function() {
        it("fail callback should be called when key length is less than 8", function() {
            var win = jasmine.createSpy().andCallFake(function(encryptedText) {
                expect(encryptedText).toBeDefined();
                expect(encryptedText == cipherText).toBe(true);
            }),
                fail = jasmine.createSpy();

            runs(function() {
                xFace.Security.encrypt("123", plainText, win, fail);
            });

            waitsFor(function() {
                return fail.wasCalled;
            }, "fail never called", Tests.TEST_TIMEOUT);

            runs(function() {
                expect(win).not.toHaveBeenCalled();
            });
        });

        //encrypt
        it("it should be  return  with encrypt key lenth less than 8 and errorcallback not exist", function() {
            xFace.Security.encrypt("123", plainText);
        });  
        
        //decrypt
        it("it should be  return  with decrypt key lenth less than 8 and errorcallback not exist", function() {
            xFace.Security.decrypt("123", plainText);
        });
        
        //encryptFile
        it("it should be  return  with encryptFile key lenth less than 8 and errorcallback not exist", function() {
            var sourceFilePath = "encrypt_source.apk";
            var targetFilePath = "encrypt_target.apk";
            xFace.Security.encryptFile("123", sourceFilePath, targetFilePath);
        });
        
        //decryptFile
        it("it should be  return  with decryptFile key lenth less than 8 and errorcallback not exist", function() {
            var sourceFilePath = "decrypt_source.apk";
            var targetFilePath = "decrypt_target.apk";
            xFace.Security.decryptFile("123", sourceFilePath, targetFilePath);
        });
    });

    describe("arguments type", function() {
        it("should throw  TypeError exception when the type of plain text is a function", function() {
            var win = jasmine.createSpy().andCallFake(function(encryptedText) {
                expect(encryptedText).toBeDefined();
                expect(encryptedText == cipherText).toBe(true);
            }),
                fail = jasmine.createSpy();

            runs(function() {
                try {
                    xFace.Security.encrypt(key, function() {}, win, fail);
                }
                catch (e) {
                    expect(e.name == "TypeError").toBe(true);
                }
            });

            runs(function() {
                expect(fail).not.toHaveBeenCalled();
            });

            runs(function() {
                expect(win).not.toHaveBeenCalled();
            });
        });

        it("should throw  TypeError exception when the type of key is a function", function() {
            var win = jasmine.createSpy().andCallFake(function(encryptedText) {
                expect(encryptedText).toBeDefined();
                expect(encryptedText == cipherText).toBe(true);
            }),
                fail = jasmine.createSpy();

            runs(function() {
                try {
                    xFace.Security.encrypt(function() {}, plainText, win, fail);
                }
                catch (e) {
                    expect(e.name == "TypeError").toBe(true);
                }
            });

            runs(function() {
                expect(fail).not.toHaveBeenCalled();
            });

            runs(function() {
                expect(win).not.toHaveBeenCalled();
            });
        });

        it("should throw  TypeError exception when the type of  key is int", function() {
            var win = jasmine.createSpy().andCallFake(function(encryptedText) {
                expect(encryptedText).toBeDefined();
                expect(encryptedText == cipherText).toBe(true);
            }),
                fail = jasmine.createSpy();

            runs(function() {
                try {
                    xFace.Security.encrypt(1, plainText, win, fail);
                }
                catch (e) {
                    expect(e.name == "TypeError").toBe(true);
                }
            });

            runs(function() {
                expect(fail).not.toHaveBeenCalled();
            });

            runs(function() {
                expect(win).not.toHaveBeenCalled();
            });
        });

        it("it should throw  TypeError exception with key is null", function() {
            var win = jasmine.createSpy().andCallFake(function(encryptedText) {
                expect(encryptedText).toBeDefined();
                expect(encryptedText == cipherText).toBe(true);
            }),
                fail = jasmine.createSpy();

            runs(function() {
                try {
                    xFace.Security.encrypt(null, plainText, win, fail);
                }
                catch (e) {
                    expect(e.name == "TypeError").toBe(true);
                }
            });

            runs(function() {
                expect(fail).not.toHaveBeenCalled();
            });

            runs(function() {
                expect(win).not.toHaveBeenCalled();
            });
        });

        it("it should throw  TypeError exception with plainText is null", function() {
            var win = jasmine.createSpy().andCallFake(function(encryptedText) {
                expect(encryptedText).toBeDefined();
                expect(encryptedText == cipherText).toBe(true);
            }),
            fail = jasmine.createSpy();
            runs(function() {
                try {
                    xFace.Security.encrypt(key, null, win, fail);
                }
                catch (e) {
                    expect(e.name == "TypeError").toBe(true);
                }
            });
            runs(function() {
                expect(fail).not.toHaveBeenCalled();
            });
            runs(function() {
                expect(win).not.toHaveBeenCalled();
            });
        });

        it("it should be error with plainText empty", function() {
            var win = jasmine.createSpy().andCallFake(function(encryptedText) {
                expect(encryptedText).toBeDefined();
                expect(encryptedText == cipherText).toBe(true);
            }),
                fail = jasmine.createSpy();

            runs(function() {
                xFace.Security.encrypt(key, "", win, fail);
            });

            waitsFor(function() {
                return fail.wasCalled;
            }, "fail never called", Tests.TEST_TIMEOUT);

            runs(function() {
                expect(win).not.toHaveBeenCalled();
            });
        });

    });
});
