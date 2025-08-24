function applyFilters() {
    const location = document.getElementById("locationFilter").value;
    const price = document.getElementById("priceFilter").value;
    const amenities = document.getElementById("amenitiesFilter").value;

    fetch(`/FarmHouse/Filter?location=${location}&price=${price}&amenities=${amenities}`)
        .then(response => response.text())
        .then(html => {
            document.getElementById("propertyList").innerHTML = html;
        })
        .catch(error => {
            console.error("Error fetching filtered properties:", error);
        });
}
