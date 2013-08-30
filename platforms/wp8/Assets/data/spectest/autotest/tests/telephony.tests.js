describe('Telephony (xFace.Telephony)', function () {
    it("should exist", function() {
        expect(xFace.Telephony).toBeDefined();
    });

    it("should contain proper functions", function() {
        if(isAndroid()) {
	        expect(xFace.Telephony.deleteAllCallRecords).toBeDefined();
	        expect(typeof xFace.Telephony.deleteAllCallRecords).toBe('function');
	        expect(xFace.Telephony.deleteCallRecord).toBeDefined();
	        expect(typeof xFace.Telephony.deleteCallRecord).toBe('function');
	        expect(xFace.Telephony.getCallRecord).toBeDefined();
	        expect(typeof xFace.Telephony.getCallRecord).toBe('function');
	        expect(xFace.Telephony.findCallRecords).toBeDefined();
	        expect(typeof xFace.Telephony.findCallRecords).toBe('function');
	        expect(xFace.Telephony.getCallRecordCount).toBeDefined();
	        expect(typeof xFace.Telephony.getCallRecordCount).toBe('function');
        }
        expect(xFace.Telephony.initiateVoiceCall).toBeDefined();
        expect(typeof xFace.Telephony.initiateVoiceCall).toBe('function');
    });
    if(isAndroid()) {
	    describe('getCallRecordCount method', function() {
	        var fail = createDoNotCallSpy('getCallRecordCountFail');
	        var win = jasmine.createSpy().andCallFake(function(entry) {
			    expect(entry).toBeDefined();
				expect(entry).toBeGreaterThan(-1);
	        });
	        it("should be able to getCallRecordCount", function() {
	            runs(function() {
	                var telephony = xFace.Telephony;
	                telephony.getCallRecordCount(xFace.Telephony.CallRecordTypes.OUTGOING,win, fail);
	            });
	            waitsForAny(win, fail);
	         });
	    });
		
	    describe('getCallRecord method', function() {
	        var fail = createDoNotCallSpy('getCallRecordFail');
	        var win = jasmine.createSpy().andCallFake(function(entry) {
			    if(null != entry) {
					expect(entry.callRecordAddress).toBeDefined();
					expect(entry.callRecordId).toBeDefined();
					expect(entry.callRecordName).toBeDefined();
					expect(entry.durationSeconds).toBeDefined();
					expect(entry.startTime).toBeDefined();
					expect(entry.callRecordType).toBeDefined();
				}
	        });
	        it("should be able to get telephony first call record", function() {
	            runs(function() {
	                var telephony = xFace.Telephony;
	                telephony.getCallRecord(xFace.Telephony.CallRecordTypes.OUTGOING,"1",win, fail);
	            });
	            waitsForAny(win, fail);
	         });
	    });

	    describe('findCallRecords method', function() {
	        var fail = createDoNotCallSpy('findCallRecordsFail');
	        var win = jasmine.createSpy().andCallFake(function(entry) {
				expect(entry.length).toBeGreaterThan(-1);
	        });
	        it("should be able to find calls record", function() {
	            runs(function() {
	                var telephony = xFace.Telephony;
	                var compairedCallRecord = new xFace.Telephony.CallRecord("*","","*","",null,null);
	                telephony.findCallRecords(compairedCallRecord,1,3,win, fail);
	            });
	            waitsForAny(win, fail);
	         });
	    });
		
	    describe('deleteCallRecord method', function() {
	        var fail = createDoNotCallSpy('deleteCallRecordFail');
	        var win = jasmine.createSpy().andCallFake(function() {
	        });
	        it("should be able to delete outgoing id = 0 record", function() {
	            runs(function() {
	                var telephony = xFace.Telephony;
	                telephony.deleteCallRecord(xFace.Telephony.CallRecordTypes.OUTGOING,"0",win, fail);
	            });
	            waitsForAny(win, fail);
	         });
	    });
		
	   describe('deleteAllCallRecords method', function() {
	        var fail = createDoNotCallSpy('deleteAllCallRecordsFail');
	        var win = jasmine.createSpy().andCallFake(function() {
				runs(function() {
	                var telephony = xFace.Telephony;
	                telephony.getCallRecordCount(xFace.Telephony.CallRecordTypes.OUTGOING,get_count_win, fail);
	            });
	            waitsForAny(get_count_win, fail);
	        });
			var get_count_win = jasmine.createSpy().andCallFake(function(entry) {
			    expect(entry).toBeDefined();
				expect(entry).toEqual(0);		
	        });	
	        it("should be able to delete all  call records", function() {
	            runs(function() {
	                var telephony = xFace.Telephony;
	                telephony.deleteAllCallRecords(xFace.Telephony.CallRecordTypes.OUTGOING,win, fail);
	            });
	            waitsForAny(win, fail);
	         });
	    });		
	 }
     describe('initiateVoiceCall method', function() {
        it("should be get error when initiateVoiceCall phonenumber has letter", function() {
            var win  = createDoNotCallSpy('initiateVoiceCallWin');
            var fail = jasmine.createSpy().andCallFake(function() {
            });
            runs(function() {
                var telephony = xFace.Telephony;
                telephony.initiateVoiceCall('11c2fd',win, fail);
            });
            waitsForAny(win, fail);
         });

        it("should be get error when initiateVoiceCall phonenumber null", function() {
            var win  = createDoNotCallSpy('initiateVoiceCallWin');
            var fail = jasmine.createSpy().andCallFake(function() {
            });
            runs(function() {
                var telephony = xFace.Telephony;
                telephony.initiateVoiceCall('',win, fail);
            });
            waitsForAny(win, fail);
         });

        it("should be get error when initiateVoiceCall phonenumber has Blank space", function() {
            var win  = createDoNotCallSpy('initiateVoiceCallWin');
            var fail = jasmine.createSpy().andCallFake(function() {
            });
            runs(function() {
                var telephony = xFace.Telephony;
                telephony.initiateVoiceCall('010 1234567',win, fail);
            });
            waitsForAny(win, fail);
         });

        it("should be get error when initiateVoiceCall phonenumber has chinese", function() {
            var win  = createDoNotCallSpy('initiateVoiceCallWin');
            var fail = jasmine.createSpy().andCallFake(function() {
            });
            runs(function() {
                var telephony = xFace.Telephony;
                telephony.initiateVoiceCall('ÖÐÎÄ',win, fail);
            });
            waitsForAny(win, fail);
        });
    });
});