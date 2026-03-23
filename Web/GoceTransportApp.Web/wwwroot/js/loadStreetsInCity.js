// loadStreetsInCity.js
// Fetches streets for a selected city and populates a street <select>.
// Shows a disabled loading state on the street dropdown while the request is in-flight.

async function loadStreets(citySelectId, streetSelectId) {
    const cityId = document.getElementById(citySelectId).value;
    const streetSelect = document.getElementById(streetSelectId);

    if (!cityId) {
        streetSelect.innerHTML = '<option value="">Select a city first</option>';
        streetSelect.disabled = true;
        return;
    }

    // ── Loading state ──────────────────────────────────────
    streetSelect.innerHTML = '<option value="">Loading streets...</option>';
    streetSelect.disabled = true;

    try {
        const response = await fetch(`https://localhost:7119/Streets/GetStreetsByCity/${cityId}`);

        if (response.ok) {
            const streets = await response.json();

            if (streets.length === 0) {
                streetSelect.innerHTML = '<option value="">No streets found for this city</option>';
            } else {
                streetSelect.innerHTML = '<option value="">Select a street</option>';
                streets.forEach(function (street) {
                    const option = document.createElement('option');
                    option.value = street.id;
                    option.textContent = street.name;
                    streetSelect.appendChild(option);
                });
            }
        } else {
            streetSelect.innerHTML = '<option value="">Failed to load streets</option>';
        }
    } catch (error) {
        console.error('Error fetching streets:', error);
        streetSelect.innerHTML = '<option value="">Error loading streets</option>';
    } finally {
        // ── Re-enable the dropdown regardless of outcome ───
        streetSelect.disabled = false;
    }
}
