namespace Calculator.Core;

internal class Calculator(ITokenizer tokenizer, INumberStack numberStack) : ICalculator
{
    public double Calculate(string expression)
    {
        foreach (IToken token in tokenizer.Tokenize(expression))
        {
            token.Apply(numberStack);
        }

        return numberStack.Pop();
    }
}