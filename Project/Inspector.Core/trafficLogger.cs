using System.Diagnostics;
using System.Text;

namespace Inspector.Core;

public class trafficLogger : IDisposable
{
    
    private static DateTime _DateTimeFileName = DateTime.UtcNow;
    private static string _file = $"../../../../log{_DateTimeFileName.Year}-{_DateTimeFileName.Month}-{_DateTimeFileName.Day}-{_DateTimeFileName.Hour}.txt" ;
    private StringBuilder _stringBuilder = new StringBuilder("");
    private FileStream _fileStream = new FileStream(_file,  FileMode.Append, FileAccess.Write, FileShare.Read);
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    
    public async void write(string packetData)
    {
        Debug.WriteLine("write");
        await _semaphore.WaitAsync();
        try
        {
            _DateTimeFileName = DateTime.UtcNow;
            _stringBuilder.Append(DateTimeOffset.Now + " :: " + packetData + "\n");
            Debug.WriteLine("fileba írás");

            if (_stringBuilder.Length > 8192)
            {
                pushToLog(_stringBuilder.ToString());
                _stringBuilder.Clear();
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    private void pushToLog(string stringPacket)
    {
        Debug.WriteLine("pushToLog");
        _fileStream.Write(new UTF8Encoding(true).GetBytes(stringPacket + "\n"));
        _fileStream.Flush();
    }

    public void Dispose()
    {
        pushToLog(_stringBuilder.ToString());
        _fileStream.Flush();
        _fileStream.Dispose();
    }
}