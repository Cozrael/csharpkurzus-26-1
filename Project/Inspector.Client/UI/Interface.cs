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
        AnsiConsole.MarkupLine($"Succesfully selected: [green]Inspector Menu[/]");
        BackMenu();
        
    }

    public void SettingsMenu()
    {
        AnsiConsole.MarkupLine($"Succesfully selected: [green]Settings Menu[/]");
        
        var selectedRules = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("Please select which rules you would like to apply:")
                .AddChoices("Rule1", "Rule2", "Rule3", "Rule4", "Rule5", "Rule6", "Rule7")
        );
        
        AnsiConsole.WriteLine("You selected: ");
        foreach (var rule in selectedRules)
        {
            AnsiConsole.MarkupLine($"- [green]{rule}[/]");
        }
        
        BackMenu();
    }

    public void Exit()
    {
        AnsiConsole.MarkupLine($"Application is closing. [yellow]See ya![/]");
        Environment.Exit(0);
    }
}