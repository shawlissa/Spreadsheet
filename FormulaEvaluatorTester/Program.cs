// See https://aka.ms/new-console-template for more information
//
using FormulaEvaluator;

class FormulaEvaluatorTester {
   static void Main(String[] args) {
        String s = "4 + 2";
        Console.WriteLine("hello");
        try
        {
            Evaluator eval = new Evaluator();
            int i = eval.evaluator(s, null);
            Console.WriteLine(i);
        }
        catch (ArgumentException)
        {
            // write a message saying that your code detected the invalid syntax of the formula
        }

    }
}
