namespace Calculator.Core;

internal sealed class Subtraction : BinaryOperation
{
    public override int Precedence => OperationPrecedence.Subtraction;

    protected override double Apply(double left, double right) => left - right;
}
