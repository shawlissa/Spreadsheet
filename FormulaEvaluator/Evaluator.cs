using System;
using System.Data;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Security;
using System.Text.RegularExpressions;

/// <summary>
/// Class <c> FormulaEvaluator </c> evaluates any given integer arithmetic expressions using infix notation such as: formulas, values, variable, and exponents.
/// </summary>
namespace FormulaEvaluator {
    public class Evaluator {
        Stack<string> variables = new Stack<string>();
        Stack<string> operations = new Stack<string>();

        public delegate int Lookup(string variable_name);

        /// <summary>
        /// Finds the sum of a given arithmetic expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="variableEvaluator"></param>
        /// <returns></returns>
        public int Evaluate(string expression, Lookup variableEvaluator)
        {
            variables = new Stack<string>();
            operations = new Stack<string>();
            string sum = "";
            ///Splits formula into tokens by each operator found.
            string[] substrings = Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            for (int i = 0; i < substrings.Length; i++)
            {
                string s = substrings[i];
                s = s.Replace(" ", "");
                //Skips any non operator found.
                if (s == "" || s == " " || isNonOperator(s))
                {
                    continue;
                }
                //If token is an operator -> figure out which one it is and analyze.
                if (isOperator(s))
                {
                    evaluateOp(s);
                //Is an integer.
                } else if (isInteger(s)) {
                    evaluateInt(s);
                //Is a variable.
                } else if (variableEvaluator != null)
                {
                    {
                        int val = variableEvaluator(s);
                        evaluateInt("" + val);
                    }
                }
            }
            //Ensures enough values in both stacks before evaluating.
            while (operations.Count > 0 && variables.Count > 1)
            {
                if (operations.Peek() == "*" || operations.Peek() == "/")
                {
                    evaluateInt(variables.Pop());
                } else if (operations.Peek() == "-" || operations.Peek() == "+")
                {
                    evaluateOp(operations.Peek());
                    operations.Pop();
                }
            }
            //Can assume sum is at top of variable stack after previous operations.
            sum = variables.Pop();
            if (int.TryParse(sum, out int result))
            {
                return result;
            } else
            {
                return 0;
            }
        }

        /// <summary>
        /// Evaluates the given valid operator to determine which valid operator it is.
        /// </summary>
        /// <param name="s"></param>
        void evaluateOp(string s)
        {
            if (s.Equals("+") || s.Equals("-"))
            {
                //Prevents exception.
                if (operations.Count != 0 && variables.Count >= 2)
                {
                    //If operations equal - or + then evaluate and push result into variables.
                    subOrAdd();
                }  else
                {
                    operations.Push(s);
                }
            }
            //If operation equals *, /, or ( then simply push operand into operations stack.
            else if (s.Equals("*") || s.Equals("/") || s.Equals("("))
            {
                operations.Push(s);
            }
            //If operations equals ) then end of a parenthesis statement -> evaluate and remove ( from stack.
            else if (s.Equals(")"))
            {
                if (variables.Count >= 2)
                {
                    if (operations.Peek() == "+" || operations.Peek() == "-")
                    {
                        subOrAdd();
                    }
                    //Removing the "(" operator.
                    operations.Pop();

                    if (operations.Peek() == "*" || operations.Peek() == "/")
                    {
                        multOrDivi();
                    }
                }
                ///Operations.Peek should absolutely equals ( however is a fail safe.
                if (operations.Peek() == "(")
                    operations.Pop();
            }
        }

        /// <summary>
        /// Push s into variables and then if top of operations stack is a * or / 
        /// then evaluate otherwise dont do anything.
        /// </summary>
        /// <param name="s"></param>
        void evaluateInt(string s)
        {
            variables.Push(s);
            if (operations.Count > 0 && variables.Count > 1)
            {
               if (operations.Peek() == "*" || operations.Peek() == "/")
                {
                    multOrDivi();
                    operations.Pop();
                }
            }
        }

