using System.Diagnostics;

using PacketDotNet;

namespace Inspector.Core;

public class RuleEngine
{
    private bool _rule1on { get; set; } = false;
    private bool _rule2on { get; set; } = false;
    private bool _rule3on { get; set; } = true;
    
    public RuleEngine() { }

    public bool PortScanDetect(ref List<PacketData> rawPackets)
    {
        List<string> vizsgaltIP = new List<string>();
        foreach (var v in rawPackets)
        {
            if (vizsgaltIP.Contains(v.SourceAddress))
            {
                continue;
            }
            int db = 0;
            List<string> port = new List<string>();
            
            foreach (var k in rawPackets)
            {
                if (v.SourceAddress == k.SourceAddress && !port.Contains(k.DestinationPort))
                {
                    port.Add(k.DestinationPort);
                    var dif = DateTime.Parse(k.Time) - DateTime.Parse(v.Time);
                    if (dif <= TimeSpan.Parse("00:00:05"))
                    {
                        db++;
                    }
                } 
            }
            vizsgaltIP.Add(v.SourceAddress);

            if (db > 10)
            {
                foreach (var rPacket in rawPackets)
                {
                    if (v.SourceAddress == rPacket.SourceAddress)
                    {
                        rPacket.PotentialDanger = true;
                        rPacket.PotentialDangerMessage = "Port scan";
                        Debug.WriteLine("Danger!!!");
                    }
                }
            }

        }

        return false;
    }

    // syn - ack
    public bool IsRule2On(ref List<PacketData> packets)
    {
        return false;
    }

    public bool HeaderLengthCheck(ref PacketData packet)
    {
        if (packet.HeaderLength * 4 < 20)
        {
            Debug.WriteLine("Danger!!!");
            packet.PotentialDanger = true;
            packet.PotentialDangerMessage = "Too short";
            return true;
        }
        if (packet.HeaderLength * 4 > 60)
        {
            Debug.WriteLine("Danger!!!");
            packet.PotentialDanger = true;
            packet.PotentialDangerMessage = "Too much";
            return true;
        }

        return false;
    }
}