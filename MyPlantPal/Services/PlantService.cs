using MyPlantPal.Models;
using MyPlantPal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPlantPal.Services
{
    public class PlantService
    {
        private const string PlantDataFileName = "plants.json";     // File to store plant data

        private readonly DataStore _dataStore;  // Data storage service
        private List<Plant> _plants;    // In-memory list of plants

        public PlantService(DataStore dataStore)    // Constructor with dependency injection
        {
            _dataStore = dataStore;
            _plants = LoadPlants();
        }

        private List<Plant> LoadPlants()    // Load plants from data store
        {
            return _dataStore.LoadFromFile<Plant>(PlantDataFileName);
        }

        private void SavePlants()
        {
            _dataStore.SaveToFile(PlantDataFileName, _plants);
        }

        public List<Plant> GetPlantTemplates()  // Predefined plant templates
        {
            return new List<Plant>
        {
            //  Easy Care Plants
            new Plant("Snake Plant", "Sansevieria trifasciata", "system") { WateringIntervalDays = 14 },
            new Plant("ZZ Plant", "Zamioculcas zamiifolia", "system") { WateringIntervalDays = 14 },
            new Plant("Pothos", "Epipremnum aureum", "system") { WateringIntervalDays = 7 },
            new Plant("Spider Plant", "Chlorophytum comosum", "system") { WateringIntervalDays = 7 },
            new Plant("Cast Iron Plant", "Aspidistra elatior", "system") { WateringIntervalDays = 10 },
            
            //  Flowering Plants
            new Plant("Peace Lily", "Spathiphyllum", "system") { WateringIntervalDays = 5 },
            new Plant("African Violet", "Saintpaulia", "system") { WateringIntervalDays = 4 },
            new Plant("Orchid", "Phalaenopsis", "system") { WateringIntervalDays = 7 },
            new Plant("Geranium", "Pelargonium", "system") { WateringIntervalDays = 5 },
            new Plant("Begonia", "Begonia", "system") { WateringIntervalDays = 4 },
            
            //  Popular Foliage
            new Plant("Monstera", "Monstera deliciosa", "system") { WateringIntervalDays = 7 },
            new Plant("Fiddle Leaf Fig", "Ficus lyrata", "system") { WateringIntervalDays = 7 },
            new Plant("Rubber Plant", "Ficus elastica", "system") { WateringIntervalDays = 7 },
            new Plant("Chinese Money Plant", "Pilea peperomioides", "system") { WateringIntervalDays = 6 },
            new Plant("String of Pearls", "Senecio rowleyanus", "system") { WateringIntervalDays = 10 },
            
            //  Succulents & Cacti
            new Plant("Aloe Vera", "Aloe barbadensis", "system") { WateringIntervalDays = 14 },
            new Plant("Jade Plant", "Crassula ovata", "system") { WateringIntervalDays = 10 },
            new Plant("Echeveria", "Echeveria", "system") { WateringIntervalDays = 10 },
            new Plant("Christmas Cactus", "Schlumbergera", "system") { WateringIntervalDays = 7 },
            new Plant("Burro's Tail", "Sedum morganianum", "system") { WateringIntervalDays = 10 },
            
            //  Herbs
            new Plant("Basil", "Ocimum basilicum", "system") { WateringIntervalDays = 3 },
            new Plant("Mint", "Mentha", "system") { WateringIntervalDays = 3 },
            new Plant("Rosemary", "Rosmarinus officinalis", "system") { WateringIntervalDays = 5 },
            new Plant("Thyme", "Thymus vulgaris", "system") { WateringIntervalDays = 5 },
            new Plant("Parsley", "Petroselinum crispum", "system") { WateringIntervalDays = 4 },
            
            //  Low Light Plants
            new Plant("Philodendron", "Philodendron hederaceum", "system") { WateringIntervalDays = 7 },
            new Plant("Dracaena", "Dracaena marginata", "system") { WateringIntervalDays = 10 },
            new Plant("Calathea", "Calathea", "system") { WateringIntervalDays = 5 },
            new Plant("English Ivy", "Hedera helix", "system") { WateringIntervalDays = 6 },
            new Plant("Fern", "Nephrolepis exaltata", "system") { WateringIntervalDays = 4 }
        };
        }

        public Plant CreatePlantFromTemplate(Plant template, string ownerUsername)
        {
            var plant = new Plant(template.Name, template.Species, ownerUsername)
            {
                WateringIntervalDays = template.WateringIntervalDays
            };
            return plant;
        }

        public bool AddPlant(Plant plant)
        {
            if (string.IsNullOrWhiteSpace(plant.Name) || string.IsNullOrWhiteSpace(plant.OwnerUsername))
            {
                return false; // Invalid plant data
            }

            _plants.Add(plant);
            SavePlants();
            return true;
        }

        public bool RemovePlant(string plantId, string ownerUsername)
        {
            var plant = _plants.FirstOrDefault(p => p.Id == plantId && p.OwnerUsername == ownerUsername);

            if (plant != null)
            {
                _plants.Remove(plant);
                SavePlants();
                return true;
            }

            return false; // Plant not found or doesn't belong to user
        }

        public List<Plant> GetUserPlants(string username)
        {
            return _plants.Where(p => p.OwnerUsername.ToLower() == username.ToLower())
                         .ToList();
        }

        public List<Plant> GetPlantsNeedingWater(string username)
        {
            return _plants.Where(p => p.OwnerUsername.ToLower() == username.ToLower() && p.NeedsWatering)
                         .ToList();
        }

        public bool WaterPlant(string plantId, string ownerUsername)
        {
            var plant = _plants.FirstOrDefault(p => p.Id == plantId && p.OwnerUsername == ownerUsername);

            if (plant != null)
            {
                plant.LastWatered = DateTime.Now;
                SavePlants();
                return true;
            }

            return false;
        }

        // Helper method for statistics
        public int GetPlantCountForUser(string username)
        {
            return _plants.Count(p => p.OwnerUsername.ToLower() == username.ToLower());
        }
        // Helper to get plant by ID
        public Plant GetPlantById(string plantId, string ownerUsername)
        {
            return _plants.FirstOrDefault(p => p.Id == plantId && p.OwnerUsername == ownerUsername);
        }
        
        public Plantstatistics GetPlantStatistics(string username)  // Generate statistics for user's plants
        {
            var userPlants = GetUserPlants(username);
            var stats = new Plantstatistics();

            if (userPlants.Count == 0)
                return stats;

            stats.TotalPlants = userPlants.Count;   
            stats.PlantsNeedingWater = userPlants.Count(p => p.NeedsWatering);
            stats.PlantsWateredToday = userPlants.Count(p => p.LastWatered.Date == DateTime.Today);
            stats.AverageWateringInterval = Math.Round(userPlants.Average(p => p.WateringIntervalDays), 1); // Rounded to 1 decimal place

            // Find most common plant species
            var mostCommon = userPlants
                .GroupBy(p => p.Species)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();

            stats.MostCommonPlantType = mostCommon?.Key ?? "None";  

            // Find plants needing most frequent watering (most thirsty)
            stats.MostThirstyPlantCount = userPlants
                .Where(p => p.WateringIntervalDays <= 3)
                .Count();

            // Find healthiest plants (watered on time - not overdue)
            stats.HealthiestPlantCount = userPlants
                .Where(p => !p.NeedsWatering)
                .Count();

            return stats;
        }

        public List<Plant> GetPlantsByWateringFrequency(string username)    // Plants sorted by watering frequency
        {
            return GetUserPlants(username)
                .OrderBy(p => p.WateringIntervalDays)
                .ToList();
        }

        public List<Plant> GetOverduePlants(string username)    // Plants that are overdue for watering
        {
            return GetUserPlants(username)
                .Where(p => p.NeedsWatering)
                .OrderByDescending(p => (DateTime.Now - p.NextWateringDate).TotalDays)
                .ToList();
        }

        public Dictionary<string, int> GetPlantsBySpecies(string username)  // Count of plants by species
        {
            return GetUserPlants(username)
                .GroupBy(p => p.Species)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public Plant GetMostThirstyPlant(string username)   // Plant that needs watering most frequently
        {
            return GetUserPlants(username)
                .OrderBy(p => p.WateringIntervalDays)
                .FirstOrDefault();
        }

        public Plant GetMostNeglectedPlant(string username)     // Plant that is most neglected (most overdue)
        {
            return GetOverduePlants(username)
                .FirstOrDefault();
        }
    }
}