        /// <summary>
        /// Verifies if given operator is valid.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private bool isNonOperator(string s)
        {
            return s == "=" || s == "!" || s == "@" || s == "#" || s == "$" ||
                  s == "%" || s == "^" || s == "&" || s == "`" || s == "|" ||
                  s == ":" || s == ";" || s == "~" || s == "." || s == "," ||
                  s == ">" || s == "<" || s == "'";
        }

        /// <summary>
        /// Determines if given token is an operator or not.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        bool isOperator(string token)
        {
            return token == "+" || token == "-" || token == "*" || token == "/" || token == "(" || token == ")";
        }

        /// <summary>
        /// Determines if given value is a valid integer value.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        bool isInteger(string token)
        {
            return int.TryParse(token, out int result);
        }

        /// <summary>
        /// Determines whether subtracting or adding then evaluates if one of those signs is at top of operator stack.
        /// </summary>
        void subOrAdd()
        {
            if (operations.Peek() == "-")
            {
                string sub = subtract();
                variables.Push(sub);
            }
            else if (operations.Peek() == "+")
            {
                string addition = add();
                variables.Push(addition);
            }
        }

        /// <summary>
        /// Adds the next two variables at the top of the variable stack.
        /// </summary>
        /// <returns></returns>
        string add()
        {
            string val1 = variables.Pop();
            string val2 = variables.Pop();
            string added = "";
            if (int.TryParse(val1, out int result1) && int.TryParse(val2, out int result2))
            {
                int add = result1 + result2;
                added = "" + add;
            }
            //Fail safe as all variables should be dealt with beforehand.
            else
            {
                added = val1 + " + " + val2;
            }
            return added;
        }

        /// <summary>
        /// Subtracts the next two variables at the top of the variable stack.
        /// </summary>
        /// <returns></returns>
        string subtract()
        {
            string val1 = variables.Pop();
            string val2 = variables.Pop();
            string subtracted = "";
            if (int.TryParse(val1, out int result1) && int.TryParse(val2, out int result2))
            {
                int sub = result1 - result2;
                subtracted = "" + sub;
            }
            //Fail safe as all variables should be dealt with beforehand.
            else
            {
                subtracted = val1 + " - " + val2;
            }
            return subtracted;
        }

        /// <summary>
        /// Determines whether multiplying or dividing.
        /// </summary>
        void multOrDivi()
        {
            if (operations.Peek() == "*")
            {
                string mult = multiply();
                variables.Push(mult);
            }
            else if (operations.Peek() == "/")
            {
                string divi = divide();
                variables.Push(divi);
            }
        }
        /// <summary>
        /// Multiplies the next two variables at the top of the variable stack.
        /// </summary>
        /// <returns></returns>
        string multiply()
        {
            string val1 = variables.Pop();
            string val2 = variables.Pop();
            string multiplied = "";
            if (int.TryParse(val1, out int result1) && int.TryParse(val2, out int result2))
            {
                int multiply = result1 * result2;
                multiplied = "" + multiply;
            }
            //Fail safe as all variables should be dealt with beforehand.
            else
            {
                multiplied = val1 + " * " + val2;
            }
            return multiplied;
        }
        
        /// <summary>
        /// Divides the next two variables at the top of the variable stack.
        /// </summary>
        /// <returns></returns>
        string divide()
        {
            string val1 = variables.Pop();
            string val2 = variables.Pop();
            string divided = "";
            if (int.TryParse(val1, out int result1) && int.TryParse(val2, out int result2))
            {
                if (result1 == 0 || result2 == 0)
                {
                    throw new ArgumentException("Cannot divide with 0.");
                }
                int divide = result2 / result1;
                divided = "" + divide;
            }           
            //Fail safe as all variables should be dealt with beforehand.
            else
            {
                divided = val1 + " / " + val2;
            }
            return divided;
        }
    }
}