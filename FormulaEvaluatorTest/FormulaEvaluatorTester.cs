﻿
using FormulaEvaluator;
try
{
    if (Evaluator.Evaluate("5+5", null) == 10) Console.WriteLine("Added simple expression.");
    if (Evaluator.Evaluate("5-5", null) == 0) Console.WriteLine("Subtracted simple expression.");
    if (Evaluator.Evaluate("5+5*2", null) == 15) Console.WriteLine("Added and multipled simple expression.");
    if (Evaluator.Evaluate("5+5 / 2", null) == 7) Console.WriteLine("Added and divided simple expression.");
    if (Evaluator.Evaluate("(5+5)", null) == 10) Console.WriteLine("Added simple parenthesis expression.");
    if (Evaluator.Evaluate("(5-5)", null) == 0) Console.WriteLine("Subtracted simple parenthesis expression.");
    if (Evaluator.Evaluate("25/5", null) == 5) Console.WriteLine("Divided simple expression.");
    if (Evaluator.Evaluate("5*5", null) == 25) Console.WriteLine("Multipled simple expression.");
    if (Evaluator.Evaluate("20/2*10/10+10*5", null) == 60) Console.WriteLine("Divided, multiplied, and added a complex expression");
    if (Evaluator.Evaluate("20/2*10-10/10+10*5", null) == 149) Console.WriteLine("Divided, multiplied, added, and subtracted a complex expression");

}
catch (ArgumentException)
{
    Console.WriteLine("Is not a valid expression.");
}

//Purpose is to throw an exception:
try
{
    if (Evaluator.Evaluate("5/0", null) == 10) Console.WriteLine("Wasnt suppose to divide.");
} catch (ArgumentException)
{
    Console.WriteLine("Good job catching that!!");
}

try
{
    if (Evaluator.Evaluate("(5/0)", null) == 10) Console.WriteLine("Wasnt suppose to happen");
}
catch (ArgumentException)
{
    Console.WriteLine("Good job catching that!!");
}

 
//Testing variables
try
{
    if (Evaluator.Evaluate("(5/x)", (x) => 5) == 1) Console.WriteLine("Used Lambda successfully on simple divison expression.");
    if (Evaluator.Evaluate("(5*x)", (x) => 5) == 25) Console.WriteLine("Used Lambda successfully on simple multiplication expression.");
    if (Evaluator.Evaluate("(5-x)", (x) => 5) == 0) Console.WriteLine("Used Lambda successfully on simple subtraction expression.");
    if (Evaluator.Evaluate("(5/x)", (x) => 5) == 1) Console.WriteLine("Used Lambda successfully on simple divison expression.");
}
catch (ArgumentException)
{
    Console.WriteLine("Not a valid lambda expression for Lookup.");
}

//Testing incorrect variables
static int foo(string x)
{
    if (x == "x")
        return 1;
    if (x == "y")
        return 2;
    if (x == "z")
        return 3;
    else
        throw new ArgumentException();
}
try
{
    if (Evaluator.Evaluate("a", foo) == 0) Console.WriteLine("Thats right theres no a value.");

} catch (ArgumentException)
{
    Console.WriteLine("Good job catching that!!");
}

try
{
    if (Evaluator.Evaluate("x + y", foo) == 3) Console.WriteLine("Delegate works on addition!");
    if (Evaluator.Evaluate("x - y", foo) == 1) Console.WriteLine("Delegate works on subtraction!");
    if (Evaluator.Evaluate("x * y", foo) == 2) Console.WriteLine("Delegate works on multiplication!");
    if (Evaluator.Evaluate("y / x", foo) == 2) Console.WriteLine("Delegate works on division!");

    //Testing negative results
    if (Evaluator.Evaluate("y - x", foo) == -1) Console.WriteLine("Delegate works on subtraction!");


}
catch (ArgumentException)
{
    Console.WriteLine("Well that wasnt suppose to happen.");
}

