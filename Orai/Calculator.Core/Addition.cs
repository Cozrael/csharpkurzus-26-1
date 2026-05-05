namespace Calculator.Core;

internal sealed class Addition : BinaryOperation
{
    public override int Precedence => OperationPrecedence.Addition;

    protected override double Apply(double left, double right) => left + right;
}
