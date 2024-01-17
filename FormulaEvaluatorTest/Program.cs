// See https://aka.ms/new-console-template for more information
using FormulaEvaluator;
Evaluator eval = new Evaluator();
String s = "(4 + 5)";
eval.evaluator(s, null);


//delegate int Math(int x, int y);
//Math addition = (a, b) => a + b;
//Math subtraction = (a, b) => a - b;
//Math operation;
//if (token == “+” ) operation = addition;
//if (token == “-” ) operation = subtraction;
//if (token == “*” ) operation = Multiplication;
//Console.WriteLine($“Answer is { operation(5, 10) }”);