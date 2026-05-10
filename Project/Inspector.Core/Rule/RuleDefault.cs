namespace Inspector.Core.Rule;

public class RuleDefault : IRule 
{
    public string Name => "Default";
    public void Apply()
    {
        throw new NotImplementedException();
    }
}