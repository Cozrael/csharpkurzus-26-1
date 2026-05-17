using System.Diagnostics;
using System.Reflection;

using PacketDotNet;

using SharpPcap;
using SharpPcap.LibPcap;

namespace Inspector.Core;

public sealed class Packets : IDisposable
{
    private readonly LibPcapLiveDevice _device; 
    private bool _isDisposed = false;
    private int _db = 0;
    private readonly TrafficLogger _tl;
    
    public Packets(TrafficLogger trafficLogger)
    {
        Debug.WriteLine("Packets Constructor");
        try
        {
            _device = LibPcapLiveDeviceList.Instance[0];

        }
        catch (PcapException pcapException)
        {
            Console.WriteLine("Nincs wifi interface");
            Console.WriteLine("Error:" + pcapException.Message);
        }
        _tl = trafficLogger;
    }



    public async void StartCapture()
    {
        Debug.WriteLine("Start capture");
        try
        {
            _device.Open();
            _device.OnPacketArrival += Device_OnPacketArrival;

            _device.StartCapture();
        }
        catch (PcapException pcapException)
        {
            throw new UnauthorizedAccessException("Nincs engedély a wifi interface eléréséhez!: " + pcapException.Message);
        }
    }

    public void StopCapture()
    {
        Debug.WriteLine("Stop capture");   
        _device.StopCapture();
    }
    
    private void Device_OnPacketArrival(object s, PacketCapture e)
        {
            var pack = Packet.ParsePacket(e.GetPacket().LinkLayerType, e.GetPacket().Data);
            if (pack == null) return;
            var ipPacket = pack.Extract<IPPacket>();
            if(ipPacket == null) return;
            var tcpPacket =  pack.Extract<TcpPacket>();
            var udpPacket = pack.Extract<UdpPacket>();
            

            switch (ipPacket.Protocol.ToString())
            {
                case "Tcp":
                    {
                        string flags = tcpPacket.Flags.ToString();
                        try
                        {
                            Task.Run(() => _tl.Write(ipPacket.SourceAddress.ToString(), ipPacket.DestinationAddress.ToString(), ipPacket.HeaderLength,
                                ipPacket.Protocol.ToString(), ipPacket.TimeToLive, tcpPacket.SourcePort, tcpPacket.DestinationPort, flags));
                            _db++;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                        break;
                    }
                case "Udp":
                    {
                        try
                        {
                            Task.Run(() => _tl.Write(ipPacket.SourceAddress.ToString(), ipPacket.DestinationAddress.ToString(), ipPacket.HeaderLength, 
                                ipPacket.Protocol.ToString(), ipPacket.TimeToLive, udpPacket.SourcePort, udpPacket.DestinationPort));
                            _db++;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                        break;
                    }
                default:
                    Debug.WriteLine("Unkown protocol:" +  ipPacket.Protocol);
                    try
                    {
                        Task.Run(() => _tl.Write(ipPacket.SourceAddress.ToString(), ipPacket.DestinationAddress.ToString(), ipPacket.HeaderLength, 
                            ipPacket.Protocol.ToString(), ipPacket.TimeToLive, udpPacket.SourcePort, udpPacket.DestinationPort));
                        _db++;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }

                    break;
            }
        }
    
    public void Dispose()
    {
        if (!_isDisposed)
        {
            _device.StopCapture();
            _device.Close();
            _isDisposed = true;
            Debug.WriteLine("Lefutott a Packets Dispose");
        }  
    }
}