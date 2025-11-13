using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPlantPal.Models
{
    public class Plant
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Species { get; set; }
        public string OwnerUsername { get; set; }
        public DateTime LastWatered { get; set; }
        public int WateringIntervalDays { get; set; }

        // Calculated properties
        public DateTime NextWateringDate => LastWatered.AddDays(WateringIntervalDays);
        public bool NeedsWatering => DateTime.Now >= NextWateringDate;

        // Constructor for JSON deserialization
        public Plant()
        {
            Id = Guid.NewGuid().ToString();     // Generate a new unique identifier
            LastWatered = DateTime.Now;
            WateringIntervalDays = 7; // Default value
        }

        // Constructor for creating new plants
        public Plant(string name, string species, string ownerUsername) : this()
        {
            Name = name;
            Species = species;
            OwnerUsername = ownerUsername;
        }
    }
}
