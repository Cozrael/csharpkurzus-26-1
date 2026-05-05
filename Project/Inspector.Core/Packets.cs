using System.Diagnostics;
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
    
    private readonly trafficLogger tl;
    public Packets(trafficLogger rmaker)
    {
        Console.WriteLine("Constructor");
        _device = LibPcapLiveDeviceList.Instance[0];
        this.tl = rmaker;
    }



    public async void packetStartCapture()
    {
        Debug.WriteLine("Start capture");
        _device.Open();
        _device.OnPacketArrival += Device_OnPacketArrival;
        
        _device.StartCapture();
    }

    public void packetStopCapture()
    {
        Debug.WriteLine("Stop capture");   
        _device.StopCapture();
    }
    
    public void Device_OnPacketArrival(object s, PacketCapture e)
    {
        var pack = Packet.ParsePacket(e.GetPacket().LinkLayerType, e.GetPacket().Data);
        /*
        Console.WriteLine(pack);
        */
        if (pack == null) return;
        var time = DateTimeOffset.Now;
        var ipPacket = pack.Extract<IPPacket>();
        if (ipPacket == null) return;
        Console.WriteLine("--------------------------");
        Console.WriteLine("{0} -- {1}:{2}:{3}:{4} | {5}",_db, time.Hour, time.Minute, time.Second, time.Millisecond, ipPacket);
        try
        {
            var packetString = ipPacket.ToString();

            Task.Run(() => tl.write(packetString));
            _db++;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }


    }
    
    public void Dispose()
    {
        if (!_isDisposed)
        {
            _device.StopCapture();
            _device.Close();
            _isDisposed = true;
        }  
    }
}