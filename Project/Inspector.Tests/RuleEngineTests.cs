using Inspector.Core;

namespace Inspector.Tests;

public class RuleEngineTests
{
    private List<PacketData> _synSuspiciousPackets;
    private List<PacketData> _notSynSuspiciousPackets;
    private List<PacketData> _emptyPackets;
    private List<PacketData> _ackBeforeSynPackets;
    private RuleEngine _ruleEngine;

    [SetUp]
    public void Setup()
    {
        _synSuspiciousPackets =
        [
            new PacketData { SourceAddress = "192.168.1.1", Flags = "2" }, // SYN flag
            new PacketData { SourceAddress = "192.168.1.1", Flags = "4" }, // nem ACK
            new PacketData { SourceAddress = "192.168.1.1", Flags = "1" } // nem ACK
            
        ];

        _notSynSuspiciousPackets =
        [
            new PacketData { SourceAddress = "192.168.1.2", Flags = "2" }, //SYN flag
            new PacketData { SourceAddress = "192.168.1.3", Flags = "18" }, //SYN + ACK flag, sőt nem ugyan az az Source IP
            new PacketData { SourceAddress = "192.168.1.2", Flags = "16" } //SYN hez tartozó ACK
        ];

        _emptyPackets = [];
        
        _ackBeforeSynPackets = 
        [
            new PacketData { SourceAddress = "192.168.1.1", Flags = "16"}, //ACK flag előbb
            new PacketData { SourceAddress = "192.168.1.1", Flags = "2"}, //SYN flag
            new PacketData { SourceAddress = "192.168.1.1", Flags = "4"} //Nem ACK
        ];
        
        _ruleEngine = new RuleEngine();
        _ruleEngine.synAckOn = true;
    }

    [Test]
    //Az eset, amikor a SYN-hez nem tartozik ACK
    public void IsRule2On_NotFoundAck()
    {
        bool itMustBeTrue = _ruleEngine.SynFloodDetect(ref _synSuspiciousPackets);
        Assert.That(itMustBeTrue, Is.True);
    }
    

    [Test]
    //Az eset, amikor találunk SYN hez ACK párt
    public void IsRule2On_FoundAck()
    {
        bool itMustBeFalse = _ruleEngine.SynFloodDetect(ref _notSynSuspiciousPackets);
        Assert.That(itMustBeFalse, Is.False);
    }

    [Test]
    //Az eset, amikor üres lista van
    public void IsRule2On_EmptyPackets()
    {
        bool itMustBeFalse = _ruleEngine.SynFloodDetect(ref _emptyPackets);
        Assert.That(itMustBeFalse, Is.False);
    }

    [Test]
    //Az eset, amikor az ACK hamarabb van mint a SYN
    public void IsRule2On_AckBeforeSynPacket()
    {
        bool itMustBeTrue = _ruleEngine.SynFloodDetect(ref _ackBeforeSynPackets);
        Assert.That(itMustBeTrue, Is.True);
    }
}