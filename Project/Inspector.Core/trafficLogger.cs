using System.Diagnostics;
using System.Text;
using System.Text.Json;

using Inspector.Core.Rule;

namespace Inspector.Core;

public sealed class TrafficLogger : IDisposable
{
    private DateTime _DateTimeFileName;
    private string _file;
    private readonly StringBuilder _stringBuilder;
    private List<PacketData> _buffer;
    private readonly FileStream _fileStream;
    private readonly SemaphoreSlim _semaphore;
    private readonly TrafficStorage _ts;
    private readonly BlackList _blackList;
    private readonly RuleEngine _ruleEngine;

    public TrafficLogger(TrafficStorage trafficStorage, BlackList blackList, RuleEngine ruleEngine)
    {
        _DateTimeFileName  = DateTime.Now;
        _file = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..","src" ,"logs",
            $"{_DateTimeFileName.Year}-{_DateTimeFileName.Month}-{_DateTimeFileName.Day}-{_DateTimeFileName.Hour}.json");
        _fileStream = new FileStream(_file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
        _stringBuilder = new StringBuilder();
        _buffer = new List<PacketData>();
        _semaphore = new SemaphoreSlim(1, 1);
        _ts = trafficStorage;
        _blackList = blackList;
        _ruleEngine = ruleEngine;
    }
    

    public async Task Write(string sourceAddress, string destinationAddress, int headerLength, string protocol, int timeToLive,
        int sourcePort, int destinationPort, string flags = null)
    {
        Debug.WriteLine("write");
        
        var packetDataJson = new PacketData
        {
            Time = DateTime.Now.ToString("HH:mm:ss"),
            SourceAddress = sourceAddress,
            DestinationAddress = destinationAddress,
            HeaderLength = headerLength,
            Protocol = protocol,
            TimeToLive = timeToLive,
            SourcePort = sourcePort.ToString(),
            DestinationPort = destinationPort.ToString(),
            Flags = flags,  
        };
        
        
        await _semaphore.WaitAsync();
        try
        {
            _buffer.Add(packetDataJson);
            
            packetDataJson.PotentialDanger = _blackList.IPCheck(packetDataJson.SourceAddress);
            if (packetDataJson.PotentialDanger) packetDataJson.PotentialDangerMessage = PotentionDangerMsg.BLACKLISTED.ToString();
            _ruleEngine.PortScanDetect(ref _buffer);
            _ruleEngine.SynFloodDetect(ref _buffer);
            _ruleEngine.HeaderLengthCheck(ref packetDataJson);

            await _ts.Add(packetDataJson);
            
            _stringBuilder.Append(JsonSerializer.Serialize(packetDataJson) + "\n");
            Debug.WriteLine("fileba írás");

            if (_stringBuilder.Length > 8192)
            {
                PushToLog(_stringBuilder.ToString());
                _stringBuilder.Clear();
                if(_buffer.Count > 1000) _buffer.Clear();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
        finally
        {
            _semaphore.Release();
        }
        
    }
    
    private void PushToLog(string stringPacket)
    {
        Debug.WriteLine("pushToLog");
        _fileStream.Write(new UTF8Encoding(true).GetBytes(stringPacket + "\n"));
        _fileStream.Flush();
    }

    public void Dispose()
    {
        Debug.WriteLine("Le futott a trafficLogger Dispose");
        PushToLog(_stringBuilder.ToString());
        _ts.MakeAndWriteSummary();
        _fileStream.Flush();
        _fileStream.Dispose();
    }
}