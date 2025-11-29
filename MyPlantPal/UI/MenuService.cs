using Spectre.Console;
using MyPlantPal.Models; 
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyPlantPal.UI
{
    public class MenuService
    {
        // --- 1. Main navigation menus ---

        public string ShowStartMenu()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(
                new FigletText("MyPlantPal")
                    .LeftJustified()
                    .Color(Color.Green));

            AnsiConsole.MarkupLine("[yellow]Your personal plant care companion![/]");
            AnsiConsole.WriteLine();

            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Choose an option:[/]")
                    .PageSize(10)
                    .AddChoices(new[] {
                        "Register",
                        "Login",
                        "Exit"
                    }));
        }

        public string ShowMainMenu()
        {
            //  Removed AnsiConsole.Clear() to allow AppController to draw the header/stats first.
            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(" ") //  Set title to empty to avoid visual clutter with the header
                    .PageSize(10)
                    .AddChoices(new[] {
                        "My Plants",
                        "Add Plants",
                        "Watering Tasks",
                        "Statistics",
                        "Settings",
                        "Logout"
                    }));
        }

        public string ShowAddPlantMenu()
        {
            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Add Plants[/]")
                    .PageSize(10)
                    .AddChoices(new[] {
                        "Choose from plant library",
                        "Add custom plant",
                        "Back to main menu"
                    }));
        }

        // --- 2. Input Forms (Login/Register/Custom Plant) ---

        public (string username, string password) ShowRegistrationForm()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(
                new FigletText("Register")
                    .LeftJustified()
                    .Color(Color.Green));

            var username = AnsiConsole.Ask<string>("[yellow]Enter username:[/]");
            var password = AnsiConsole.Prompt(
                new TextPrompt<string>("[yellow]Enter password:[/]")
                    .Secret());

            return (username, password);
        }

        public (string username, string password) ShowLoginForm()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(
                new FigletText("Login")
                    .LeftJustified()
                    .Color(Color.Green));

            var username = AnsiConsole.Ask<string>("[yellow]Enter username:[/]");
            var password = AnsiConsole.Prompt(
                new TextPrompt<string>("[yellow]Enter password:[/]")
                    .Secret());

            return (username, password);
        }

        public (string name, string species, int interval, bool goBack) ShowCustomPlantForm()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(
                new FigletText("Custom Plant")
                    .LeftJustified()
                    .Color(Color.Blue));

            // Fråga först om användaren vill fortsätta eller gå tillbaka
            var actionChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]What would you like to do?[/]")
                    .AddChoices(new[] {
                "Add a custom plant",
                "Back to Plant Menu"
                    }));
            
            if (actionChoice == "Back to Plant Menu")
                
                return (string.Empty, string.Empty, 0, true);

            var name = AnsiConsole.Ask<string>("[yellow]Plant name:[/]");
            var species = AnsiConsole.Ask<string>("[yellow]Plant species:[/]");
            var interval = AnsiConsole.Ask<int>("[yellow]Watering interval (days):[/]");

            return (name, species, interval, false);
        }

        // --- 3. Displaying data and messages ---

        // FIX: Returns Plant? to handle null selection.
       
        public Plant? ShowPlantTemplates(List<Plant> templates)
        {
            // Display both standard and winter intervals to inform the user
            var templateOptions = templates.Select(t =>
            $"{t.Name} - {t.Species}").ToList();

            templateOptions.Add("← Back");

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Choose a plant from our library:[/]")
                    .PageSize(15)
                    .AddChoices(templateOptions));

            if (choice == "← Back") return null;

            var selectedIndex = templateOptions.IndexOf(choice);
            return templates[selectedIndex];
        }

        public void DisplayPlantsTable(List<Plant> plants)
        {
            if (plants.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No plants found![/]");
                return;
            }

            var table = new Table();
            table.Border(TableBorder.Rounded);
            table.Title = new TableTitle("[green] Your Plants[/]");

            table.AddColumn(new TableColumn("[yellow]Name[/]").Centered());
            table.AddColumn(new TableColumn("[yellow]Species[/]").Centered());

            // New column to show seasonal schedule changes
            table.AddColumn(new TableColumn("[yellow]Schedule[/]").Centered());

            table.AddColumn(new TableColumn("[yellow]Last Watered[/]").Centered());
            table.AddColumn(new TableColumn("[yellow]Next Watering[/]").Centered());
            table.AddColumn(new TableColumn("[yellow]Status[/]").Centered());

            foreach (var plant in plants)
            {
                // Calculate current active interval
                var currentInterval = (plant.NextWateringDate - plant.LastWatered).Days;
                var baseInterval = plant.WateringIntervalDays;

                string scheduleDisplay;

                if (currentInterval != baseInterval)
                {
                    // Show change if seasonal logic is active
                    scheduleDisplay = $"[blue]{baseInterval} -> {currentInterval} days[/]";
                }
                else
                {
                    scheduleDisplay = $"{baseInterval} days";
                }

                var status = plant.NeedsWatering ?
                    new Markup("[red] NEEDS WATER[/]") :
                    new Markup("[green] OK[/]");

                table.AddRow(
                    new Text(plant.Name),
                    new Text(plant.Species),
                    new Markup(scheduleDisplay), // Add the schedule info here
                    new Text(plant.LastWatered.ToLocalTime().ToString("yyyy-MM-dd")),
                    new Text(plant.NextWateringDate.ToLocalTime().ToString("yyyy-MM-dd")),
                    status
                );
            }

            AnsiConsole.Write(table);
        }

        public void DisplayWateringTasks(List<Plant> plantsNeedingWater)
        {
            if (plantsNeedingWater.Count == 0)
            {
                AnsiConsole.MarkupLine("[green] All your plants are happy! No watering needed.[/]");
                return;
            }

            var table = new Table();
            table.Border(TableBorder.Rounded);
            table.Title = new TableTitle("[red] Plants Needing Water[/]");

            table.AddColumn(new TableColumn("[yellow]Name[/]"));
            table.AddColumn(new TableColumn("[yellow]Species[/]"));
            table.AddColumn(new TableColumn("[yellow]Days Overdue[/]"));
            table.AddColumn(new TableColumn("[yellow]Action[/]"));

            foreach (var plant in plantsNeedingWater)
            {
                var daysOverdue = (DateTime.UtcNow - plant.NextWateringDate).Days;
                table.AddRow(
                    plant.Name,
                    plant.Species,
                    $"{daysOverdue} days",
                    "[blue]Water Now[/]"
                );
            }

            AnsiConsole.Write(table);
        }

        public void DisplayStatistics(Plantstatistics stats)
        {
            var panel = new Panel(
                $@"[bold] Plant Statistics[/]

                [blue]Total Plants:[/] {stats.TotalPlants}
                [red]Need Water:[/] {stats.PlantsNeedingWater}
                [green]Watered Today:[/] {stats.PlantsWateredToday}
                [yellow]Average Interval:[/] {stats.AverageWateringInterval:F1} days
                [purple]Most Common:[/] {stats.MostCommonPlantType}
                [orange1]Thirsty Plants (<=3 days):[/] {stats.MostThirstyPlantCount}
                [springgreen1]Healthy Plants (Not overdue):[/] {stats.HealthiestPlantCount}"
            )
            .Header(" Your Plant Health")
            .BorderColor(Color.Green)
            .Padding(1, 1);

            AnsiConsole.Write(panel);
        }

        // FIX: Returns string? to handle null selection.
        public string? AskForPlantToWater(List<Plant> plantsNeedingWater)
        {
            var plantOptions = plantsNeedingWater.Select(p => p.Name).ToList();
            plantOptions.Add("← Back");

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Select plant to water:[/]")
                    .AddChoices(plantOptions));

            if (choice == "← Back") return null;
            return choice;
        }

        // FIX: Returns string? to handle null selection.
        public string? AskForPlantToRemove(List<Plant> plants)
        {
            if (plants.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No plants available to remove![/]");
                return null;
            }

            var plantOptions = plants.Select(p => p.Name).ToList();
            plantOptions.Add("← Back");

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[red]Select plant to remove:[/]")
                    .AddChoices(plantOptions));

            if (choice == "← Back") return null;
            return choice;
        }

        // --- 4. Messages and waiting for input ---

        public void ShowSuccessMessage(string message)
        {
            AnsiConsole.MarkupLine($"[green] {message}[/]");
        }

        public void ShowErrorMessage(string message)
        {
            AnsiConsole.MarkupLine($"[red] {message}[/]");
        }

        public void WaitForContinue()
        {
            AnsiConsole.WriteLine();
            AnsiConsole.Prompt(
                new TextPrompt<string>("Press [green]Enter[/] to continue...")
                    .AllowEmpty());
        }

        // Shows the settings menu (account deletion, etc.)
        public string ShowSettingsMenu()
        {
            AnsiConsole.Clear();  // Clears the console screen
            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Settings[/]")
                    .AddChoices(new[] { 
                        "Change Password",
                        "Delete Account",  
                        "Back"             
                    }));
        }

        public bool ConfirmDeleteAccount()  //// Shows a warning screen and asks the user to confirm account deletion
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new FigletText("DELETE ACCOUNT").Color(Color.Red));  //// Big red warning delete account 
            AnsiConsole.MarkupLine("[bold red]WARNING: This action cannot be undone![/]");
            AnsiConsole.MarkupLine("[red]All your plants and data will be permanently lost.[/]");
            AnsiConsole.WriteLine();
            //// Returns true if user selects "Yes", false otherwise
            return AnsiConsole.Confirm("Are you sure you want to delete your account?");
        }
        // Asks the user to enter their password before deleting the account
        public string AskPasswordForDeletion() // Prompts user for their password (input is hidden with .Secret())
        { 
            return AnsiConsole.Prompt(
                new TextPrompt<string>("Enter your [red]password[/] to confirm deletion:")
                    .Secret());
        }

        public (string oldPassword, string newPassword) ShowChangePasswordForm()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new FigletText("Change Password").Color(Color.Yellow));
            AnsiConsole.MarkupLine("[yellow]Please enter your current and new password.[/]");
            AnsiConsole.WriteLine();

            var oldPassword = AnsiConsole.Prompt(
                new TextPrompt<string>("[yellow]Current password:[/]")
                    .Secret()); // Hides the input

            var newPassword = AnsiConsole.Prompt(
                new TextPrompt<string>("[yellow]New password:[/]")
                    .Secret()); // Hides the input

            return (oldPassword, newPassword);
        }


    }
}