namespace Calculator.Core;

public interface IToken
{
    void Apply(INumberStack stack);
}
