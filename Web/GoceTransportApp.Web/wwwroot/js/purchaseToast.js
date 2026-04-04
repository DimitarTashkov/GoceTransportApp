(function () {
    var data = document.getElementById('purchaseToastData');
    if (!data) return;

    var from = data.dataset.from || '';
    var to   = data.dataset.to   || '';
    var org  = data.dataset.org  || '';

    function fireToast() {
        if (typeof showToast === 'function') {
            showToast('\uD83C\uDFAB \u0411\u0438\u043B\u0435\u0442\u044A\u0442 \u0435 \u0437\u0430\u043A\u0443\u043F\u0435\u043D', from + ' \u2192 ' + to + ' | ' + org, 'success');
        }
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', fireToast);
    } else {
        fireToast();
    }
})();
