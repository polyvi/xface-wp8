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

describe("Message", function () {
    it("should exist", function() {
        expect(xFace.Messaging).toBeDefined();

    });

    it("should define constants for Message Types", function() {
        expect(xFace.MessageTypes).toBeDefined();
        expect(xFace.MessageTypes.EmailMessage == "Email").toBe(true);
        expect(xFace.MessageTypes.MMSMessage == "MMS").toBe(true);
        expect(xFace.MessageTypes.SMSMessage == "SMS").toBe(true);
    });

    it("should contain a createMessage function", function() {
        expect(xFace.Messaging.createMessage).toBeDefined();
        expect(typeof xFace.Messaging.createMessage == 'function').toBe(true);
    });

    describe("createMessage method", function() {
        it("success callback should be called with a message", function() {
            var win = jasmine.createSpy().andCallFake(function(message) {
                    expect(message).toBeDefined();
                    expect(message.subject).toBeDefined();
                    expect(message.body).toBeDefined();
                    expect(message.destinationAddresses).toBeDefined();
                    expect(message.messageType).toBeDefined();
                    if (isAndroid()) {
                        expect(message.messageId).toBeDefined();
                        expect(message.date).toBeDefined();
                        expect(message.isRead).toBeDefined();
                    }
                }),
                fail = jasmine.createSpy();

            runs(function () {
                xFace.Messaging.createMessage(xFace.MessageTypes.SMSMessage, win, fail);
            });

            waitsFor(function () { return win.wasCalled; }, "win never called", Tests.TEST_TIMEOUT);

            runs(function () {
                expect(fail).not.toHaveBeenCalled();
            });
        });
    });

    describe("createMessage method with unknown type", function() {
        it("fail callback should be called", function() {
            var win = jasmine.createSpy().andCallFake(function(message) {
                    expect(message).toBeDefined();
                    expect(message.subject).toBeDefined();
                    expect(message.body).toBeDefined();
                    expect(message.destinationAddresses).toBeDefined();
                    expect(message.messageType).toBeDefined();
                    if (isAndroid()) {
                        expect(message.messageId).toBeDefined();
                        expect(message.date).toBeDefined();
                        expect(message.isRead).toBeDefined();
                    }
                }),
                fail = jasmine.createSpy();

            runs(function () {
                xFace.Messaging.createMessage("UNKNOWN_TYPE", win, fail);
            });

            waitsFor(function () { return fail.wasCalled; }, "fail never called", Tests.TEST_TIMEOUT);

            runs(function () {
                expect(win).not.toHaveBeenCalled();
            });
        });
    });

    if (isAndroid()) {
        it("should define constants for MessageFolder Types", function() {
            expect(xFace.MessageTypes).toBeDefined();
            expect(xFace.MessageFolderTypes.DRAFTS == "DRAFT").toBe(true);
            expect(xFace.MessageFolderTypes.INBOX == "INBOX").toBe(true);
            expect(xFace.MessageFolderTypes.OUTBOX == "OUTBOX").toBe(true);
            expect(xFace.MessageFolderTypes.SENTBOX == "SENT").toBe(true);
        });

        it("should contain a getMessage  function", function() {
            expect(xFace.Messaging.getMessage).toBeDefined();
            expect(typeof xFace.Messaging.getMessage  == 'function').toBe(true);
        });
        describe("getMessage method", function() {
            it("success callback should be called with a message", function() {
                var win = jasmine.createSpy().andCallFake(function(message) {
                        expect(message).toBeDefined();
                        expect(message.subject).toBeDefined();
                        expect(message.body).toBeDefined();
                        expect(message.destinationAddresses).toBeDefined();
                        expect(message.messageType).toBeDefined();
                        expect(message.messageId).toBeDefined();
                        expect(message.date).toBeDefined();
                        expect(message.isRead).toBeDefined();

                    }),
                    fail = jasmine.createSpy();

                runs(function () {
                    xFace.Messaging.getMessage (xFace.MessageTypes.SMSMessage, xFace.MessageFolderTypes.INBOX, 0, win, fail);
                });

                waitsFor(function () { return win.wasCalled; }, "win never called", Tests.TEST_TIMEOUT);

                runs(function () {
                    expect(fail).not.toHaveBeenCalled();
                });
            });
        });

        it("should contain a getAllMessages function", function() {
            expect(xFace.Messaging.getAllMessages).toBeDefined();
            expect(typeof xFace.Messaging.getAllMessages  == 'function').toBe(true);
        });
        describe("getAllMessages method", function() {
            it("success callback should be called with a message array", function() {
                var win = jasmine.createSpy().andCallFake(function(messages) {
                        expect(messages).toBeDefined();
                        expect(messages.length >= 0).toBe(true);
                    }),
                    fail = jasmine.createSpy();

                runs(function () {
                    xFace.Messaging.getAllMessages(xFace.MessageTypes.SMSMessage, xFace.MessageFolderTypes.INBOX, win, fail);
                });

                waitsFor(function () { return win.wasCalled; }, "win never called", Tests.TEST_TIMEOUT);

                runs(function () {
                    expect(fail).not.toHaveBeenCalled();
                });
            });
        });

        it("should contain a getQuantities function", function() {
            expect(xFace.Messaging.getMessage).toBeDefined();
            expect(typeof xFace.Messaging.getMessage  == 'function').toBe(true);
        });

        describe("getQuantities method", function() {
            it("success callback should be called with a num", function() {
                var win = jasmine.createSpy().andCallFake(function(num) {
                        expect(num).toBeDefined();
                        expect(num >= 0).toBe(true);
                    }),
                    fail = jasmine.createSpy();

                runs(function () {
                    xFace.Messaging.getQuantities (xFace.MessageTypes.SMSMessage, xFace.MessageFolderTypes.INBOX, win, fail);
                });

                waitsFor(function () { return win.wasCalled; }, "win never called", Tests.TEST_TIMEOUT);

                runs(function () {
                    expect(fail).not.toHaveBeenCalled();
                });
            });
        });

        describe("findMessage method", function() {
            it("success callback should be called with a message array", function() {
                var win = jasmine.createSpy().andCallFake(function(messages) {
                        expect(messages).toBeDefined();
                        expect(messages.length >= 0).toBe(true);
                    }),
                    fail = jasmine.createSpy();
                function successCallback(message) {
                    message.destinationAddresses = "10086";
                    message.body = "this is a test";
                    xFace.Messaging.findMessages (message, xFace.MessageFolderTypes.INBOX, 0, 3, win, fail);
                }

                runs(function () {
                    xFace.Messaging.createMessage(xFace.MessageTypes.SMSMessage, successCallback, fail);
                });

                waitsFor(function () { return win.wasCalled; }, "win never called", Tests.TEST_TIMEOUT);

                runs(function () {
                    expect(fail).not.toHaveBeenCalled();
                });
            });
        });
    }
});
