using System;
using System.Text.Json.Serialization;

namespace MyPlantPal.Models
{
    public class Plant
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Species { get; set; }
        public string OwnerUsername { get; set; }
        public DateTime LastWatered { get; set; }
        public int WateringIntervalDays { get; set; } // Base interval (standard for Spring/Autumn)

        // --- SEASONAL LOGIC ---

        public DateTime NextWateringDate
        {
            get
            {
                int currentMonth = DateTime.Now.Month;
                double multiplier = 1.0;

                // WINTER (November - February): Plants are dormant, water less frequently.
                // Increase the interval by 1.5x (e.g., 10 days becomes 15 days).
                if (currentMonth >= 11 || currentMonth <= 2)
                {
                    multiplier = 1.5;
                }
                // SUMMER (June - August): Hot weather.
                // Currently kept at standard rate (1.0), but can be decreased (e.g., 0.8) for more frequent watering.
                else if (currentMonth >= 6 && currentMonth <= 8)
                {
                    multiplier = 1.0;
                }

                // Calculate the actual interval based on the season
                int realInterval = (int)(WateringIntervalDays * multiplier);

                return LastWatered.AddDays(realInterval);
            }
        }

        // Calculated property: Is it time to water the plant now?
        public bool NeedsWatering => DateTime.UtcNow >= NextWateringDate;

        // --- Constructors ---

        public Plant()
        {
            Id = Guid.NewGuid().ToString();
            LastWatered = DateTime.UtcNow;
            WateringIntervalDays = 7; // Default
        }

        public Plant(string name, string species, string ownerUsername) : this()
        {
            Name = name;
            Species = species;
            OwnerUsername = ownerUsername;
        }
    }
}
