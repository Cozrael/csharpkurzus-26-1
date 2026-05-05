namespace Calculator.Core;

internal interface ITokenizer
{
    IEnumerable<IToken> Tokenize(string expression);
}
