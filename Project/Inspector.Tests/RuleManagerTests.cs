using Inspector.Core.Rule;

namespace Inspector.Tests;

public class RuleManagerTests
{
    private class TestRule : IRule
    {
        public string Name { get; }
        public TestRule(string name) => Name = name;
        public void Apply() { }
    }
    

    private List<IRule> _availableRules;
    private List<IRule> _chosenRules;
    
    private IRule _ruleA;
    private IRule _ruleB;
    private IRule _ruleC;
    private IRule _ruleD;
    
    private RuleManager _manager;
    
    [SetUp]
    public void Setup()
    {
        _ruleA = new TestRule("RuleA");
        _ruleB = new TestRule("RuleB");
        _ruleC = new TestRule("RuleC");
        _ruleD = new TestRule("RuleD");
        
        _availableRules = new List<IRule> { _ruleA, _ruleB, _ruleC, _ruleD };
        
        _manager =  new RuleManager();
        
    }

    
    //Ellenőrízzük hogy belekerül-e a kiválasztott rule.
    [Test]
    public void UpdateActiveRules_AddsChosenRule_ToActiveRules()
    {
        _chosenRules = new List<IRule> { _ruleA };
        
        _manager.UpdateActiveRules(_availableRules, _chosenRules);
        
        Assert.That(_manager.ActiveRules, Does.Contain(_ruleA)); 

    }
}
