using Newtonsoft.Json.Linq;
using NuGet.Frameworks;
using SpreadsheetUtilities;
using SS;
using System.Security.Cryptography;

namespace SpreadsheetTests

///<summary>
///Tester class for the 'Spreadsheet' project.
///</summary>
{
    [TestClass]
    public class SpreadsheetTests
    {
        AbstractSpreadsheet sheet = new Spreadsheet();

        /// <summary>
        /// Initializes a generic small spreadsheet, 'sheet', to use among tests.
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
            sheet.SetCellContents("A1", 5); // 5
            sheet.SetCellContents("A2", 2); // 2
            sheet.SetCellContents("A3", "A1 + A2"); // 7
            sheet.SetCellContents("A4", "5 / A2"); // 2.5
            sheet.SetCellContents("A5", "A3 * A1"); // 35
            sheet.SetCellContents("B1", 3 * 2); // 6
            sheet.SetCellContents("B2", "B1 + A1"); // 11
            sheet.SetCellContents("B3", "A1 + A2 / A4 - ( B1 * B2 )"); // -60.2
            sheet.SetCellContents("B4", 10); // 10
            sheet.SetCellContents("B5", "5e2"); // 500              
        }
        //GetCellContents Tests: ----------------------------------
        /// <summary>
        /// 'Name' is not of type Cell
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsWrongType()
        {
            sheet.GetCellContents("AA");
        }

        /// <summary>
        /// Name is a nonempty cell
        /// </summary>
        [TestMethod]
        public void GetCellContentsValidNonEmptyName()
        {
            sheet.SetCellContents("A1", 5.0);
            Assert.AreEqual(5.0, sheet.GetCellContents("A1"));
        }

        /// <summary>
        /// Get contents of formula
        /// </summary>
        [TestMethod]
        public void GetCellContentsValidFormulaContent()
        {
            Assert.AreEqual("A1+A2", sheet.GetCellContents("A3"));
        }

        /// <summary>
        /// Get contents of formula
        /// </summary>
        [TestMethod]
        public void GetCellContentsValidComplexFormulaContent()
        {
            Assert.AreEqual("A1+A2/A4-(B1*B2)", sheet.GetCellContents("B3"));
        }

        /// <summary>
        /// Gets same cell content repeatedly
        /// </summary>
        [TestMethod]
        public void GetCellContentsStressTest()
        {
            for (int i = 0; i < 100; i++)
            {
                Assert.AreEqual("A1+A2/A4-(B1*B2)", sheet.GetCellContents("B3"));
            }
        }

        /// <summary>
        /// Gets different cell content repeatedly
        /// </summary>
        [TestMethod]
        public void GetDifferingCellContentsStressTest()
        {
            for (int i = 0; i < 100; i++)
            {
                Assert.AreEqual("A1+A2/A4-(B1*B2)", sheet.GetCellContents("B3"));
            }
            for (int i = 0; i < 100; i++)
            {
                Assert.AreEqual("B1+A1", sheet.GetCellContents("B2"));
            }
            for (int i = 0; i < 100; i++)
            {
                Assert.AreEqual(6.0, sheet.GetCellContents("B1"));
            }
            for (int i = 0; i < 100; i++)
            {
                Assert.AreEqual(5.0, sheet.GetCellContents("A1"));
            }
            for (int i = 0; i < 100; i++)
            {
                Assert.AreEqual(2.0, sheet.GetCellContents("A2"));
            }
        }
        //SetCellContents Tests: ----------------------------------

        /// <summary>
        /// Sets cell content with no dependencies
        /// </summary>
        [TestMethod]
        public void SetCellContentsNoDependency()
        {
            //Only one cell name should be in set
            HashSet<string> cells = (HashSet<string>)sheet.SetCellContents("B4", 1);
            foreach (string s in cells)
            {
                Assert.AreEqual("B4", s);
            }
        }

        /// <summary>
        /// Sets cell content with one dependencies
        /// </summary>
        [TestMethod]
        public void SetCellContentsOneDependency()
        {
            HashSet<string> cells = (HashSet<string>)sheet.SetCellContents("A3", 1);
            IEnumerator<string> expected = cells.GetEnumerator();
            expected.MoveNext();
            Assert.AreEqual("A3", expected.Current);
            expected.MoveNext();
            Assert.AreEqual("A5", expected.Current);
        }

        /// <summary>
        /// Sets cell content with two dependencies
        /// </summary>
        [TestMethod]
        public void SetCellContentsTwoDependencies()
        {
            sheet.SetCellContents("B4", "A3 + 3");
            //Only one cell name should be in set
            HashSet<string> cells = (HashSet<string>)sheet.SetCellContents("A3", 1);
            IEnumerator<string> expected = cells.GetEnumerator();
            expected.MoveNext();
            Assert.AreEqual("A3", expected.Current);
            expected.MoveNext();
            Assert.AreEqual("B4", expected.Current);
            expected.MoveNext();
            Assert.AreEqual("A5", expected.Current);
        }

        /// <summary>
        /// Verifies can set cell contents with text type
        /// </summary>
        [TestMethod]
        public void SetCellContentText()
        {
            AbstractSpreadsheet emptySheet = new Spreadsheet();
            emptySheet.SetCellContents("C4", "7");
            foreach (string actual in emptySheet.GetNamesOfAllNonemptyCells())
                Assert.AreEqual("C4", actual);
        }

        /// <summary>
        /// Verifies can set cell contents with double type
        /// </summary>
        [TestMethod]
        public void SetCellContentDouble()
        {
            AbstractSpreadsheet emptySheet = new Spreadsheet();
            emptySheet.SetCellContents("C4", 3.0);
            foreach (string actual in emptySheet.GetNamesOfAllNonemptyCells())
                Assert.AreEqual("C4", actual);
        }

        /// <summary>
        /// Verifies can set cell contents with int type
        /// </summary>
        [TestMethod]
        public void SetCellContentInt()
        {
            AbstractSpreadsheet emptySheet = new Spreadsheet();
            emptySheet.SetCellContents("C4", 3);
            foreach (string actual in emptySheet.GetNamesOfAllNonemptyCells())
                Assert.AreEqual("C4", actual);
        }
        /// <summary>
        /// Sets cell contents for a 5 x 5 spreadsheet
        /// Ranging A -> E; 1 -> 5
        /// A1 content = A2; A2 content = A3... A5 content = A1
        /// B1 content = B2; B2 content = B3... B5 content = B1
        /// ... E5 content = E1
        /// </summary>
        [TestMethod]
        public void SetCellContentsMediumSpreadsheet()
        {
            AbstractSpreadsheet emptySheet = new Spreadsheet();
            string cellName = "";
            string cellContent = "";
            for (int i = 0; i < 5; i++) // Letters
            {
                for (int j = 1; j <= 5; j++) // Numbers
                {
                    cellName = "" + (char)('a' + i) + j;
                    if (j % 2 == 1) // Odd number cell
                    {
                        emptySheet.SetCellContents(cellName, j);
                    }
                    else // Even number cell
                    {
                        // Depend on the previous cell
                        string dependency = "" + (char)('a' + i) + (j - 1);
                        emptySheet.SetCellContents(cellName, dependency);
                    }
                }
            }
            //Verify dependencies
            for (int i = 0; i < 5; i++) // Letters
            {
                for (int j = 1; j <= 5; j++) // Numbers
                {
                    cellName = "" + (char)('a' + i) + j;
                    if (j % 2 == 1) // Odd number cell
                    {
                        Assert.AreEqual((double)j, emptySheet.GetCellContents(cellName));
                    }
                    else // Even number cell
                    {
                        // Depend on the previous cell
                        string dependency = "" + (char)('A' + i) + (j - 1);
                        Assert.AreEqual(dependency, emptySheet.GetCellContents(cellName));
                    }
                }
            }

        }

        //GetNamesOfAllNonemptyCells Tests: ----------------------------------
        /// <summary>
        /// Returns names of all nonempty cells in 'sheet' Spreadsheet
        /// </summary>
        [TestMethod]
        public void GetNamesOfAllNonemptyCellsCellExists()
        {
            List<string> names = new List<string> { "A1", "A2", "A3", "A4", "A5", "B1", "B2", "B3", "B4", "B5" };
            IEnumerator<string> actual = names.GetEnumerator();
                foreach(string expected in sheet.GetNamesOfAllNonemptyCells())
            {
                actual.MoveNext();
                Assert.AreEqual(actual.Current, expected);
            }
        }

        /// <summary>
        /// Returns empty enumerable for an empty sheet
        /// </summary>
        [TestMethod]
        public void GetNamesOfAllNonemptyCellsEmptySheet()
        {
            AbstractSpreadsheet emptySheet = new Spreadsheet();
            foreach (string expected in emptySheet.GetNamesOfAllNonemptyCells())
            {
                Assert.AreEqual(null, expected);
            }
        }

        /// <summary>
        /// Returns names of all nonempty cells, adds cells, then re-retrieves all nonempty cell names.
        /// </summary>
        [TestMethod]
        public void GetNamesOfAllNonemptyCellsEditCells()
        {
            List<string> names = new List<string> { "A1", "A2", "A3", "A4", "A5", "B1", "B2", "B3", "B4", "B5" };
            IEnumerator<string> actual = names.GetEnumerator();
            foreach (string expected in sheet.GetNamesOfAllNonemptyCells())
            {
                actual.MoveNext();
                Assert.AreEqual(actual.Current, expected);
            }

            //Adding more cells
            sheet.SetCellContents("B6", 4);
            sheet.SetCellContents("A6", 12);
            names.Add("B6");
            names.Add("A6");
            actual = names.GetEnumerator();

            foreach (string expected in sheet.GetNamesOfAllNonemptyCells())
            {
                actual.MoveNext();
                Assert.AreEqual(actual.Current, expected);
            }
        }

        /// <summary>
        /// Stress Test for GetNamesOfAllNonEmptyCells
        /// </summary>
        [TestMethod]
        public void GetNamesOfAllNonemptyCellsStressTest()
        {
            AbstractSpreadsheet emptySheet = new Spreadsheet();
            List<string> names = new List<string>();
            for (int i = 1; i < 10; i++)
            {
                emptySheet.SetCellContents("A" + i, i);
                names.Add("A" + i);
            }

            //"Empty" sheet now contains A1 -> A9
            IEnumerator<string> actual = names.GetEnumerator();
            foreach (string expected in emptySheet.GetNamesOfAllNonemptyCells())
            {
                actual.MoveNext();
                Assert.AreEqual(actual.Current, expected);
            }

            for (int i = 1; i < 10; i++)
            {
                emptySheet.SetCellContents("B" + i, i);
                names.Add("B" + i);
            }

            //"Empty" sheet now contains B1 -> B9
            actual = names.GetEnumerator();
            foreach (string expected in emptySheet.GetNamesOfAllNonemptyCells())
            {
                actual.MoveNext();
                Assert.AreEqual(actual.Current, expected);
            }

            for (int i = 1; i < 10; i++)
            {
                emptySheet.SetCellContents("C" + i, i);
                names.Add("C" + i);
            }

            //"Empty" sheet now contains C1 -> C9
            actual = names.GetEnumerator();
            foreach (string expected in emptySheet.GetNamesOfAllNonemptyCells())
            {
                actual.MoveNext();
                Assert.AreEqual(actual.Current, expected);
            }

            for (int i = 1; i < 10; i++)
            {
                emptySheet.SetCellContents("D" + i, i);
                names.Add("D" + i);
            }

            //"Empty" sheet now contains D1 -> D9
            actual = names.GetEnumerator();
            foreach (string expected in emptySheet.GetNamesOfAllNonemptyCells())
            {
                actual.MoveNext();
                Assert.AreEqual(actual.Current, expected);
            }

            for (int i = 1; i < 10; i++)
            {
                emptySheet.SetCellContents("E" + i, i);
                names.Add("E" + i);
            }

            //"Empty" sheet now contains E1 -> E9
            actual = names.GetEnumerator();
            foreach (string expected in emptySheet.GetNamesOfAllNonemptyCells())
            {
                actual.MoveNext();
                Assert.AreEqual(actual.Current, expected);
            }

            for (int i = 1; i < 10; i++)
            {
                emptySheet.SetCellContents("F" + i, i);
                names.Add("F" + i);
            }

            //"Empty" sheet now contains F1 -> F9
            actual = names.GetEnumerator();
            foreach (string expected in emptySheet.GetNamesOfAllNonemptyCells())
            {
                actual.MoveNext();
                Assert.AreEqual(actual.Current, expected);
            }

            for (int i = 1; i < 10; i++)
            {
                emptySheet.SetCellContents("G" + i, i);
                names.Add("G" + i);
            }

            //"Empty" sheet now contains G1 -> G9
            actual = names.GetEnumerator();
            foreach (string expected in emptySheet.GetNamesOfAllNonemptyCells())
            {
                actual.MoveNext();
                Assert.AreEqual(actual.Current, expected);
            }

            for (int i = 1; i < 10; i++)
            {
                emptySheet.SetCellContents("H" + i, i);
                names.Add("H" + i);
            }

            //"Empty" sheet now contains H1 -> H9
            actual = names.GetEnumerator();
            foreach (string expected in emptySheet.GetNamesOfAllNonemptyCells())
            {
                actual.MoveNext();
                Assert.AreEqual(actual.Current, expected);
            }

        }

        //Extra Testing: ----------------------------

        /// <summary>
        /// Testing invalid type of content 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WrongCellContentType()
        {
            sheet.SetCellContents("A1", new Formula("1"));
        }

        /// <summary>
        /// Trying to use line that throws circular exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]

        public void CircularException()
        {
            sheet.SetCellContents("A1", "A2");           
            sheet.SetCellContents("A2", "A1");
        }
    }
}
