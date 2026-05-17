using System.Diagnostics;

using Inspector.Client.UI;

using SharpPcap;

namespace Inspector.Client;

using Inspector.Core;

public class Program
{
    

    static void Main(string[] args)
    {
        Interface _interface = new Interface();
        
        _interface.MainMenu();
        
        Console.ReadKey();
    }
    
}
