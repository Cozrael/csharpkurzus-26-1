namespace Inspector.Core;

public class ruleEngine
{
    private bool _rule1on { get; set; } = false;
    private bool _rule2on { get; set; } = false;
    private bool _rule3on { get; set; } = true;

    public ruleEngine()
    {
    }

    // port scan detect
    public bool IsRule1On(List<PacketData> packets)
    {
        foreach (var packet in packets)
        {
            
        }
        return true;
    }

    // syn - ack
    public bool IsRule2On(ref List<PacketData> packets)
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
                    }
                }
            }
        }

        if (synFloodList.Count > 0)
        {
            return  true;
        }

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