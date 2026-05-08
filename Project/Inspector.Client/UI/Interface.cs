using Inspector.Core.Rule;

namespace Inspector.Client.UI;

using Spectre.Console;

public class Interface
{
    
    //  =================
    //      Main Menu
    //  =================
    
    public void MainMenu()
    {

        var main_menu = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Please select a menu.")
                .AddChoices("Main Menu", "Inspector Menu", "Rule Menu", "Exit")
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
            case "Rule Menu":
                AnsiConsole.Clear();
                RulesMenu();
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

    //  =================
    //      Back Menu
    //  =================
    
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

    //  ======================
    //      Inspector Menu
    //  ======================

    public void InspectorMenu()
    {
        AnsiConsole.MarkupLine($"Successfully selected: [green]Inspector Menu[/]");
        BackMenu();

    }
    
    
    //  ===================
    //      Rules Menu
    //  ===================
    
    private List<IRule> selectedList = [new Rule1()];  //Kiválasztott szabályok listája

    public void RulesMenu()
    {


        var applyableRules = new IRule[] { new Rule1(), new Rule2(), new Rule3() }; //Beállítható szabályok listája

        AnsiConsole.MarkupLine($"Successfully selected: [green]Rule Menu[/]");

        var selectedRules = new MultiSelectionPrompt<IRule>()
            .Title("Modify the rules below:")
            .PageSize(Math.Max(3, applyableRules.Length))
            .UseConverter(rule => rule.Name)
            .AddChoices(applyableRules);

        foreach (var rule in applyableRules)
        {
            if (this.selectedList.Any(r => r.Name == rule.Name))
            {
                selectedRules.Select(rule);
            }
        }
        
        var chosenRules = AnsiConsole.Prompt(selectedRules);
        
        //Itt történik meg a hozzáadás
        foreach (var rule in applyableRules)
        {
            if (chosenRules.Any(r => r.Name == rule.Name) && !selectedList.Any(r => r.Name == rule.Name))
            {
                this.selectedList.Add(rule);
            } else if (!chosenRules.Any(r => r.Name == rule.Name) && selectedList.Any(r => r.Name == rule.Name))
            {
                this.selectedList.RemoveAll(r => r.Name == rule.Name);
            }
        }
        
        //Visszajelzés
        AnsiConsole.MarkupLine("You added the following rules:");
        foreach (var rule in chosenRules)
        {
            AnsiConsole.MarkupLine($"- [green]{rule.Name}[/]");
        }
        
        BackMenu();
    }
    
    //  =================
    //      Exit Menu
    //  =================

    public void Exit()
    {
        AnsiConsole.MarkupLine($"Application is closing. [yellow]See ya![/]");
        Environment.Exit(0);
    }
}