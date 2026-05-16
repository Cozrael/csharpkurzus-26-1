using System.Diagnostics;

using SharpPcap;

namespace Inspector.Client;

using Inspector.Core;

public class Program
{
    private static readonly BlackList _blackList = new BlackList();
    private static readonly TrafficStorage _trafficStorage = new TrafficStorage(_blackList);

    private static readonly TrafficLogger _trafficLogger = new TrafficLogger(_trafficStorage);
    private static readonly Packets _PacketCapture = new Packets(_trafficLogger);
    static void Main(string[] args)
    {
        ReadSummarys summary = new ReadSummarys();

        string[] test = summary.ListAllSummaries();
        
        foreach(string i in test)
        {
            Console.WriteLine(i);
        }

        var b = summary.ReadSummary(test[0]);
        foreach (var i  in b)
        {
            Console.WriteLine(i);
        }
        _PacketCapture.StartCapture();
        Console.ReadLine();
        _PacketCapture.StopCapture();
        _PacketCapture.Dispose();
        _trafficLogger.Dispose();
    }
}
