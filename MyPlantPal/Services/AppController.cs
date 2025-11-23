using MyPlantPal.Models;
using MyPlantPal.Services;
using MyPlantPal.UI;
using Spectre.Console;
using System;
using System.Linq;
using System.Threading;

namespace MyPlantPal
{
    public class AppController
    {
        private readonly MenuService _menuService;
        private readonly UserService _userService;
        private readonly PlantService _plantService;
        private User? _currentUser;

        // Constructor: Receives all necessary services via Dependency Injection (DI)
        public AppController(MenuService menuService, UserService userService, PlantService plantService)
        {
            _menuService = menuService;
            _userService = userService;
            _plantService = plantService;
            _currentUser = null;
        }

        // --- 1. Main Application Entry Point ---

        public void Run()
        {
            string choice;

            // Loop for the initial Start Menu (Login/Register/Exit)
            while (true)
            {
                choice = _menuService.ShowStartMenu();

                if (choice == "Exit") break;

                if (choice == "Register")
                {
                    HandleRegistration();
                }
                else if (choice == "Login")
                {
                    if (HandleLogin())
                    {
                        ShowMainMenuLoop(); // Move to Main Menu upon successful login
                    }
                }
            }
            _menuService.ShowSuccessMessage("Exiting MyPlantPal. Goodbye!");
        }

        // --- 2. Login and Registration Handlers ---

        private void HandleRegistration()
        {
            AnsiConsole.Clear();
            var (username, password) = _menuService.ShowRegistrationForm();

            if (_userService.Register(username, password))
            {
                _menuService.ShowSuccessMessage($"User {username} registered successfully!");
            }
            else
            {
                _menuService.ShowErrorMessage("Registration failed. Username already exists.");
            }
        }

        private bool HandleLogin()
        {
            AnsiConsole.Clear();
            var (username, password) = _menuService.ShowLoginForm();

            var user = _userService.Login(username, password);
            if (user != null)
            {
                _currentUser = user;

                // --- PROGRESS SIMULATION (Optional loading screen) ---
                AnsiConsole.Progress()
                    .Start(ctx =>
                    {
                        var task1 = ctx.AddTask("[green]Authenticating user data[/]");
                        var task2 = ctx.AddTask("[green]Loading Your Virtual Garden[/]");

                        // Simulate work until both tasks are complete
                        while (!ctx.IsFinished)
                        {
                            task1.Increment(0.9);
                            task2.Increment(0.5);
                            Thread.Sleep(8);
                        }
                    });
                // --------------------------------------------------

                _menuService.ShowSuccessMessage($"Welcome back, {username}!");
                return true;
            }
            else
            {
                _menuService.ShowErrorMessage("Login failed. Invalid credentials.");
                return false;
            }
        }

        // --- 3. Main Menu Loop and Navigation ---

        private void ShowMainMenuLoop()
        {
            string choice;
            while (_currentUser != null)
            {
                // CLEAR SCREEN AND DISPLAY WELCOME HEADER
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine($"[bold green]Welcome back, {_currentUser.Username}![/]");
                AnsiConsole.WriteLine();
                // --------------------------------------------

                // REMOVED: DisplayUserStats() is now removed from the main loop!

                // Display menu options
                choice = _menuService.ShowMainMenu();

                switch (choice)
                {
                    case "My Plants":
                        HandleShowAllPlants();
                        break;
                    case "Add Plants":
                        HandleAddPlantWorkflow();
                        break;
                    case "Watering Tasks":
                        HandleWateringTasks();
                        break;
                    case "Statistics":
                        // FIX: Statistics are now displayed only when requested
                        AnsiConsole.Clear();
                        AnsiConsole.MarkupLine($"[bold green]Statistics for {_currentUser.Username}[/]");
                        AnsiConsole.WriteLine();
                        DisplayUserStats();
                        break;
                    case "Logout":
                        _currentUser = null;
                        _menuService.ShowSuccessMessage("Logged out successfully.");
                        break;
                }

                if (_currentUser != null)
                {
                    _menuService.WaitForContinue();
                }
            }
        }

        // --- 4. Plant Management and UI Methods ---

        private void DisplayUserStats()
        {
            if (_currentUser == null) return;
            // Get stats from PlantService and display them using MenuService
            var stats = _plantService.GetPlantStatistics(_currentUser.Username);
            _menuService.DisplayStatistics(stats);
        }

