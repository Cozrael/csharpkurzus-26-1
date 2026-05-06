using System.Net.Mime;

namespace Inspector.Client.UI;

using Spectre.Console;

public class Interface
{
    public void MainMenu()
    {

        var main_menu = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Please select a menu.")
                .AddChoices("Main Menu","Inspector Menu", "Settings Menu", "Exit")
        );

        switch (main_menu)
        {
            case "Main Menu":
                AnsiConsole.Clear();
                MainMenu();
                break;
            case "Inspector Menu":
                AnsiConsole.Clear();
                InspectorMenu();
                break;
            case "Settings Menu":
                AnsiConsole.Clear();
                SettingsMenu();
                break;
            case "Exit":
                AnsiConsole.Clear();
                Exit();
                break;
            default:
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine($"[yellow]Choose a valid menu![/]");
                break;
        }

    }

    public void BackMenu()
    {
        var back_menu = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .AddChoices("Back to Main Menu")
        );
        
        switch (back_menu)
        {
            case "Back to Main Menu":
                AnsiConsole.Clear();
                MainMenu();
                break;
            default:
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine($"[yellow]Choose a valid menu![/]");
                break;
        }
    }

    public void InspectorMenu()
    {
        AnsiConsole.MarkupLine($"Successfully selected: [green]Inspector Menu[/]");
        BackMenu();
        
    }

    public void SettingsMenu()
    {

        List<String> selectedList = new List<String>(); //Kiválasztott szabályok listája

        var applyableRules = new[] { "Rule1", "Rule2", "Rule3", "Rule4", }; //Beállítható szabályok listája
        
        AnsiConsole.MarkupLine($"Successfully selected: [green]Settings Menu[/]");

        var selectedRules = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("Please select which rules you would like to apply:")
                .PageSize(applyableRules.Length)
                .AddChoices(applyableRules)
                
            //TODO: A listában szereplő szabályok már alapból be vannak pipálva és ki leeht őket szedni a listából.
            //TODO: Ha a user véletlen lép be a beállításokba akkor vissza tudjon lépni a main menübe és ezzel ne változzón a jelenlegi beállítás.
            
            
        );
        
        
        AnsiConsole.WriteLine("You selected: ");
        selectedList.Clear();
        foreach (var rule in selectedRules)
        {
            AnsiConsole.MarkupLine($"- [green]{rule}[/]");
            selectedList.Add(rule);
        }
        
        BackMenu();
    }

    public void Exit()
    {
        AnsiConsole.MarkupLine($"Application is closing. [yellow]See ya![/]");
        Environment.Exit(0);
    }
}