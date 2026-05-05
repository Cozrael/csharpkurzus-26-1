using Calculator.Core;

Console.WriteLine("Welcome to the calculator!");
Console.Write("> ");

string expression = Console.ReadLine() ?? string.Empty;

ICalculator calculator = CalculatorFactory.Create();
double result = calculator.Calculate(expression);

Console.WriteLine(result);