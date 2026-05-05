namespace Calculator.Core;

public interface INumberStack
{
    int Count { get; }

    double Pop();

    void Push(double number);
}

internal class NumberStack(int capacity) : INumberStack
{
    private readonly double[] _stack = new double[capacity];
    private int _count = 0;

    public int Count => _count;

    public double Pop()
    {
        return _stack[--_count];
    }

    public void Push(double number)
    {
        _stack[_count++] = number;
    }
}