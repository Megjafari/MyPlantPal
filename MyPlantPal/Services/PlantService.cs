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


        public List<Plant> GetPlantTemplates()
        {
            //load plant templates from templates.json
            return _dataStore.LoadFromFile<Plant>("templates.json");
        }



        public Plant CreatePlantFromTemplate(Plant template, string ownerUsername)
        {
            var plant = new Plant(template.Name, template.Species, ownerUsername)
            {
                WateringIntervalDays = GetSeasonAdjustedInterval(template.WateringIntervalDays)
            };
            return plant;
        }

        public bool AddPlant(string testPlantName, string v, Plant plant)
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

        public bool AddPlant(string name, string species, string ownerUsername, int wateringIntervalDays)
        {
            // 1. Create the new Plant object
            var plant = new Plant(name, species, ownerUsername)
            {
                WateringIntervalDays = wateringIntervalDays,
            };

            // Simple validation
            if (string.IsNullOrWhiteSpace(plant.Name) || string.IsNullOrWhiteSpace(plant.OwnerUsername))
            {
                return false;
            }

            // 2. Add to list and save
            _plants.Add(plant);
            SavePlants();
            return true;
        }
        private int GetSeasonAdjustedInterval(int baseInterval)
        {
            var currentMonth = DateTime.Now.Month;
            // WINTER (Nov-Feb): 50% Less watering
            if (currentMonth >= 11 || currentMonth <= 2)
            {
                return (int)(baseInterval * 1.5);
            }
            return baseInterval;
        }
    }
}
