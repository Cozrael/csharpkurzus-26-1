using System.Diagnostics;
using System.Net;

using PacketDotNet;

namespace Inspector.Core;

public sealed class BlackList
{
    private readonly List<IPNetwork> _blackList;
    private readonly List<string> _file;

    public BlackList()
    {
        _blackList = new List<IPNetwork>();
        _file = new List<string>()
        {
            "feodotrackerBotNetBlackList.txt",
            "SpamhausBlackList.txt",
            "testerBlackList.txt",
        };
        
        for (int i = 0; i < _file.Count(); i++)
        {
            Debug.WriteLine(_file.Count());
            Debug.WriteLine(_file[i] + " beolvasása");
            using (StreamReader sr = new StreamReader(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..","src", "blacklists", _file[i])))
            {
                while (!sr.EndOfStream)
                {
                    string str = sr.ReadLine();
                    if (str != null)
                    {
                        if (str.Contains('/'))
                        {
                            _blackList.Add(IPNetwork.Parse(str));
                        }
                        else
                        {
                            _blackList.Add(IPNetwork.Parse(str + "/32"));
                        }
                    }
                }
            }
        }
    }


    public Boolean IPCheck(string ip)
    {
        var ipCheckQuery = from i in _blackList
            where i.Contains(IPAddress.Parse(ip))
            select i;
        if (ipCheckQuery.Count() > 0)
        {
            Debug.WriteLine("Danger!!!");
            return true;
        }
        return false;
    }
}