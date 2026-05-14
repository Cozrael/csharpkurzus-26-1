namespace Inspector.Core;

public class ruleEngine
{
    private bool _rule1on { get; set; } = false;
    private bool _rule2on { get; set; } = false;
    private bool _rule3on { get; set; } = true;
    
    ruleEngine()
    {
    }

    // port scan detect
    public bool IsRule1On(List<PacketData> packets)
    {
        foreach (var packet in packets)
        {
            
        }
    }

    // syn - ack
    public bool IsRule2On(List<PacketData> packets)
    {
        return false;
    }

    // header length
    public (string, bool) headerLengthCheck(PacketData packet)
    {
        if (packet.HeaderLength * 4 <= 20)
        {
            return ("Not enough", true);
        }
        else if (packet.HeaderLength * 4 >= 24 && packet.HeaderLength * 4 <= 60)
        {
            return ("warrning", true);
        }
        else if (packet.HeaderLength * 4 >= 60)
        {
            return ("Too much", true);
        }
        else
        {
            return ("OK", false);
        }
        
    }
}