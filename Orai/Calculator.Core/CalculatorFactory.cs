using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator.Core;

public static class CalculatorFactory
{
    public static ICalculator Create()
    {
        Tokenizer tokenizer = new Tokenizer();
        NumberStack numberStack = new NumberStack(10);

        return new Calculator(tokenizer, numberStack);
    }
}
