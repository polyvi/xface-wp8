describe('App (navigator.app)', function () {
    it("should exist", function () {
        expect(navigator.app).toBeDefined();
    });

    describe("openUrl", function() {
        it("should exist", function() {
            expect(typeof navigator.app.openUrl).toBeDefined();
            expect(typeof navigator.app.openUrl == 'function').toBe(true);
        });
    });
    if (isAndroid()) {
        describe("exitApp", function() {
                it("should exist", function() {
                    expect(typeof navigator.app.exitApp).toBeDefined();
                    expect(typeof navigator.app.exitApp == 'function').toBe(true);
                });
        });

        describe("install", function() {
                it("should exist", function() {
                    expect(typeof navigator.app.install).toBeDefined();
                    expect(typeof navigator.app.install == 'function').toBe(true);
                });
        });

        describe("backHistory", function() {
                it("should exist", function() {
                    expect(typeof navigator.app.backHistory).toBeDefined();
                    expect(typeof navigator.app.backHistory == 'function').toBe(true);
                });
        });

        describe("clearHistory", function() {
                it("should exist", function() {
                    expect(typeof navigator.app.clearHistory).toBeDefined();
                    expect(typeof navigator.app.clearHistory == 'function').toBe(true);
                });
        });

        describe("clearCache", function() {
                it("should exist", function() {
                    expect(typeof navigator.app.clearCache).toBeDefined();
                    expect(typeof navigator.app.clearCache == 'function').toBe(true);
                });
        });
    }

});
