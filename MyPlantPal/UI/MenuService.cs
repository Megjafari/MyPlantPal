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
            AnsiConsole.Clear();
            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green] Main Menu[/]")
                    .PageSize(10)
                    .AddChoices(new[] {
                        "My Plants",
                        "Add Plants",
                        "Watering Tasks",
                        "Statistics",
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

        // Method used by AppController for registration
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

        // Method used by AppController to log in
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

        // Form for adding a custom plant
        public (string name, string species, int interval) ShowCustomPlantForm()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(
                new FigletText("Custom Plant")
                    .LeftJustified()
                    .Color(Color.Blue));

            var name = AnsiConsole.Ask<string>("[yellow]Plant name:[/]");
            var species = AnsiConsole.Ask<string>("[yellow]Plant species:[/]");
            var interval = AnsiConsole.Ask<int>("[yellow]Watering interval (days):[/]");

            return (name, species, interval);
        }

        // --- 3. Displaying data and messages ---

        public Plant? ShowPlantTemplates(List<Plant> templates)
        {
            var templateOptions = templates.Select(t =>
                $"{t.Name} - {t.Species} ( every {t.WateringIntervalDays} days)").ToList();
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
            table.AddColumn(new TableColumn("[yellow]Last Watered[/]").Centered());
            table.AddColumn(new TableColumn("[yellow]Next Watering[/]").Centered());
            table.AddColumn(new TableColumn("[yellow]Status[/]").Centered());

            foreach (var plant in plants)
            {
                var status = plant.NeedsWatering ?
                    new Markup("[red] NEEDS WATER[/]") :
                    new Markup("[green] OK[/]");

                table.AddRow(
                    new Text(plant.Name),
                    new Text(plant.Species),
                    new Text(plant.LastWatered.ToString("yyyy-MM-dd")),
                    new Text(plant.NextWateringDate.ToString("yyyy-MM-dd")),
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
                var daysOverdue = (DateTime.Now - plant.NextWateringDate).Days;
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

        public string AskForPlantToWater(List<Plant> plantsNeedingWater)
        {
            var plantOptions = plantsNeedingWater.Select(p => p.Name).ToList();
            plantOptions.Add(" Back");

            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Select plant to water:[/]")
                    .AddChoices(plantOptions));
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
        public string AskForPlantToRemove(List<Plant> plants)
        {
            if (plants.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No plants available to remove![/]");
                return null;
            }

            var plantOptions = plants.Select(p => p.Name).ToList();
            plantOptions.Add(" Back");

            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[red]Select plant to remove:[/]")
                    .AddChoices(plantOptions));
        }
    }
}