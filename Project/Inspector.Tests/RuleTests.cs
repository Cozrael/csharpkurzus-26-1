using Inspector.Core;
using Inspector.Core.Rule;

namespace Inspector.Tests;

public class RuleTests
{
    private readonly BlackList _blackList = new();
    
    private readonly RuleEngine _ruleEngine = new();

    
    [Test]
    public void TestBlackListIPtrue()
    {
        Assert.That(_blackList.IPCheck("1.1.1.1"), Is.True);
        Assert.That(_blackList.IPCheck("223.254.0.0"), Is.True);
        Assert.That(_blackList.IPCheck("1.1.1.2"), Is.False);
    }

    [Test]
    public void TestTruePortScan()
    {
        var portScan = new List<PacketData>();
        _ruleEngine.portScanOn =  true;

        for (int i = 0; i < 26; i++)
        {
            portScan.Add(new PacketData
            {
                Time = "12:00:00",
                SourceAddress = "1.1.1.1",
                DestinationPort = (1000 + i).ToString(),
                Protocol = "TCP"
            });
        }
        
        Assert.That(_ruleEngine.PortScanDetect(ref portScan), Is.True);
        
    }

    [Test]
    public void TestFalsePortScan()
    {
        var noPortScan = new List<PacketData>();
        _ruleEngine.portScanOn =  true;
        
        for (int i = 0; i < 26; i++)
        {
            noPortScan.Add(new PacketData
            {
                Time = $"12:00:{i}",
                SourceAddress = "192.168.1.100",
                DestinationPort = (1000 + i).ToString(),
                Protocol = "TCP"
            });
        }
        
        Assert.That(_ruleEngine.PortScanDetect(ref noPortScan), Is.False);
    }

    [Test]
    public void TestHeaderLengthOk()
    {
        var headerLength = new PacketData();
        _ruleEngine.headLengthOn = true;
        
        headerLength.HeaderLength = 5;
        Assert.That(_ruleEngine.HeaderLengthCheck(ref headerLength), Is.False);
        Assert.That(headerLength.PotentialDanger, Is.False);
        Assert.That(headerLength.PotentialDangerMessage, Is.Null);
    }

    [Test]
    public void TestHeaderLengthTooMuch()
    {
        var headerLength = new PacketData();
        _ruleEngine.headLengthOn = true;
        
        headerLength.HeaderLength = 100;
        Assert.That(_ruleEngine.HeaderLengthCheck(ref headerLength), Is.True);
        Assert.That(headerLength.PotentialDanger, Is.True);
        Assert.That(headerLength.PotentialDangerMessage, Is.EqualTo(PotentionDangerMsg.TooLongHeader.ToString()));
    }
    
    [Test]
    public void TestHeaderLengthTooShort()
    {
        var headerLength = new PacketData();
        _ruleEngine.headLengthOn = true;
        
        headerLength.HeaderLength = 1;
        Assert.That(_ruleEngine.HeaderLengthCheck(ref headerLength), Is.True);
        Assert.That(headerLength.PotentialDanger, Is.True);
        Assert.That(headerLength.PotentialDangerMessage, Is.EqualTo(PotentionDangerMsg.TooShortHeader.ToString()));
    }
}