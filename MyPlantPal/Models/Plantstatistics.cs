using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPlantPal.Models
{
    public class Plantstatistics    // Class to hold statistics about plants
    {
        public int TotalPlants { get; set; }
        public int PlantsNeedingWater { get; set; }
        public int PlantsWateredToday { get; set; }
        public double AverageWateringInterval { get; set; }
        public string MostCommonPlantType { get; set; }
        public int MostThirstyPlantCount { get; set; } // Plants needing water every 1-3 days
        public int HealthiestPlantCount { get; set; } // Plants that are watered on time

        public Plantstatistics()    // Default constructor
        {
            TotalPlants = 0;
            PlantsNeedingWater = 0;
            PlantsWateredToday = 0;
            AverageWateringInterval = 0;
            MostCommonPlantType = "None";
            MostThirstyPlantCount = 0;
            HealthiestPlantCount = 0;
        }
    }
}
