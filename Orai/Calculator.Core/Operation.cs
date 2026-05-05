namespace Calculator.Core;

public abstract class Operation : IToken
{
    public abstract void Apply(INumberStack stack);

    public abstract int Precedence { get; }

    public virtual bool IsRightAssociative { get; } = false;
}
