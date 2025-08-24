<script>
        // JavaScript functions for filtering properties
        function filterByLocation() {
            const location = document.getElementById('locationFilter').value;
            // Add logic to filter properties by location
            console.log('Filtering by location:', location);
        }

        function applyFilters() {
            const location = document.getElementById('locationFilter').value;
            const priceRange = document.getElementById('priceFilter').value;
            const amenity = document.getElementById('amenitiesFilter').value;

            // Add logic to apply all filters
            console.log('Applying filters:', { location, priceRange, amenity });
        }

        // Example function to display properties (replace with actual data)
        function displayProperties(properties) {
            const propertyList = document.getElementById('propertyList');
            propertyList.innerHTML = properties.map(property => `
                <div class="property-card">
                    <div class="property-image" style="background-image: url('${property.image}')"></div>
                    <div class="property-info">
                        <h3>${property.name}</h3>
                        <p><i class="fas fa-map-marker-alt"></i> ${property.location}</p>
                        <p class="property-price">₹${property.price} per night</p>
                        <p><i class="fas fa-users"></i> Up to ${property.guests} guests</p>
                        <p><i class="fas fa-list"></i> ${property.amenities.join(', ')}</p>
                    </div>
                </div>
            `).join('');
        }

        // Example data (replace with data from your backend)
        const properties = [
            {
                id: 1,
                name: "Luxury Farmhouse Villa",
                location: "Lonavala",
                price: 8000,
                image: "https://images.unsplash.com/photo-1505843513577-22bb7d21e455",
                amenities: ["pool", "garden", "bbq"],
                guests: 6
            },
            {
                id: 2,
                name: "Riverside Retreat",
                location: "Karjat",
                price: 12000,
                image: "https://images.unsplash.com/photo-1571055107559-3e67626fa8be",
                amenities: ["pool", "garden, "spa"],
                guests: 8
            },
            {
                id: 3,
                name: "Green Valley Farm",
                location: "Alibaug",
                price: 4500,
                image: "https://images.unsplash.com/photo-1518780664697-55e3ad937233",
                amenities: ["garden", "bbq"],
                guests: 4
            }
        ];

        // Display properties on page load
        displayProperties(properties);
    </script>