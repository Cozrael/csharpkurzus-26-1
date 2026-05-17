using System.Diagnostics;

using Inspector.Core.Rule;

using PacketDotNet;

namespace Inspector.Core;

public class RuleEngine
{
    public bool portScanOn { get; set; } = false;
    public bool synAckOn { get; set; } = false;
    public bool headLengthOn { get; set; } = false;
    
    public RuleEngine() { }

    public bool PortScanDetect(ref List<PacketData> rawPackets)
    {
        if (portScanOn)
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

                if (db > 25)
                {
                    foreach (var rPacket in rawPackets)
                    {
                        if (v.SourceAddress == rPacket.SourceAddress)
                        {
                            rPacket.PotentialDanger = true;
                            rPacket.PotentialDangerMessage = PotentionDangerMsg.PortScan.ToString();
                            Debug.WriteLine("Danger!!!");
                            return true;
                        }
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
        if (headLengthOn)
        {
            if (packet.HeaderLength * 4 < 20)
            {
                Debug.WriteLine("Danger!!!");
                packet.PotentialDanger = true;
                packet.PotentialDangerMessage = PotentionDangerMsg.TooShortHeader.ToString();
                return true;
            }
            if (packet.HeaderLength * 4 > 60)
            {
                Debug.WriteLine("Danger!!!");
                packet.PotentialDanger = true;
                packet.PotentialDangerMessage = PotentionDangerMsg.TooLongHeader.ToString();
                return true;
            }
        }
        
        return false;
        
    }
}