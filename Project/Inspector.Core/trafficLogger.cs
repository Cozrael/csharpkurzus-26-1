using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace Inspector.Core;

public sealed class TrafficLogger : IDisposable
{

    private DateTime _DateTimeFileName;
    private string _file;
    private readonly StringBuilder _stringBuilder;
    private readonly FileStream _fileStream;
    private readonly SemaphoreSlim _semaphore;
    private readonly TrafficStorage _ts;

    public TrafficLogger(TrafficStorage trafficStorage)
    {
        _DateTimeFileName  = DateTime.UtcNow;
        _file = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..","src" ,"logs",
            $"{_DateTimeFileName.Year}-{_DateTimeFileName.Month}-{_DateTimeFileName.Day}-{_DateTimeFileName.Hour}.json");
        _fileStream = new FileStream(_file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
        _stringBuilder = new StringBuilder();
        _semaphore = new SemaphoreSlim(1, 1);
        _ts = trafficStorage;
    }
    

    public async Task Write(string sourceAddress, string destinationAddress, int headerLength, string protocol, int timeToLive,
        int sourcePort, int destinationPort, string flags = null)
    {
        Debug.WriteLine("write");
        await _semaphore.WaitAsync();
        try
        {
            var packetDataJson = new PacketData
            {
                Time = DateTime.Now,
                SourceAddress = sourceAddress,
                DestinationAddress = destinationAddress,
                HeaderLength = headerLength,
                Protocol = protocol,
                TimeToLive = timeToLive,
                SourcePort = sourcePort.ToString(),
                DestinationPort = destinationPort.ToString(),
                Flags = flags,  
            };

            Task.Run(() => _ts.Add(packetDataJson));
            
            _stringBuilder.Append(JsonSerializer.Serialize(packetDataJson) + "\n");
            Debug.WriteLine("fileba írás");

            if (_stringBuilder.Length > 8192)
            {
                PushToLog(_stringBuilder.ToString());
                _stringBuilder.Clear();
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