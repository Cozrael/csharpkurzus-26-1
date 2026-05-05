using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator.Core;

internal static class OperationPrecedence
{
    public const int Addition = 1;
    public const int Subtraction = 1;
    public const int Multiplication = 2;
    public const int Division = 2;
}