        private void HandleShowAllPlants()
        {
            if (_currentUser == null) return;
            AnsiConsole.Clear();
            var plants = _plantService.GetUserPlants(_currentUser.Username);
            _menuService.DisplayPlantsTable(plants); // Show the table

            if (plants.Count > 0)
            {
                // Option to remove a plant after viewing the list
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Options[/]")
                        .AddChoices(new[] {
                    "Remove Plant",
                    "Back to Main Menu"
                        }));

                if (choice == "Remove Plant")
                {
                    HandleRemovePlant(plants); // Call the remove handler
                }
            }
        }

        private void HandleAddPlantWorkflow()
        {
            if (_currentUser == null) return;
            string choice;

            while (true)
            {
                AnsiConsole.Clear();
                choice = _menuService.ShowAddPlantMenu();

                if (choice == "Back to main menu") return;

                if (choice == "Choose from plant library")
                {
                    HandleAddPlantFromTemplate();
                }
                else if (choice == "Add custom plant")
                {
                    HandleAddCustomPlant();
                }

                _menuService.WaitForContinue();
            }
        }

        private void HandleAddPlantFromTemplate()
        {
            var templates = _plantService.GetPlantTemplates();
            var selectedTemplate = _menuService.ShowPlantTemplates(templates);

            if (selectedTemplate != null)
            {
                // Add the plant to the user's data (saves to plants.json)
                bool success = _plantService.AddPlant(
                    selectedTemplate.Name,
                    selectedTemplate.Species,
                    _currentUser!.Username,
                    selectedTemplate.WateringIntervalDays
                );

                if (success)
                {
                    _menuService.ShowSuccessMessage($"Plant '{selectedTemplate.Name}' added and saved!");
                }
                else
                {
                    _menuService.ShowErrorMessage("Failed to add plant from template.");
                }
            }
        }

        private void HandleAddCustomPlant()
        {
            var (name, species, interval) = _menuService.ShowCustomPlantForm();

            // Add the custom plant
            bool success = _plantService.AddPlant(
                name,
                species,
                _currentUser!.Username,
                interval
            );

            if (success)
            {
                _menuService.ShowSuccessMessage($"Custom plant '{name}' added and saved!");
            }
            else
            {
                _menuService.ShowErrorMessage("Failed to add custom plant. Check input data.");
            }
        }

        private void HandleWateringTasks()
        {
            if (_currentUser == null) return;
            AnsiConsole.Clear();

            // Get plants that need watering
            var plantsNeedingWater = _plantService.GetPlantsNeedingWater(_currentUser.Username);
            _menuService.DisplayWateringTasks(plantsNeedingWater);

            if (plantsNeedingWater.Count > 0)
            {
                var plantNameChoice = _menuService.AskForPlantToWater(plantsNeedingWater);

                if (plantNameChoice == null || plantNameChoice == " Back") return;

                var plantToWater = plantsNeedingWater.FirstOrDefault(p => p.Name == plantNameChoice);

                if (plantToWater != null)
                {
                    // Call the service method to update LastWatered and save the file
                    bool success = _plantService.WaterPlant(plantToWater.Id, _currentUser.Username);

                    if (success)
                    {
                        _menuService.ShowSuccessMessage($"Successfully watered {plantToWater.Name}! Date updated.");
                    }
                    else
                    {
                        _menuService.ShowErrorMessage("Error during watering. Could not save changes.");
                    }
                }
            }

        }

        private void HandleRemovePlant(List<Plant> currentPlants)
        {
            if (_currentUser == null) return;

            // 1. Get the user's choice from MenuService
            var plantNameChoice = _menuService.AskForPlantToRemove(currentPlants);

            // Check if the user selected "Back" or if the list was empty
            if (string.IsNullOrEmpty(plantNameChoice) || plantNameChoice == " Back") return;

            // 2. Find the plant object by name to get its unique ID
            var plantToRemove = currentPlants.FirstOrDefault(p => p.Name == plantNameChoice);

            if (plantToRemove != null)
            {
                // 3. Request confirmation
                var confirmed = AnsiConsole.Confirm($"[red]Are you sure you want to permanently remove {plantToRemove.Name}?[/]");

                if (confirmed)
                {
                    // 4. Call PlantService to remove the plant
                    bool success = _plantService.RemovePlant(plantToRemove.Id, _currentUser.Username);

                    if (success)
                    {
                        _menuService.ShowSuccessMessage($"Plant '{plantToRemove.Name}' removed successfully.");
                    }
                    else
                    {
                        _menuService.ShowErrorMessage("Error: Could not remove plant.");
                    }
                }
                else
                {
                    _menuService.ShowErrorMessage("Removal cancelled.");
                }
            }
        }
    }
}