using System.Text;
using System.Text.Json;

namespace Inspector.Core;

public sealed class TrafficStorage
{
    private HashSet<PacketData> _packets;

    private readonly SemaphoreSlim _semaphore;

    private readonly TrafficLogger _trafficLogger;

    private readonly StringBuilder _stringBuilder;
    
    public TrafficStorage()
    {
        _packets = new HashSet<PacketData>();
        _semaphore = new SemaphoreSlim(1, 1);
        _stringBuilder = new StringBuilder();
    }

    public HashSet<PacketData> GetPackets() { return _packets; }
    
    public async Task Add(PacketData packet)
    {
        await _semaphore.WaitAsync();
            try
            {
                var packetContain = _packets.FirstOrDefault(ipP => packet.Similar(ipP));
                if (packetContain == null)
                {
                    _packets.Add(packet);
                }
                else
                {
                    if (packet.PotentialDanger)
                    {
                        packetContain.PotentialDanger = true;
                        packetContain.PotentialDangerMessage = packet.PotentialDangerMessage;
                    }
                    packetContain.Count += 1;
                }
            }
            finally
            {
                   
                _semaphore.Release();
            }

    }

    public List<PacketData> GetCurrentPotentialDanger()
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
        
            using FileStream fileStream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            fileStream.Write(new UTF8Encoding(true).GetBytes(_stringBuilder.ToString()));
            _stringBuilder.Clear();
            fileStream.Flush();

    }
    
}