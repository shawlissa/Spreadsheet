using System.Data;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;

/// <summary>
/// Class <c> FormulaEvaluator </c> evaluates any given integer arithmetic expressions using infix notation such as: formulas, values, variable, and exponents.
/// </summary>
namespace FormulaEvaluator {
    public class Evaluator {
        private bool firstToken = false; ///Determines in parseFormula whether first token is a variable or operation. True if variable, false if operation.
        Stack<string> variables = new Stack<string>();
        Stack<string> operations = new Stack<string>();
        public delegate int Lookup(string variable_name);

        /// <summary>
        /// Finds the sum of a given arithmetic expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="variableEvaluator"></param>
        /// <returns></returns>
        public int evaluator(string expression, Lookup variableEvaluator) {
            parseFormula(expression);
            ///Base case
            if (operations.Peek() == "(") {
                buildBracketFormula();
            }
            return 0;
        }

        /// <summary>
        /// Breaks up valid tokens into two stacks:
        /// Variables/values stack and exponent stack.  
        /// </summary>
        /// <param name="Formula"></param>
        /// <returns></returns>
        private void parseFormula(string formula) {
            ///Splits formula into tokens by each operator found
            string[] substrings = Regex.Split(formula, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            ///Organizes tokens into two stacks based on either operations or variables
            foreach (string s in substrings) {
                if (isOperator(s)) {
                    operations.Push(s);
                }
                else {
                    //Add more complicated nuances here; variables and whitespace
                    variables.Push(s);
                }
            }
        }

        /// <summary>
        /// Creates a string formula of all variables and operations within parenthesis then pushes into variable stack.
        /// Method call assumes first operation in stack to be "(".
        /// </summary>
        private void buildBracketFormula() {
            string formula = "";
            while (operations.Peek() != ")") {
                formula += operations.Pop(); 
                formula += variables.Pop();
            }
            formula += operations.Pop();
            variables.Push(formula);
        }
        private void evalInt() {
            if (operations.Peek() == "*" || operations.Peek() == "/") {
                String newVar = variables.Pop() + operations.Pop();
            }

        }
        private bool isOperator(string token) {
            return token == "+" || token == "-" || token == "*" || token == "/" || token == "(" || token == ")";
        }
    }
}
