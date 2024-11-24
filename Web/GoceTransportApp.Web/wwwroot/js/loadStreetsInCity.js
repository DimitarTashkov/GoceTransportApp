async function loadStreets(citySelectId, streetSelectId) {
    const cityId = document.getElementById(citySelectId).value;
    const streetSelect = document.getElementById(streetSelectId);

    // Clear existing streets
    streetSelect.innerHTML = '<option value="">Loading...</option>';

    if (cityId) {
        try {
            const response = await fetch(`https://localhost:7119/Streets/GetStreetsByCity/${cityId}`);
            if (response.ok) {
                const streets = await response.json();
                streetSelect.innerHTML = '<option value="">Select a street</option>';
                streets.forEach(street => {
                    const option = document.createElement('option');
                    option.value = street.id;
                    option.textContent = street.name;
                    streetSelect.appendChild(option);
                });
            } else {
                streetSelect.innerHTML = '<option value="">Failed to load streets</option>';
            }
        } catch (error) {
            console.error('Error fetching streets:', error);
            streetSelect.innerHTML = '<option value="">Error loading streets</option>';
        }
    } else {
        streetSelect.innerHTML = '<option value="">Select a city first</option>';
    }
}