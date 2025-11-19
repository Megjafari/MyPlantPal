using MyPlantPal.Services;
using MyPlantPal.UI;

namespace MyPlantPal
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataStore = new DataStore();

            var userService = new UserService(dataStore);
            var plantService = new PlantService(dataStore);

            var menuService = new MenuService();

            var appController = new AppController(menuService, userService, plantService);
            appController.Run();
        }
    }
}