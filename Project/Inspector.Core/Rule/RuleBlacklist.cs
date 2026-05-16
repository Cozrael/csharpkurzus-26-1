namespace Inspector.Core.Rule;

public class RuleBlacklist : IRule 
{
    public string Name => "Blacklist";
    public void Apply()
    {
        throw new NotImplementedException();
    }
}