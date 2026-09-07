// See https://aka.ms/new-console-template for more information

using CalculatorApp;

Console.WriteLine("Hello, World!");

var calc = new Calculator();

Console.WriteLine("Difference: " + calc.Subtract(3,2));
Console.WriteLine("Division: " + calc.Divide(3,2));
Console.WriteLine("Division: " + calc.Divide(3,0));