(function () {
    var button = document.querySelector('#cookieConsent button[data-cookie-string]');
    var container = document.querySelector('#cookieConsent');
    if (!button || !container) return;
    button.addEventListener('click', function () {
        document.cookie = button.dataset.cookieString;
        container.classList.add('d-none');
    }, false);
})();
