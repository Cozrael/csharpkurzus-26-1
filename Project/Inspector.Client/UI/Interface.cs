using System.ComponentModel;

using Inspector.Core;
using Inspector.Core.Rule;

using PacketDotNet;

namespace Inspector.Client.UI;

using Spectre.Console;

public class Interface
{
    
    private static readonly RuleEngine _ruleEngine = new RuleEngine();
    private static readonly BlackList _blackList = new BlackList();
    private static readonly TrafficStorage _trafficStorage = new TrafficStorage();

    private static readonly TrafficLogger _trafficLogger = new TrafficLogger(_trafficStorage, _blackList, _ruleEngine);
    private static readonly Packets _PacketCapture = new Packets(_trafficLogger);
    

    
    //  =================
    //      Main Menu
    //  =================
    
    public void MainMenu()
    {
        _PacketCapture.StartCapture();
        

        


        var main_menu = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Please select a menu.")
                .AddChoices("Main Menu", "Inspector Menu", "Rule Menu", "Summeries Menu", "Exit")
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
            case "Summeries Menu":
                AnsiConsole.Clear();
                Summeries();
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


        //TODO: Valós adatok tombje;
        var currentDanger = _trafficStorage.GetCurrentPotentialDanger();
        

        var table = new Table()
            .AddColumn("Id")
            .AddColumn("Time")
            .AddColumn("Source IP:Port")
            .AddColumn("Protocol")
            .AddColumn("Reason");

        bool exitRequested = false;

        var keyListener = Task.Run(() =>
        {
            while (!exitRequested)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(intercept: true);
                    if (key.Key == ConsoleKey.Spacebar)
                        exitRequested = true;
                }
                Thread.Sleep(500);
            }
        });
        
        AnsiConsole.MarkupLine("Press [blue]SPACE[/] to stop!");
        
        AnsiConsole.Live(table)
            .Overflow(VerticalOverflow.Visible)
            .Start(ctx =>
            {
                
                var id = 1;
                var shown = new HashSet<int>(); 

                while (!exitRequested)
                {
                    var currentDanger = _trafficStorage.GetCurrentPotentialDanger(); 

                    foreach (var packet in currentDanger)
                    {
                        int hash = packet.GetHashCode();
                        if (shown.Contains(hash)) continue; 

                        shown.Add(hash);
                        table.AddRow(
                            $"[maroon]{id++.ToString()}[/]",
                            $"[maroon]{packet.Time ?? "-"}[/]",
                            $"[maroon]{packet.SourceAddress}:{packet.SourcePort}[/]",
                            $"[maroon]{packet.Protocol ?? "-"}[/]",
                            $"[red]{packet.PotentialDangerMessage ?? "-"}[/]"
                        );
                        ctx.Refresh();
                    }

                    Thread.Sleep(1000);
                    
                }
                
                
            });

        exitRequested = true;
        BackMenu();

    }
    
    
    //  ===================
    //      Rules Menu
    //  ===================
    
    private List<IRule> activeRules = [];  //Kiválasztott szabályok listája
    private RuleManager ruleManager = new RuleManager();

    public void RulesMenu()
    {


        var availableRules = new IRule[] {new RuleSynFlood(), new RulePortScanDetector(), new RuleHeaderLenght() }; //Beállítható szabályok listája

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
        
        if (chosenRules.Any(r => r.Name == "Header Lenght"))
        {
            _ruleEngine.headLengthOn = true;
        }
        else
        {
            _ruleEngine.headLengthOn = false;
        }
        
        if (chosenRules.Any(r => r.Name == "SYN Flood"))
        {
            _ruleEngine.synAckOn = true;
        }
        else
        {
            _ruleEngine.synAckOn = false;
        }
        
        if (chosenRules.Any(r => r.Name == "Port Scan Detector"))
        {
            _ruleEngine.portScanOn = true;
        }
        else
        {
            _ruleEngine.portScanOn = false;
        }
        
        BackMenu();
    }

    public void Summeries()
    {
        AnsiConsole.MarkupLine($"Successfully selected: [green]Summeries[/]");
        
        ReadSummarys summary = new ReadSummarys();

        string[] files = summary.ListAllSummaries();

        if (files.Length == 0 ||  files == null)
        {
            AnsiConsole.MarkupLine($"[red]No Summaries![/]");
            BackMenu();
        }
        
        var selectedFile = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Choose a Summary to read!")
                .AddChoices(files)
        );
        
        List<PacketData> packets = summary.ReadSummary(selectedFile);

        if (packets.Count == 0 || packets == null)
        {
            AnsiConsole.MarkupLine($"[red]No Packets![/]");
        }

        var table = new Table();
        table.AddColumn("Source IP");
        table.AddColumn("Destination IP");
        table.AddColumn("Flag");
        table.AddColumn("Potential Danger");
        table.AddColumn("Reason");


        AnsiConsole.Live(table)
            .Overflow(VerticalOverflow.Ellipsis)
            .Start(ctx =>
            {
                foreach (var packet in packets)
                {
                    
                    table.AddRow(
                        $"[cyan]{packet.SourceAddress ?? "-"}[/]",
                        $"[cyan]{packet.DestinationAddress ?? "-"}[/]",
                        $"[cyan]{packet.Flags ?? "No Flag"}[/]",
                        packet.PotentialDanger ? "[red]Yes[/]" : "[green]No[/]",
                        $"[cyan]{packet.PotentialDangerMessage ?? "-"}[/]"
                    );

                    ctx.Refresh();
                    /*
                    Thread.Sleep(200); 
                */
                }
            });

        var dangerous = summary.GetPotentialDangerIPs(packets);
        if (dangerous.Count > 0)
        {
            AnsiConsole.MarkupLine($"\n[red]Potential danger packets: {dangerous.Count}[/]");
        }
        

        
        BackMenu();
    }
    
    //  =================
    //      Exit Menu
    //  =================

    public void Exit()
    {
        _PacketCapture.StopCapture();
        _PacketCapture.Dispose();
        _trafficLogger.Dispose();
        
        AnsiConsole.MarkupLine($"Application is closing. [yellow]See ya![/]");
        Environment.Exit(0);
    }
}