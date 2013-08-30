describe('PushNotification (xFace.PushNotification)', function () {
   it("should exist", function() {
        expect(xFace.PushNotification).toBeDefined();
    });

    it("should contain proper functions", function() {
        expect(xFace.PushNotification.getDeviceToken).toBeDefined();
        expect(typeof xFace.PushNotification.getDeviceToken).toBe('function');
        expect(xFace.PushNotification.registerOnReceivedListener).toBeDefined();
        expect(typeof xFace.PushNotification.registerOnReceivedListener).toBe('function');
    });
});