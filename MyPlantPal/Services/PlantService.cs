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
    }
}
