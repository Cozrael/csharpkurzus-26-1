using SharpPcap;
using SharpPcap.LibPcap;
using PacketDotNet;

namespace Inspector.Core;

public class Class1
{
    public void SayHello()
    {
        Console.WriteLine("Hello Inspector!");
    }
    
    /*
    void Device_OnPacketArrival(object s, PacketCapture e)
    {
        Console.WriteLine(e.GetPacket());
        
        using var device = new CaptureFileReaderDevice("200722_win_scale_examples_anon.pcapng");
        device.Open();
        device.OnPacketArrival += Device_OnPacketArrival;
        device.Capture();
    }
    */
    
}
