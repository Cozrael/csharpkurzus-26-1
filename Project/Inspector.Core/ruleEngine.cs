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
    public bool SynFloodDetect(ref List<PacketData> packets)
    {
        if (synAckOn)
        {
            List<String> suspiciousList = new List<String>();
            List<PacketData> synFloodList = new List<PacketData>();
            int id = 1;
            int vizsgaloId = 0;

            foreach (var packet in packets)
            {
                id++;
                if (packet.Flags == "2" && !suspiciousList.Contains(packet.SourceAddress)) // 2 -> csak SYN
                {
                    suspiciousList.Add(packet.SourceAddress);
                    vizsgaloId = id - 1;
                    for (int i = vizsgaloId; i < packets.Count; i++)
                    {
                        if (suspiciousList.Contains(packets[i].SourceAddress) //ha megegyezik az eltárolt listában szereplő IP a vizsgált csomag IP-jével 
                            && packets[i].Flags != "16") //és a flagje nem 16, avagy csak ACK
                        {
                            synFloodList.Add(packets[i]); //akkor hozzá adjuk a fixen veszélyes csomagokat tároló listába
                            packets[i].PotentialDanger = true;
                            packets[i].PotentialDangerMessage = PotentionDangerMsg.SynFlood.ToString();
                            Debug.WriteLine("Danger!!!");
                            return true;
                        }
                    }
                }
            }

            if (synFloodList.Count > 0)
            {
                return  true;
            }
        }
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