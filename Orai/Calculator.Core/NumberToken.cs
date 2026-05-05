namespace Calculator.Core;

public class NumberToken(double number) : IToken
{
    public void Apply(INumberStack stack)
    {
        stack.Push(number);
    }
}
