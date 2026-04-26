using System.Reflection;

using PacketDotNet;

using SharpPcap;
using SharpPcap.LibPcap;

namespace Inspector.Core;

public class Packets : IDisposable
{

    private LibPcapLiveDevice _device; 
    private bool _isDisposed = false;
    private int _db = 0;
    public Packets()
    {
        Console.WriteLine("Constructor");
        _device = LibPcapLiveDeviceList.Instance[0];
    }



    public void packetStartCapture()
    {
        Console.WriteLine("Start capture");
        _device.Open();
        _device.OnPacketArrival += Device_OnPacketArrival;
        
        _device.StartCapture();
    }

    public void packetStopCapture()
    {
        Console.WriteLine("Stop capture");   
        _device.StopCapture();
    }
    
    public void Device_OnPacketArrival(object s, PacketCapture e)
    {
        var pack = Packet.ParsePacket(e.GetPacket().LinkLayerType, e.GetPacket().Data);
        if (pack == null) return;
        var time = DateTimeOffset.Now;
        var ipPacket = pack.Extract<IPPacket>();
        Console.WriteLine("--------------------------");
        Console.WriteLine("{0} -- {1}:{2}:{3}:{4} | {5}",_db, time.Hour, time.Minute, time.Second, time.Millisecond, ipPacket);    
        _db++;
    }
    
    public void Dispose()
    {
        if (!_isDisposed)
        {
            _device.StopCapture();
            _device.Close();
            this.Dispose();
            _isDisposed = true;
        }  
    }
}