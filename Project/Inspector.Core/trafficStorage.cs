using System.Text;
using System.Text.Json;

namespace Inspector.Core;

public sealed class TrafficStorage
{
    private HashSet<PacketData> _packets;
    
    private readonly SemaphoreSlim _semaphore;

    private readonly BlackList _bl;

    private readonly TrafficLogger _tl;

    public TrafficStorage(BlackList blackList)
    {
        _packets = new HashSet<PacketData>();
        _semaphore = new SemaphoreSlim(1, 1);
        _bl = blackList;
    }
    
    public async Task Add(PacketData packet)
    {
        await _semaphore.WaitAsync();
        try
        {
            var packetContain = _packets.FirstOrDefault(ip => packet.Similar(ip));
            if (packetContain == null)
            {
                packet.PotentialDanger = (_bl.IPCheck(packet.SourceAddress));
                if (packet.PotentialDanger)
                {
                    Console.WriteLine("Danger!!!");
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

    public void MakeAndWriteSummary()
    {
        DateTime DateTimeFileName  = DateTime.Now;
        int db = 0;

        StringBuilder sb = new StringBuilder();
        foreach (var packet in _packets)
        {
            sb.AppendLine(JsonSerializer.Serialize(packet));
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
        fileStream.Write(new UTF8Encoding(true).GetBytes(sb + "\n"));
        fileStream.Flush();
    }
}