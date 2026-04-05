(function () {
    var data = document.getElementById('purchaseToastData');
    if (!data) return;

    var from = data.dataset.from || '';
    var to   = data.dataset.to   || '';
    var org  = data.dataset.org  || '';
    var isBg = document.documentElement.lang === 'bg';

    function fireToast() {
        if (typeof showToast === 'function') {
            var title = isBg ? '🎫 Билетът е закупен' : '🎫 Ticket Purchased';
            showToast(title, from + ' \u2192 ' + to + ' | ' + org, 'success');
        }
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', fireToast);
    } else {
        fireToast();
    }
})();
