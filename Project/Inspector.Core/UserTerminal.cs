
using PacketDotNet;

namespace Inspector.Core;

public class UserTerminal
{

    public void Menus()
    {
        Console.WriteLine("<< Type a menu number >>");
        Console.WriteLine("\t1: All");
        Console.WriteLine("\t2: Warrnings");
        Console.WriteLine("\t3: Placeholder");
        Console.WriteLine("\t4: Settings");
        Console.WriteLine("<< Press CTRL + C to exit >>");
        
        int input = int.TryParse(Console.ReadLine(), out input) ? input : 0;

        
        switch (input)
        {
            case 0:
                throw new InvalidOperationException("Enter a valid number from the list!");
                break;
            case 1:
                MenusAll();
                break;
            case 2:
                MenusWarnings();
                break;
            case 4:
                MenusSettings();
                break;
            default:
                MenusPlaceholder();
                break;
        }
            
            
            
    }

    public void MenusAll()
    {
        Console.WriteLine("Menu All selected");
        Menus();
    }

    public void MenusWarnings()
    {
        Console.WriteLine("\rMenu Warnings selected");
        Menus();

    }

    public void MenusSettings()
    {
        Console.WriteLine("\rMenu Settings selected");
        Menus();

    }

    public void MenusPlaceholder()
    {
        Console.WriteLine("\rMenu Placeholder selected");
        Menus();

    }


    
}