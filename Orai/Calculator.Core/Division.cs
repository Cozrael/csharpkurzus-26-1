namespace Calculator.Core;

internal sealed class Division : BinaryOperation
{
    public override int Precedence => OperationPrecedence.Division;

    protected override double Apply(double left, double right) => left / right;
}