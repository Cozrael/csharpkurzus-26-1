using System.ComponentModel;

namespace Inspector.Core;

public sealed class PacketData
{
    public DateTime Time { get; set; }
    public string SourceAddress { get; set; }
    public string DestinationAddress { get; set; }
    public int HeaderLength { get; set; }
    public string Protocol { get; set; }
    public int TimeToLive { get; set; }
    public string SourcePort { get; set; }
    public string DestinationPort { get; set; }
    public string Flags { get; set; }

    public Boolean PotentialDanger { get; set; }
    public int Count { get; set; } = 1;


    public bool Similar(PacketData packet)
    {
        if (SourceAddress == packet.SourceAddress && DestinationAddress == packet.DestinationAddress &&
            SourcePort == packet.SourcePort && DestinationPort == packet.DestinationPort && Protocol == packet.Protocol)
        {
            return true;
        }
        return false;
    }
}