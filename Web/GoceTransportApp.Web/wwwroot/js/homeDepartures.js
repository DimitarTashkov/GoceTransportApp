(function () {
    document.querySelectorAll('.popular-route-btn').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var fromId  = this.dataset.from;
            var toId    = this.dataset.to;
            var fromSel = document.getElementById('fromCitySelect');
            var toSel   = document.getElementById('toCitySelect');
            if (fromSel) fromSel.value = fromId;
            if (toSel)   toSel.value   = toId;
            document.getElementById('searchForm').submit();
        });
    });

    var select    = document.getElementById('nextDeparturesCitySelect');
    var container = document.getElementById('nextDeparturesContainer');
    if (!select || !container) return;

    var msgEmpty   = container.dataset.msgEmpty   || 'Select a city to see upcoming departures';
    var msgLoading = container.dataset.msgLoading || 'Loading...';
    var msgError   = container.dataset.msgError   || 'Unable to load departures.';

    function loadDepartures(cityId) {
        if (!cityId) {
            container.innerHTML = '<div class="text-center py-5 text-muted"><i class="fas fa-bus fa-2x mb-2 d-block opacity-50"></i><span class="small">' + msgEmpty + '</span></div>';
            return;
        }
        container.innerHTML = '<div class="text-center py-4 text-muted"><i class="fas fa-spinner fa-spin fa-2x mb-2 d-block"></i><span class="small">' + msgLoading + '</span></div>';
        fetch('/Home/NextDepartures?fromCityId=' + encodeURIComponent(cityId))
            .then(function (r) { return r.text(); })
            .then(function (html) { container.innerHTML = html; })
            .catch(function () { container.innerHTML = '<div class="text-center py-4 text-muted small">' + msgError + '</div>'; });
    }

    select.addEventListener('change', function () { loadDepartures(this.value); });

    setInterval(function () {
        if (select.value) loadDepartures(select.value);
    }, 60000);
})();
