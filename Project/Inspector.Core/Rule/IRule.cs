namespace Inspector.Core.Rule;

public interface IRule
{
    string Name { get; }
    void Apply();
}