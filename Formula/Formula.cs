// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens


using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        private string formula;
        private List<string> variables;

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
            if (formula == null)
                throw new FormulaFormatException("Formula cannot be null.");
            variables = new List<string>();
            this.formula = parseFormula(formula, s => s, s => true);

        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            variables = new List<string>();
            if (formula == null)
                throw new FormulaFormatException("Formula cannot be null.");
            this.formula = parseFormula(formula, normalize, isValid);
        }

        /// <summary>
        /// Breaks given formula into substrings, determined by every operator found.
        /// Skips any whitespace, string builds together any operators and float values found,
        /// any variables found are evaluated with normalize and isValid, if it passes through
        /// both then adds the variable to the formula.
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="norm"></param>
        /// <param name="valid"></param>
        /// <returns></returns>
        /// <exception cref="FormulaFormatException"></exception>
        private string parseFormula(string formula, Func<string, string> norm, Func<string, bool> valid)
        {
            StringBuilder sb = new StringBuilder();
            string[] substrings = splitFormula(formula);

            //Unable to use isOperator as it contains "(" and "(" is valid for first substring
            if (substrings[0] == ")" || substrings[0] == "+" || substrings[0] == "-" || substrings[0] == "/" || substrings[0] == "*")
                throw new FormulaFormatException("Formula must begin with a '(', variable, or value.");

            if (substrings[substrings.Length - 1] == "(" || substrings[substrings.Length - 1] == "+" || substrings[substrings.Length - 1] == "-" || substrings[substrings.Length - 1] == "/" || substrings[substrings.Length - 1] == "*")
                throw new FormulaFormatException("Formula must end with a ')', variable, or value.");

            foreach (string s in substrings)
            {
                //Skip whitespaces
                if (s.Contains(" ") || s == "")
                    continue;
                if (isOperator(s))
                    sb.Append(s);
                else if (int.TryParse(s, out int result))
                    sb.Append(s);
                else if (decimal.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal dec)) {
                    sb.Append(s);
                }
                else
                {
                    //Verifies validity of variable & normalizes
                    string normS = norm(s);
                    if (valid(norm(normS)))
                    {
                        sb.Append(normS);
                        variables.Add(normS);
                    }
                    else
                        throw new FormulaFormatException("Not a valid variable.");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns the formula in a split list version
        /// </summary>
        /// <param name="formula"></param>
        /// <returns></returns>
        private string[] splitFormula(string formula)
        {
            return Regex.Split(formula, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)|(\\s+)");
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            Stack<double> values = new Stack<double>();
            Stack<string> operations = new Stack<string>();
            double sum = 0;
            string[] substrings = splitFormula(this.formula);

            ///Splits formula into tokens by each operator found.
            foreach (string s in substrings)
            {

                //No amount of parsing is able to remove "" unfortunately still need these statements
                if (s == "")
                    continue;
                //If token is an operator -> figure out which one it is and analyze.
                if (isOperator(s))
                {
                    evaluateOp(s, operations, values);
                }
                else if (double.TryParse(s, out double result))
                {
                    evaluateDouble(result, operations, values);
                }
                //For scientific notation
                else if (decimal.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal dec))
                {
                    evaluateDouble((double)dec, operations, values);
                }
                else
                {
                    double varVal = (double)lookup(s);
                    if (varVal == -1)
                        new FormulaError("Invalid variable.");
                    evaluateDouble(varVal, operations, values);
                }
            }
            //Ensures enough values in both stacks before evaluating.
            while (operations.Count > 0 && values.Count > 1)
            {
                if (operations.Peek() == "-" || operations.Peek() == "+")
                {
                    evaluateOp(operations.Peek(), operations, values);
                    operations.Pop();
                }
                //Excess parenthesis not previously dealt with
                else if (operations.Peek() == "(")
                    operations.Pop();
            }
            //Base case for if no values in expression
            if (operations.Count == 1)
                throw new FormulaFormatException("Must have values or variables in expression to be valid.");
            //Can assume sum is at top of variable stack after previous operations.
            sum = values.Pop();
            return sum;
        }

        /// <summary>
        /// Evaluates the given valid operator to determine which valid operator it is.
        /// </summary>
        /// <param name="s"></param>
        private static void evaluateOp(string s, Stack<string> op, Stack<double> val)
        {
            if (s.Equals("+") || s.Equals("-"))
            {
                //Prevents exception.
                if (op.Count != 0 && val.Count >= 2 && (op.Peek() == "+" || op.Peek() == "-"))
                {
                    //If operations equal - or + then evaluate and push result into variables.
                    subOrAdd(op, val);
                    op.Pop();
                }
                op.Push(s);
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
                    //if (op.Peek() == "(")
                    //    throw new FormulaFormatException("Parenthesis statement must contain operations.");
                    if (op.Peek() == "+" || op.Peek() == "-")
                    {
                        subOrAdd(op, val);
                        //Remove + or -
                        op.Pop();
                    }

                    ///Operations.Peek should absolutely equals ( however is a fail safe.
                    if (op.Peek() == "(")
                        op.Pop();


                    if (op.Count == 0)
                        throw new FormulaFormatException("Must have a complete parenthesis statement.");
                    if (op.Peek() == "*" || op.Peek() == "/")
                    {
                        multOrDivi(op, val);
                    }
                }

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
        private static void evaluateDouble(double s, Stack<string> op, Stack<double> val)
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
        private static bool isOperator(string token) { return token == "+" || token == "-" || token == "*" || token == "/" || token == "(" || token == ")"; }

        /// <summary>
        /// Determines whether subtracting or adding then evaluates if one of those signs is at top of operator stack.
        /// </summary>
        private static void subOrAdd(Stack<string> op, Stack<double> val)
        {
            if (op.Peek() == "-")
            {
                double sub = subtract(val);
                val.Push(sub);
            }
            else if (op.Peek() == "+")
            {
                double addition = add(val);
                val.Push(addition);
            }
        }

        /// <summary>
        /// Adds the next two variables at the top of the variable stack.
        /// </summary>
        /// <returns></returns>
        private static double add(Stack<double> val)
        {
            double val1 = val.Pop();
            double val2 = val.Pop();
            return val1 + val2;
        }

        /// <summary>
        /// Subtracts the next two variables at the top of the variable stack.
        /// </summary>
        /// <returns></returns>
        private static double subtract(Stack<double> val)
        {
            double val1 = val.Pop();
            double val2 = val.Pop();
            return val2 - val1;
        }

        /// <summary>
        /// Determines whether multiplying or dividing.
        /// </summary>
        private static void multOrDivi(Stack<string> op, Stack<double> val)
        {
            if (op.Peek() == "*")
            {
                double mult = multiply(val);
                val.Push(mult);
                op.Pop();
            }
            else if (op.Peek() == "/")
            {
                double divi = divide(val);
                val.Push(divi);
                op.Pop();
            }

            ////Multiplying/dividing inside parenthesis leaves extra "(" to deal with.
            //if (op.Peek() == "(")
            //    op.Pop();
        }
        /// <summary>
        /// Multiplies the next two variables at the top of the variable stack.
        /// </summary>
        /// <returns></returns>
        private static double multiply(Stack<double> val)
        {
            double val1 = val.Pop();
            double val2 = val.Pop();
            return val1 * val2;
        }

        /// <summary>
        /// Divides the next two variables at the top of the variable stack.
        /// </summary>
        /// <returns></returns>
        private static double divide(Stack<double> val)
        {
            double val1 = val.Pop();
            double val2 = val.Pop();
            if (val1 == 0 || val2 == 0)
                new FormulaError("Cannot divide by 0.");
            return val2 / val1;
        }


        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            return variables;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            return this.formula;
        }

        /// <summary>
        ///  <change> make object nullable </change>
        ///
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object? obj)
        {
            //Throws if obj is null
            if (obj == null)
                return false;
            //Throws if object is not type Formula
            if (!obj.GetType().Equals(typeof(Formula)))
                return false;

            //Converting obj into official Formula object to get Formula.ToString
            Formula otherFormula = (Formula)obj;
            string otherStr = otherFormula.ToString();

            if (otherStr.Length != this.formula.Length)
                return false;
            if (otherStr.Equals(this.formula))
                return true;

            return false;
        }

        /// <summary>
        ///   <change> We are now using Non-Nullable objects.  Thus neither f1 nor f2 can be null!</change>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// 
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            return f1.Equals(f2);
        }

        /// <summary>
        ///   <change> We are now using Non-Nullable objects.  Thus neither f1 nor f2 can be null!</change>
        ///   <change> Note: != should almost always be not ==, if you get my meaning </change>
        ///   Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return !f1.Equals(f2);
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            return this.formula.GetHashCode();
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory rOeason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }

    }
}

// <change>
//   If you are using Extension methods to deal with common stack operations (e.g., checking for
//   an empty stack before peeking) you will find that the Non-Nullable checking is "biting" you.
//
//   To fix this, you have to use a little special syntax like the following:
//
//       public static bool OnTop<T>(this Stack<T> stack, T element1, T element2) where T : notnull
//
//   Notice that the "where T : notnull" tells the compiler that the Stack can contain any object
//   as long as it doesn't allow nulls!
// </change>
