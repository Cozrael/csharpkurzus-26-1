using System.Text;
using System.Text.Json;

namespace Inspector.Core;

public sealed class TrafficStorage
{
    private HashSet<PacketData> _packets;
    
    private readonly SemaphoreSlim _semaphore;

    private readonly BlackList _blackList;

    private readonly TrafficLogger _trafficLogger;

    private readonly StringBuilder _stringBuilder;
    
    
    public TrafficStorage(BlackList blackList)
    {
        _packets = new HashSet<PacketData>();
        _semaphore = new SemaphoreSlim(1, 1);
        _blackList = blackList;
        _stringBuilder = new StringBuilder();
    }
    
    public async Task Add(PacketData packet)
    {
        await _semaphore.WaitAsync();
        try
        {
            var packetContain = _packets.FirstOrDefault(ip => packet.Similar(ip));
            if (packetContain == null)
            {
                packet.PotentialDanger = (_blackList.IPCheck(packet.SourceAddress));
                if (packet.PotentialDanger) packet.PotentialDangerMessage = "BLACKLIST IP";
                // rule 1
                // rule 2
                if (packet.PotentialDanger)
                {
                    Console.WriteLine("Danger!!!"); // alert helye
                }
                _packets.Add(packet);
            }
            else
            {
                packetContain.Count += 1;
            }
        }
        finally
        {
                   
            _semaphore.Release();
        }
    }

    public List<PacketData> getCurrentPotentialDanger()
    {
        var potentailDanger = from pack in _packets where pack.PotentialDanger == true select pack;
        return potentailDanger.ToList();
    }
    

    public void MakeAndWriteSummary()
    {
            DateTime DateTimeFileName  = DateTime.Now;
            int db = 0;


            foreach (var packet in _packets)
            {
                _stringBuilder.AppendLine(JsonSerializer.Serialize(packet));
            }

            string file;
            while (true)
            {
                file = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..","src" ,"summary",
                    $"{DateTimeFileName.Year}-{DateTimeFileName.Month}-{DateTimeFileName.Day}-{DateTimeFileName.Hour}({db})-summary.json");
                if (File.Exists(file)) db++;
                else
                {
                    break;
                }
            }
        
            FileStream fileStream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            fileStream.Write(new UTF8Encoding(true).GetBytes(_stringBuilder.ToString()));
            fileStream.Flush();

    }
    
}