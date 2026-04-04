(function () {
    var data = document.getElementById('routeDetailMapData');
    if (!data) return;

    var fromLat = data.dataset.fromLat ? parseFloat(data.dataset.fromLat) : null;
    var fromLng = data.dataset.fromLng ? parseFloat(data.dataset.fromLng) : null;
    var toLat   = data.dataset.toLat   ? parseFloat(data.dataset.toLat)   : null;
    var toLng   = data.dataset.toLng   ? parseFloat(data.dataset.toLng)   : null;
    var fromStop = data.dataset.fromStop || data.dataset.departureLabel || 'Departure stop';
    var toStop   = data.dataset.toStop   || data.dataset.arrivalLabel   || 'Arrival stop';

    var mapsInitialized = false;

    function initRouteDetailMaps() {
        if (mapsInitialized) return;
        mapsInitialized = true;

        if (fromLat && fromLng && document.getElementById('modalFromMap')) {
            var fromPos = { lat: fromLat, lng: fromLng };
            new google.maps.Marker({
                position: fromPos,
                map: new google.maps.Map(document.getElementById('modalFromMap'), {
                    center: fromPos, zoom: 16,
                    mapTypeControl: false, streetViewControl: false
                }),
                icon: 'http://maps.google.com/mapfiles/ms/icons/green-dot.png',
                title: fromStop
            });
        }

        if (toLat && toLng && document.getElementById('modalToMap')) {
            var toPos = { lat: toLat, lng: toLng };
            new google.maps.Marker({
                position: toPos,
                map: new google.maps.Map(document.getElementById('modalToMap'), {
                    center: toPos, zoom: 16,
                    mapTypeControl: false, streetViewControl: false
                }),
                icon: 'http://maps.google.com/mapfiles/ms/icons/red-dot.png',
                title: toStop
            });
        }
    }

    var modalEl = document.getElementById('routeMapModal');
    if (modalEl) {
        modalEl.addEventListener('shown.bs.modal', function () {
            if (typeof google !== 'undefined' && google.maps) {
                initRouteDetailMaps();
            }
        });
    }

    document.querySelectorAll('#mapModalTabs [data-bs-toggle="pill"]').forEach(function (tab) {
        tab.addEventListener('shown.bs.tab', function () {
            if (typeof google !== 'undefined' && google.maps) {
                setTimeout(function () {
                    ['modalFromMap', 'modalToMap'].forEach(function (id) {
                        var el = document.getElementById(id);
                        if (el && el.__gmap) google.maps.event.trigger(el.__gmap, 'resize');
                    });
                }, 50);
            }
        });
    });
})();
