namespace Calculator.Core;

internal abstract class BinaryOperation : Operation
{
    public override sealed void Apply(INumberStack stack)
    {
        if (stack.Count < 2)
        {
            throw new InvalidOperationException("Not enugh values on stack");
        }

        double left = stack.Pop();
        double right = stack.Pop();

        double result = Apply(left, right);

        stack.Push(result);
    }

    protected abstract double Apply(double left, double right);
}
