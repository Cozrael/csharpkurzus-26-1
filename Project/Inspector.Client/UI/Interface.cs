using System.ComponentModel;

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

        var sensedDummies = new[] //Teszt adatok
        {
            ("12:01:03", "192.168.1.105:4231", "TCP",  "Blacklist hit"),
            ("12:01:07", "10.0.0.44:80", "TCP", "SYN Flood"),
            ("12:01:09", "185.220.101.3:443",  "TCP", "Blacklist hit"),
            ("12:01:12", "192.168.1.1:53", "UDP", "Port Scan"),
            ("12:01:15", "172.16.0.2:8080", "TCP", "SYN Flood"),
        }; //TODO: Valós adatok tömbje

        var table = new Table()
            .AddColumn("Id")
            .AddColumn("Time")
            .AddColumn("Source IP:Port")
            .AddColumn("Protocol")
            .AddColumn("Reason");

        AnsiConsole.Live(table)
            .Overflow(VerticalOverflow.Crop)
            .Start(ctx =>
            {
                var id = 1;
                
                foreach (var (time, sourceIp, protocol, reason) in sensedDummies)
                {
                    table.AddRow(id++.ToString(), time, sourceIp, protocol, reason);
                    ctx.Refresh();
                    Thread.Sleep(new Random().Next(500, 2000)); //Szimuláljuk hogy úgymond random időbe jönnek az alertek
                }
                //TODO: Valós alertek kiírása
                
            });

        /*
        BackMenu();
        */

    }
    
    
    //  ===================
    //      Rules Menu
    //  ===================
    
    private List<IRule> activeRules = [];  //Kiválasztott szabályok listája
    private RuleManager ruleManager = new RuleManager();

    public void RulesMenu()
    {


        var availableRules = new IRule[] {new RuleDefault(), new Rule1(), new Rule2(), new Rule3() }; //Beállítható szabályok listája

        AnsiConsole.MarkupLine($"Successfully selected: [green]Rule Menu[/]");

        var rulesPrompt = new MultiSelectionPrompt<IRule>()
            .Title("Modify the rules below:")
            .PageSize(Math.Max(3, availableRules.Length))
            .NotRequired()
            .UseConverter(rule => rule.Name)
            .AddChoices(availableRules);
            

        //Vizsgáljuk hogy benne van-e már,
        foreach (var rule in availableRules)
        {
            if (ruleManager.ActiveRules.Any(r => r.Name == rule.Name))
            {
                rulesPrompt.Select(rule); //Ha igen -> pipáljuk
            }
            
        }
        
        var chosenRules = AnsiConsole.Prompt(rulesPrompt);
        
        //Itt történik meg a hozzáadás
        ruleManager.UpdateActiveRules(availableRules, chosenRules);
        
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