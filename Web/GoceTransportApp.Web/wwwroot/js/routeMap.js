var fromMap, fromMarker, toMap, toMarker;

function initMaps() {
    var data = document.getElementById('routeMapData');
    var existingFromLat = data && data.dataset.fromLat ? parseFloat(data.dataset.fromLat) : null;
    var existingFromLng = data && data.dataset.fromLng ? parseFloat(data.dataset.fromLng) : null;
    var existingToLat   = data && data.dataset.toLat   ? parseFloat(data.dataset.toLat)   : null;
    var existingToLng   = data && data.dataset.toLng   ? parseFloat(data.dataset.toLng)   : null;
    var departureLabel  = data ? data.dataset.departureLabel : 'Departure stop';
    var arrivalLabel    = data ? data.dataset.arrivalLabel   : 'Arrival stop';
    var bulgariaCenter  = { lat: 42.7339, lng: 25.4858 };

    fromMap = new google.maps.Map(document.getElementById('fromMap'), {
        center: (existingFromLat && existingFromLng) ? { lat: existingFromLat, lng: existingFromLng } : bulgariaCenter,
        zoom: (existingFromLat && existingFromLng) ? 14 : 7,
        mapTypeControl: false,
        streetViewControl: false
    });

    toMap = new google.maps.Map(document.getElementById('toMap'), {
        center: (existingToLat && existingToLng) ? { lat: existingToLat, lng: existingToLng } : bulgariaCenter,
        zoom: (existingToLat && existingToLng) ? 14 : 7,
        mapTypeControl: false,
        streetViewControl: false
    });

    if (existingFromLat && existingFromLng) {
        fromMarker = new google.maps.Marker({
            position: { lat: existingFromLat, lng: existingFromLng },
            map: fromMap,
            icon: 'http://maps.google.com/mapfiles/ms/icons/green-dot.png',
            title: departureLabel
        });
    }

    if (existingToLat && existingToLng) {
        toMarker = new google.maps.Marker({
            position: { lat: existingToLat, lng: existingToLng },
            map: toMap,
            icon: 'http://maps.google.com/mapfiles/ms/icons/red-dot.png',
            title: arrivalLabel
        });
    }

    fromMap.addListener('click', function (e) { placeFromMarker(e.latLng); });
    toMap.addListener('click', function (e) { placeToMarker(e.latLng); });

    document.querySelectorAll('[data-bs-toggle="pill"]').forEach(function (tab) {
        tab.addEventListener('shown.bs.tab', function () {
            if (fromMap) google.maps.event.trigger(fromMap, 'resize');
            if (toMap) google.maps.event.trigger(toMap, 'resize');
        });
    });

    // Pre-load streets when editing an existing route
    var fromStreetId = data ? data.dataset.fromStreet : null;
    var toStreetId   = data ? data.dataset.toStreet   : null;

    if (fromStreetId || toStreetId) {
        document.addEventListener('DOMContentLoaded', function () {
            preloadStreet('fromCity', 'fromStreet', fromStreetId);
            preloadStreet('toCity',   'toStreet',   toStreetId);
        });
    }
}

function preloadStreet(citySelectId, streetSelectId, streetId) {
    var citySelect = document.getElementById(citySelectId);
    if (!citySelect || !citySelect.value || !streetId) return;
    loadStreets(citySelectId, streetSelectId).then(function () {
        var streetSelect = document.getElementById(streetSelectId);
        if (streetSelect) streetSelect.value = streetId;
    });
}

function placeFromMarker(latLng) {
    var data = document.getElementById('routeMapData');
    var departureLabel = data ? data.dataset.departureLabel : 'Departure stop';
    if (fromMarker) fromMarker.setMap(null);
    fromMarker = new google.maps.Marker({
        position: latLng,
        map: fromMap,
        icon: 'http://maps.google.com/mapfiles/ms/icons/green-dot.png',
        title: departureLabel
    });
    document.getElementById('fromLat').value = latLng.lat().toFixed(7);
    document.getElementById('fromLng').value = latLng.lng().toFixed(7);
}

function placeToMarker(latLng) {
    var data = document.getElementById('routeMapData');
    var arrivalLabel = data ? data.dataset.arrivalLabel : 'Arrival stop';
    if (toMarker) toMarker.setMap(null);
    toMarker = new google.maps.Marker({
        position: latLng,
        map: toMap,
        icon: 'http://maps.google.com/mapfiles/ms/icons/red-dot.png',
        title: arrivalLabel
    });
    document.getElementById('toLat').value = latLng.lat().toFixed(7);
    document.getElementById('toLng').value = latLng.lng().toFixed(7);
}

function clearFromMarker() {
    if (fromMarker) { fromMarker.setMap(null); fromMarker = null; }
    document.getElementById('fromLat').value = '';
    document.getElementById('fromLng').value = '';
}

function clearToMarker() {
    if (toMarker) { toMarker.setMap(null); toMarker = null; }
    document.getElementById('toLat').value = '';
    document.getElementById('toLng').value = '';
}
