using System;
using System.Data;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Security;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Windows.Markup;

/// <summary>
/// Class <c> FormulaEvaluator </c> evaluates any given integer arithmetic expressions using infix notation such as: formulas, values, variable, and exponents.
/// </summary>
namespace FormulaEvaluator {
   static public class Evaluator {
        public delegate int Lookup(string variable_name);

        /// <summary>
        /// Finds the sum of a given arithmetic expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="variableEvaluator"></param>
        /// <returns></returns>
        static public int Evaluate(string expression, Lookup variableEvaluator)
        {
            if (expression == "" || expression == " ")
                throw new ArgumentException("Expression cannot be empty.");
            Stack<int> values = new Stack<int>();
            Stack <string> operations = new Stack<string>();
            int sum = 0;
            ///Splits formula into tokens by each operator found.
            string[] substrings = Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            foreach (string s in substrings)
            {
                //Skips any spacing found.
                if (s == "" || s == " ")
                    continue;
                //If token is an operator -> figure out which one it is and analyze.
                if (isOperator(s))
                {
                    evaluateOp(s, operations, values);
                //Is an integer.
                } else if (int.TryParse(s, out int result)) {
                    evaluateInt(result, operations, values);
                //Is a variable.
                } else if (variableEvaluator != null)
                {
                    {
                        evaluateVariable(s);
                        int val = variableEvaluator(s);
                        evaluateInt(val, operations, values);
                    }
                }
            }
            //Ensures enough values in both stacks before evaluating.
            while (operations.Count > 0 && values.Count > 1)
            {
                if (operations.Peek() == "*" || operations.Peek() == "/")
                {
                    evaluateInt(values.Pop(), operations, values);
                } else if (operations.Peek() == "-" || operations.Peek() == "+")
                {
                    evaluateOp(operations.Peek(), operations, values);
                    operations.Pop();
                }
            }
            //Base case for if no values in expression
            if (operations.Count == 1)
                throw new ArgumentException("Must have values or variables in expression to be valid.");
            //Can assume sum is at top of variable stack after previous operations.
            sum = values.Pop();
            return sum;
        }

        /// <summary>
        /// Evaluates the given valid operator to determine which valid operator it is.
        /// </summary>
        /// <param name="s"></param>
        private static void evaluateOp(string s, Stack<string> op, Stack<int> val)
        {
            if (s.Equals("+") || s.Equals("-"))
            {
                //Prevents exception.
                if (op.Count != 0 && val.Count >= 2 && (op.Peek() == "+" || op.Peek() == "-"))
                {
                    //If operations equal - or + then evaluate and push result into variables.
                    subOrAdd(op, val);
                }  else
                {
                    op.Push(s);
                }
            }
            //If operation equals *, /, or ( then simply push operand into operations stack.
            else if (s.Equals("*") || s.Equals("/") || s.Equals("("))
            {
                op.Push(s);
            }
            //If operations equals ) then end of a parenthesis statement -> evaluate and remove ( from stack.
            else if (s.Equals(")"))
            {
                if (val.Count >= 2)
                {
                    if (op.Peek() == "(")
                        throw new ArgumentException("Parenthesis statement must contain operations.");
                    if (op.Peek() == "+" || op.Peek() == "-")
                    {
                        subOrAdd(op, val);
                    }

                    op.Pop();

                    
                    if (op.Count == 0)
                        throw new ArgumentException("Must have a complete parenthesis statement.");
                    if (op.Peek() == "*" || op.Peek() == "/")
                    {
                        multOrDivi(op, val);
                    }
                }
                ///Operations.Peek should absolutely equals ( however is a fail safe.
                if (op.Peek() == "(")
                    op.Pop();

                //Check for multiplication/division outside of parenthesis
                if (op.Count > 0 && (op.Peek() == "*" || op.Peek() == "/"))
                {
                    multOrDivi(op, val);
                }
            }
        }

        /// <summary>
        /// Push s into variables and then if top of operations stack is a * or / 
        /// then evaluate otherwise dont do anything.
        /// </summary>
        /// <param name="s"></param>
        private static void evaluateInt(int s, Stack<string> op, Stack<int> val)
        {
            val.Push(s);
            if (op.Count > 0 && val.Count > 1)
            {
               if (op.Peek() == "*" || op.Peek() == "/")
                {
                    multOrDivi(op, val);
                }
            }
        }
        /// <summary>
        /// Determines if given token is an operator or not.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
       private static bool isOperator(string token)
        {
            return token == "+" || token == "-" || token == "*" || token == "/" || token == "(" || token == ")";
        }

        /// <summary>
        /// Determines whether subtracting or adding then evaluates if one of those signs is at top of operator stack.
        /// </summary>
        private static void subOrAdd(Stack<string> op, Stack<int> val)
        {
            if (op.Peek() == "-")
            {
                int sub = subtract(val);
                val.Push(sub);
            }
            else if (op.Peek() == "+")
            {
                int addition = add(val);
                val.Push(addition);
            }
        }

        /// <summary>
        /// Adds the next two variables at the top of the variable stack.
        /// </summary>
        /// <returns></returns>
        private static int add(Stack<int> val)
        {
            int val1 = val.Pop();
            int val2 = val.Pop();
            return val1 + val2;
        }

        /// <summary>
        /// Subtracts the next two variables at the top of the variable stack.
        /// </summary>
        /// <returns></returns>
        private static int subtract(Stack<int> val)
        {
            int val1 = val.Pop();
            int val2 = val.Pop();
            return val2 - val1;          
        }

        /// <summary>
        /// Determines whether multiplying or dividing.
        /// </summary>
        private static void multOrDivi(Stack<string> op, Stack<int> val)
        {
            if (op.Peek() == "*")
            {
                int mult = multiply(val);
                val.Push(mult);
                op.Pop();
            }
            else if (op.Peek() == "/")
            {
                int divi = divide(val);
                val.Push(divi);
                op.Pop();
            }
        }
        /// <summary>
        /// Multiplies the next two variables at the top of the variable stack.
        /// </summary>
        /// <returns></returns>
        private static int multiply(Stack<int> val)
        {
            int val1 = val.Pop();
            int val2 = val.Pop();
            return val1 * val2;
        }
        
        /// <summary>
        /// Divides the next two variables at the top of the variable stack.
        /// </summary>
        /// <returns></returns>
        private static int divide(Stack<int> val)
        {
            int val1 = val.Pop();
            int val2 = val.Pop();
            if (val1 == 0 || val2 == 0)
                throw new ArgumentException("Cannot divide by 0.");
            return val2 / val1;
        }

        /// <summary>
        /// Verifies validity of given variable
        /// </summary>
        /// <param name="s"></param>
        /// <exception cref="ArgumentException"></exception>
        private static void evaluateVariable(string s)
        {
            bool isInt = false;
            string[] substrings = Regex.Split(s, "");

            //First substring is not a letter -> throw argumentException
            if (Regex.Matches(substrings[1], @"[a-zA-Z]").Count == 0)
                throw new ArgumentException("Variable must start with a Letter A -> Z.");
            foreach(string str in substrings)
            {
                //Int value found
                if (Regex.Matches(str, @"[0-9]").Count > 0)
                {
                    isInt = true;
                }
                //If variable already has int values but switches back to letters -> throw (ex A1A)
                if (isInt)
                    if (Regex.Matches(str, @"[a-zA-Z]").Count > 0)
                        throw new ArgumentException("Cannot follow ints by letters in variables.");
            }

            //If no int value at end -> throw
            if (!isInt)
                throw new ArgumentException("Variable must end with an int value.");
        }
    }
}