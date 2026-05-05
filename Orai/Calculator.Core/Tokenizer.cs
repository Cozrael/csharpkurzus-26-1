using static System.StringSplitOptions;

namespace Calculator.Core;

internal class Tokenizer : ITokenizer
{
    private readonly Dictionary<string, IToken> _tokens = new()
    {
        ["+"] = new Addition(),
        ["-"] = new Subtraction(),
        ["*"] = new Multiplication(),
        ["/"] = new Division(),
    };

    public IEnumerable<IToken> Tokenize(string expression)
    {
        foreach (string part in GetExpressionParts(expression))
        {
            if (_tokens.TryGetValue(part, out IToken? token))
            {
                yield return token;
            }
            else if (double.TryParse(part, out double number))
            {
                yield return new NumberToken(number);
            }
            else
            {
                throw new InvalidOperationException($"Unrecognized expression part: '{part}'");
            }
        }
    }

    private static string[] GetExpressionParts(string expression)
    {
        return expression.Split(' ', TrimEntries | RemoveEmptyEntries);
    }
}