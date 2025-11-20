(function () {
    const originalSetAttribute = Element.prototype.setAttribute;

    Element.prototype.setAttribute = function (name, value) {
        try {
            return originalSetAttribute.call(this, name, value);
        } catch (err) {
            console.error('[invalid-attribute-guard] blocked attribute application', {
                name,
                value,
                element: this,
                error: err
            });
            // Swallow the exception to avoid breaking the page render pipeline
            return undefined;
        }
    };

    window.addEventListener('error', (event) => {
        console.error('[global-error]', event.message, event.error);
    });

    window.addEventListener('unhandledrejection', (event) => {
        console.error('[global-unhandled-rejection]', event.reason);
    });
})();
