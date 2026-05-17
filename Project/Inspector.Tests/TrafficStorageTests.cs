using Inspector.Core;

namespace Inspector.Tests;

public class TrafficStorageTests
{
    private TrafficStorage _storage = new TrafficStorage();

    [Test]
    public void TrafficStoragePotentialDanger()
    {
        var _testPacket = new PacketData
        {
            Time = "12:00:00",
            SourceAddress = "192.168.1.1",
            DestinationAddress = "10.0.0.1",
            SourcePort = "443",
            DestinationPort = "443",
            Protocol = "TCP",
            HeaderLength = 20,
            TimeToLive = 64,
            PotentialDanger = true,
            Count = 1
        };

        _storage.Add(_testPacket);
        _storage.Add(_testPacket);

        var currentPotentialDanger= _storage.GetCurrentPotentialDanger();
        Assert.That(currentPotentialDanger.Count, Is.EqualTo(1));
    }

    [Test]
    public void TrafficStorageAdd()
    {
        var _testPacket = new PacketData
        {
            Time = "12:00:00",
            SourceAddress = "192.168.1.1",
            DestinationAddress = "10.0.0.1",
            SourcePort = "443",
            DestinationPort = "443",
            Protocol = "TCP",
            HeaderLength = 20,
            TimeToLive = 64,
            PotentialDanger = true,
            Count = 1
        };

        _storage.Add(_testPacket);

        var res=_storage.GetPackets();
        
        Assert.That(res.Contains(_testPacket), Is.True);
    }
    
    [Test]
    public void TrafficStorageAddAndIncrease()
    {
        TrafficStorage storage = new TrafficStorage();
        var _testPacket = new PacketData
        {
            Time = "12:00:00",
            SourceAddress = "192.168.1.1",
            DestinationAddress = "10.0.0.1",
            SourcePort = "443",
            DestinationPort = "443",
            Protocol = "TCP",
            HeaderLength = 20,
            TimeToLive = 64,
            PotentialDanger = true,
            Count = 1
        };

        storage.Add(_testPacket);
        storage.Add(_testPacket);

        var res=storage.GetPackets();
        
        Assert.That(res.FirstOrDefault().Count, Is.EqualTo(2));
    }
}