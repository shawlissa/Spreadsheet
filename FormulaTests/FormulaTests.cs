using SpreadsheetUtilities;
using System.Text;
namespace FormulaTests
{

    /// <summary>
    /// Test class for the 'Formula' Project.
    /// </summary>
    [TestClass]
    public class FormulaTests
    {
        Func<string, string> norm = (s) => s.ToUpper();
        Func<string, bool> valid = (s) =>
        { return s.Length >= 2; };
        //Formula Compile Tests: ---------------------------

        /// <summary>
        /// Verifies creating formula object compiles and uses both func objects correctly
        /// </summary>
        [TestMethod]
        public void FormulaCompile()
        {
            Assert.AreEqual("X2+Y2", new Formula("x2 + y2", norm, valid).ToString());
        }

        /// <summary>
        /// Expected to throw because of incorrect variable in normalizer
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException), "Formula syntax error.")]
        public void FormulaNormThrowFormatException()
        {
            new Formula("3x + yy", norm, valid);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException), "Is not a valid formula.")]
        public void FormulaValidThrowFormatException()
        {
            new Formula("x + y3", norm, valid);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException),"Formula cannot be null.")]
        public void FormulaNullComplie()
        {
            new Formula(null);
        }

        //GetVariables Tests: ----------------------

        /// <summary>
        /// Get single variable
        /// </summary>
        [TestMethod]
        public void GetSingleVariable()
        {
            foreach (string expected in new Formula("x1").GetVariables())
            {
                Assert.AreEqual("x1", expected);
            }
        }

        /// <summary>
        /// Get variables from a formula
        /// </summary>
        [TestMethod]
        public void GetVariablesSmallFormula()
        {
            int i = 1;
            foreach (string expected in new Formula("x1 + (x2 * x3)").GetVariables())
            {
                Assert.AreEqual("x" + i, expected);
                i++;
            }
        }

        /// <summary>
        /// Get variables from a scientific notation
        /// Scientific notation is not a variable thus should return nothing
        /// </summary>
        [TestMethod]
        public void GetVariablesSN()
        {
            new Formula("5e2").GetVariables();
            int i = 1;
            foreach (string expected in new Formula("5e2").GetVariables())
            {
                Assert.AreEqual("", expected);
                i++;
            }
        }

        /// <summary>
        /// Get variables from a large complex formula
        /// </summary>
        [TestMethod]
        public void GetVariablesComplexFormula()
        {
            int i = 1;
            foreach (string expected in new Formula("x1 * (x2 + x3) / x4 + x5 * (x6 * (x7 * (x8 + x9))) - x10").GetVariables())
            {
                Assert.AreEqual("x" + i, expected);
                i++;
            }
        }

        /// <summary>
        /// Get variables from a formula using norm and valid
        /// </summary>
        [TestMethod]
        public void GetVariablesFormulaWithNormAndValid()
        {
            int i = 1;
            foreach (string expected in new Formula("x1 + (x2 * x3)", norm, valid).GetVariables())
            {
                Assert.AreEqual("X" + i, expected);
                Assert.AreNotEqual("x" + i, expected);
                i++;
            }
        }

        //ToString Tests: -----------------------------
        /// <summary>
        /// C# overridden ToString method removes whitespace from given formula
        /// </summary>
        [TestMethod]
        public void ToStringRemovesWhitespace()
        {
            string actual = new Formula("x1 + y1").ToString();
            Assert.AreEqual("x1+y1", actual);
        }

        /// <summary>
        /// Converts a scientific notation formula toString
        /// </summary>
        [TestMethod]
        public void ToStringSN()
        {
            string actual = new Formula("5e2").ToString();
            Assert.AreEqual("5e2", actual);
        }

        /// <summary>
        /// C# overridden ToString method removes whitespace from given formula and applies norm and valid funcs
        /// </summary>
        [TestMethod]
        public void ToStringRemovesWhitespaceWithNormAndValid()
        {
            string actual = new Formula("x2  +  y2", norm, valid).ToString();
            Assert.IsFalse(actual.Contains(" "));
        }

        /// <summary>
        /// C# overridden ToString method equals version with whitespace from given formula and applies norm and valid funcs
        /// </summary>
        [TestMethod]
        public void ToStringEqualsWithNormAndValid()
        {
            Assert.AreEqual("X2+Y2", new Formula("x2  +  y2", norm, valid).ToString());
        }

        //Equals Tests: ----------------------------------
        /// <summary>
        /// Formula does not equal null
        /// </summary>
        [TestMethod]
        public void EqualsNull()
        {
            Assert.IsFalse(new Formula("x1").Equals(null));
        }

        /// <summary>
        /// Formula does not equal a non formula
        /// </summary>
        [TestMethod]
        public void EqualsNonFormula()
        {
            Assert.IsFalse(new Formula("x1").Equals("x1"));
        }

        /// <summary>
        /// Formula does not equal a non formula
        /// </summary>
        [TestMethod]
        public void EqualsNotSameFormula()
        {
            Assert.IsFalse(new Formula("x1").Equals("x1 + x2"));
        }


        /// <summary>
        /// Equals a simple variable 'x'.
        /// </summary>
        [TestMethod]
        public void EqualsVariable()
        {
            Assert.IsTrue(new Formula("x1").Equals(new Formula("x1")));
        }

        /// <summary>
        /// Equals a simple variable 'x'.
        /// </summary>
        [TestMethod]
        public void EqualsVariableWithNormValid()
        {
            Assert.IsTrue(new Formula("x1", norm, valid).Equals(new Formula("X1")));
        }

        /// <summary>
        /// Equals a simple formula 'x + y'.
        /// </summary>
        [TestMethod]
        public void EqualsSimpleFormula()
        {
            Assert.IsTrue(new Formula("x1 + y1", norm, valid).Equals(new Formula("X1+Y1")));
        }

        /// <summary>
        /// Equals a simple formula 'x + y' after being normalized and validated.
        /// </summary>
        [TestMethod]
        public void EqualsSimpleFormulaWithNormValid()
        {
            Assert.IsTrue(new Formula("x1 + y1", norm, valid).Equals(new Formula("X1+Y1")));
        }

        /// <summary>
        /// Equals a complex formula
        /// </summary>
        [TestMethod]
        public void EqualsComplexFormula()
        {
            Assert.IsTrue(new Formula("x1 * (x2 + x3) / x4 + x5 * (x6 * (x7 * (x8 + x9))) - x10").Equals(new Formula("x1*(x2+x3)/x4+x5*(x6*(x7*(x8+x9)))-x10")));
        }

        /// <summary>
        /// Equals a complex formula after being normalized and validated.
        /// </summary>
        [TestMethod]
        public void EqualsComplexFormulaWithNormValid()
        {
            Assert.IsTrue(new Formula("x1 * (x2 + x3) / x4 + x5 * (x6 * (x7 * (x8 + x9))) - x10",norm, valid).Equals(new Formula("X1*(X2+X3)/X4+X5*(X6*(X7*(X8+X9)))-X10")));
        }

        /// <summary>
        /// Two simple formulas do not equal eachother
        /// </summary>
        [TestMethod]
        public void EqualsFalseSimple()
        {
            Assert.IsFalse(new Formula("x1 + y1", norm, valid).Equals(new Formula("X1+Z1")));
        }

        /// <summary>
        /// Two complex formulas do not equal eachother from 1 off variable.
        /// </summary>
        [TestMethod]
        public void EqualsFalseComplex()
        {
            Assert.IsFalse(new Formula("x1 * (x2 + x3) / x4 + x5 * (x6 * (x7 * (x8 + x9))) - x10", norm, valid).Equals(new Formula("X1*(X2+X3)/X4+X5*(X6*(X7*(X8+X9)))-X11")));
        }

        //Operator== Tests: -----------------------------------
        /// <summary>
        /// Verifies equality of a variable using ==
        /// </summary>
        [TestMethod]
        public void OperatorEqualsVariable()
        {
            Assert.IsTrue(new Formula("x1", norm, valid) == new Formula("X1"));
        }
        /// <summary>
        /// Verifies equality of simple formulas using ==
        /// </summary>
        [TestMethod]
        public void OperatorEqualsSimpleFormula()
        {
            Assert.IsTrue(new Formula("x1 + y1", norm, valid) == new Formula("X1+Y1"));
        }

        /// <summary>
        /// Verifies equality of complex formulas using ==
        /// </summary>
        [TestMethod]
        public void OperatorEqualsComplexFormula()
        {
            Assert.IsTrue(new Formula("x1 * (x2 + x3) / x4 + x5 * (x6 * (x7 * (x8 + x9))) - x10", norm, valid) == new Formula(("X1*(X2+X3)/X4+X5*(X6*(X7*(X8+X9)))-X10")));
        }

        //Operator!= Tests: ---------------------------------

        /// <summary>
        /// Verifies inequality of a variable using !=
        /// </summary>
        [TestMethod]
        public void OperatorNotEqualsVariable()
        {
            Assert.IsTrue(new Formula("x1", norm, valid) != new Formula("x1"));
        }
        /// <summary>
        /// Verifies equality of simple formulas using !=
        /// </summary>
        [TestMethod]
        public void OperatorotEqualsSimpleFormula()
        {
            Assert.IsTrue(new Formula("x1 + y1", norm, valid) != new Formula("x1+y1"));
        }

        /// <summary>
        /// Verifies inequality of complex formulas using !=
        /// </summary>
        [TestMethod]
        public void OperatorNotEqualsComplexFormula()
        {
            Assert.IsTrue(new Formula("x1 * (x2 + x3) / x4 + x5 * (x6 * (x7 * (x8 + x9))) - x10", norm, valid) != new Formula(("X1*(X2+X3)/X4+X5*(X6*(X7*(X8+X9)))-X11")));
        }

        //Evaluate Tests: ----------------------------


        //[TestMethod]
        //[ExpectedException(typeof(FormulaError), "Formula syntax error.")]
        //public void EvaluateNonExistentVariable()
        //{
        //    new Formula("y1").Evaluate(s => (s == "x1") ? 1 : -1);
        //}

        /// <summary>
        /// Evaluates x using lookup function
        /// </summary>
        [TestMethod]
        public void EvaluateVariable()
        {
            Assert.AreEqual(5.0, new Formula("x1").Evaluate((s) => 5));
        }

        /// <summary>
        /// Evaluates simple formula with variables of same value using lookup function
        /// </summary>
        [TestMethod]
        public void EvaluateSimpleFunction()
        {
            Assert.AreEqual(20.0, new Formula("x1 + (y1 * 3)").Evaluate((s) => 5));
        }

        /// <summary>
        /// Evaluates simple formula with variables of different values using lookup function
        /// </summary>
        [TestMethod]
        public void EvaluateSimpleFunctionComplexVariables()
        {
            Assert.AreEqual(8.0, new Formula("x1 + (y1 * 3)").Evaluate(s => (s == "x1") ? 5 : 1));
        }

        /// <summary>
        /// Evaluates simple formula with variables of same value using lookup function
        /// </summary>
        [TestMethod]
        public void EvaluateComplexFunction()
        {
            Assert.AreEqual(13.0, new Formula("x1 + (y1 * (3 + u1) / z1").Evaluate((s) => 5));
        }

        /// <summary>
        /// Evaluates simple formula with variables of different values using lookup function
        /// </summary>
        [TestMethod]
        public void EvaluateComplexFunctionComplexVariables()
        {
            Assert.AreEqual(17.0, new Formula("x1 + (y1 * (3 + u1) / z1").Evaluate(s => (s == "x1") ? 5 : (s == "y1") ? 2 : (s == "u1") ? 3: 1));
        }

        //[TestMethod]
        //[ExpectedException(typeof(FormulaError), "Formula syntax error.")]
        //public void EvaluateDivisionByZero()
        //{
        //    new Formula("x1 + (y1 * (3 + u1) / z1").Evaluate(s => (s == "x1") ? 5 : (s == "y1") ? 2 : (s == "u1") ? 3 : 0);
        //}

        /// <summary>
        /// Evaluates 10e2 scientific notation
        /// </summary>
        [TestMethod]
        public void EvaluateSN()
        {
            Assert.AreEqual(1000, 10e2);
        }

        [TestMethod]
        public void EvaluateComplexFormulaWithSN()
        {
            Assert.AreEqual(1017.0, new Formula("x1 + (y1 * (3 + u1) / z1 + 10e2").Evaluate(s => (s == "x1") ? 5 : (s == "y1") ? 2 : (s == "u1") ? 3 : 1));
        }

        /// <summary>
        /// Repeatedly creates new random operators; +, -, /, or *
        /// Reapeatedly creates a new double
        /// Appends the operator then the double
        /// Creates a new formula based on current string builder and evaluates
        /// </summary>
        [TestMethod]
        public void EvaluateStressTest()
        {
            Formula form;
            StringBuilder sb = new StringBuilder();
            sb.Append(1);
            for (int i = 0; i < 10; i++)
            {
                //Adds and operator 
                if (i % 4 == 0)
                    sb.Append('+');
                if (i % 4 == 1)
                    sb.Append('-');
                if (i % 4 == 2)
                    sb.Append('*');
                if (i % 4 == 3)
                    sb.Append("/");

                //Adds a variable

                if (i % 10 == 0)
                    sb.Append(10);
                if (i % 10 == 1)
                    sb.Append(1);
                if (i % 10 == 2)
                    sb.Append(2);
                if (i % 10 == 3)
                    sb.Append(3);
                if (i % 10 == 4)
                    sb.Append(4);
                if (i % 10 == 5)
                    sb.Append(5);
                if (i % 10 == 6)
                    sb.Append(6);
                if (i % 10 == 7)
                    sb.Append(7);
                if (i % 10 == 8)
                    sb.Append(8);
                if (i % 10 == 9)
                    sb.Append(9);

            }
            string s = sb.ToString();
            form = new Formula(sb.ToString());
            Assert.AreEqual(9.047619047619047, form.Evaluate((s) => 0));
        }
    }
}