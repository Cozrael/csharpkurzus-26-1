using Inspector.Client.UI;

namespace Inspector.Client;

using Inspector.Core;

public class Program
{
    static void Main(string[] args)
    {

        new Interface().MainMenu();
        
        
        
        Console.ReadKey();
        



        /*new UserTerminal().Menus();



        Console.ReadKey();*/

        /*Packets p = new Packets();
        p.packetStartCapture();

        Console.ReadKey();
        p.Dispose();*/
    }
    
    
    
}
