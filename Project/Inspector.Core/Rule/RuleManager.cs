namespace Inspector.Core.Rule;

public class RuleManager
{
    private List<IRule> _activeRules = [new RuleDefault()];
    
    public IReadOnlyList<IRule> ActiveRules => _activeRules;

    public void UpdateActiveRules(IEnumerable<IRule> availableRules,  IEnumerable<IRule> chosenRules)
    {
        foreach (var rule in availableRules)
        {
            if (chosenRules.Any(r => r.Name == rule.Name) && !_activeRules.Any(r => r.Name == rule.Name))
            {
                this._activeRules.Add(rule);
            } else if (!chosenRules.Any(r => r.Name == rule.Name) && _activeRules.Any(r => r.Name == rule.Name))
            {
                this._activeRules.RemoveAll(r => r.Name == rule.Name);
            }
        }
    }
    
    
}