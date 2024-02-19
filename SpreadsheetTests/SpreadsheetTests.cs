using SS;

namespace DevelopmentTests;

///<summary>
///Tester class for the 'Spreadsheet' project.
///</summary>

[TestClass]
public class SpreadsheetTests
{
    AbstractSpreadsheet basicSheet;
    AbstractSpreadsheet sheetWithParams;
    AbstractSpreadsheet PreSavedSheet;

    /// <summary>
    /// Initializes a generic small spreadsheet, 'sheet', to use among tests.
    /// </summary>
    [TestInitialize]
    public void InitializeTest()
    {
        basicSheet = new Spreadsheet();
        basicSheet.SetContentsOfCell("A1", "5"); // 5
        basicSheet.SetContentsOfCell("A2", "2"); // 2
        basicSheet.SetContentsOfCell("A3", "=A1 + A2"); // 7
        basicSheet.SetContentsOfCell("A4", "=5 / A2"); // 2.5
        basicSheet.SetContentsOfCell("A5", "=A3 * A1"); // 35
        basicSheet.SetContentsOfCell("B1", "=3 * 2"); // 6
        basicSheet.SetContentsOfCell("B2", "=B1 + A1"); // 11
        basicSheet.SetContentsOfCell("B3", "=A1 + A2 / A4 - ( B1 * B2 )"); // -60.2
        basicSheet.SetContentsOfCell("B4", "10"); // 10
        basicSheet.SetContentsOfCell("B5", "=5e2"); // 500

        //All variables are valid and converts to lowercase
        sheetWithParams = new Spreadsheet((s) => true, (s) => s.ToLower(), "V1.0");
        sheetWithParams.SetContentsOfCell("A1", "5"); // 5
        sheetWithParams.SetContentsOfCell("A2", "2"); // 2
        sheetWithParams.SetContentsOfCell("A3", "=A1 + A2"); // 7
        sheetWithParams.SetContentsOfCell("A4", "=5 / A2"); // 2.5
        sheetWithParams.SetContentsOfCell("A5", "=A3 * A1"); // 35
        sheetWithParams.SetContentsOfCell("B1", "=3 * 2"); // 6
        sheetWithParams.SetContentsOfCell("B2", "=B1 + A1"); // 11
        sheetWithParams.SetContentsOfCell("B3", "=A1 + A2 / A4 - ( B1 * B2 )"); // -60.2
        sheetWithParams.SetContentsOfCell("B4", "10"); // 10
        sheetWithParams.SetContentsOfCell("B5", "=5e2"); // 500
       
        PreSavedSheet = new Spreadsheet("C://Users/Owner/source/repos/CS3500/Spreadsheet/Spreadsheet/bin/savedFile.XML", (s) => true, (s) => s.ToUpper(), "V2.0");
    }
    //GetCellContents Tests: ----------------------------------
    /// <summary>
    /// 'Name' is not of type Cell
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetCellContentsWrongTypeBasic()
    {
        basicSheet.GetCellContents("AA");
    }

    /// <summary>
    /// 'Name' is not of type Cell
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetCellContentsWrongType()
    {
        sheetWithParams.GetCellContents("A1");
    }

    /// <summary>
    /// Name is a nonempty cell
    /// </summary>
    [TestMethod]
    public void GetCellContentsValidNonEmptyNameBasic()
    {
        basicSheet.SetContentsOfCell("a1", "5.0");
        Assert.AreEqual(5.0, basicSheet.GetCellContents("A1"));
    }

    /// <summary>
    /// Name is a nonempty cell
    /// </summary>
    [TestMethod]
    public void GetCellContentsValidNonEmptyNameParams()
    {
        sheetWithParams.SetContentsOfCell("A1", "5.0");
        Assert.AreEqual(5.0, sheetWithParams.GetCellContents("a1"));
    }

    /// <summary>
    /// Get contents of formula
    /// </summary>
    [TestMethod]
    public void GetCellContentsValidFormulaContentBasic()
    {
        Assert.AreEqual("=A1+A2", basicSheet.GetCellContents("A3"));
    }


    /// <summary>
    /// Get contents of formula
    /// </summary>
    [TestMethod]
    public void GetCellContentsValidFormulaContentParams()
    {
        Assert.AreEqual("=a1+a2", sheetWithParams.GetCellContents("a3"));
    }

    /// <summary>
    /// Get contents of formula
    /// </summary>
    [TestMethod]
    public void GetCellContentsValidComplexFormulaContentBasic()
    {
        Assert.AreEqual("=A1+A2/A4-(B1*B2)", basicSheet.GetCellContents("B3"));
    }

    /// <summary>
    /// Get contents of formula
    /// </summary>
    [TestMethod]
    public void GetCellContentsValidComplexFormulaContentParams()
    {
        Assert.AreEqual("=a1+a2/a4-(b1*b2)", sheetWithParams.GetCellContents("b3"));
    }

    /// <summary>
    /// Gets same cell content repeatedly
    /// </summary>
    [TestMethod]
    public void GetCellContentsStressTestBasic()
    {
        for (int i = 0; i < 100; i++)
        {
            Assert.AreEqual("=A1+A2/A4-(B1*B2)", basicSheet.GetCellContents("B3"));
        }
    }

    /// <summary>
    /// Gets same cell content repeatedly
    /// </summary>
    [TestMethod]
    public void GetCellContentsStressTestParams()
    {
        for (int i = 0; i < 100; i++)
        {
            Assert.AreEqual("=a1+a2/a4-(b1*b2)", sheetWithParams.GetCellContents("b3"));
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
            Assert.AreEqual("=A1+A2/A4-(B1*B2)", basicSheet.GetCellContents("B3"));
        }
        for (int i = 0; i < 100; i++)
        {
            Assert.AreEqual("=B1+A1", basicSheet.GetCellContents("B2"));
        }
        for (int i = 0; i < 100; i++)
        {
            Assert.AreEqual("=3*2", basicSheet.GetCellContents("B1"));
        }
        for (int i = 0; i < 100; i++)
        {
            Assert.AreEqual(5.0, basicSheet.GetCellContents("A1"));
        }
        for (int i = 0; i < 100; i++)
        {
            Assert.AreEqual(2.0, basicSheet.GetCellContents("A2"));
        }
    }
    //SetContentsOfCell Tests: ----------------------------------

    /// <summary>
    /// InvalidNameException thrown as cell name begins with a number and ends with a letter.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void InvalidCellNameOne()
    {
        basicSheet.SetContentsOfCell("1A", "5");
    }

    /// <summary>
    /// InvalidNameException thrown as cell name ends with a letter.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void InvalidCellNameTwo()
    {
        basicSheet.SetContentsOfCell("A1A", "5");
    }

    /// <summary>
    /// InvalidNameException thrown as cell name does not end with a value.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void InvalidCellNameThree()
    {
        basicSheet.SetContentsOfCell("A", "5");
    }

    /// <summary>
    /// Throws NullReferenceException when entering null content/name for a cell.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(NullReferenceException))]
    public void SetCellContentsNull()
    {
        basicSheet.SetContentsOfCell(null, null);
    }

    /// <summary>
    /// Sets cell content with no dependencies
    /// </summary>
    [TestMethod]
    public void SetCellContentsNoDependencyBasic()
    {
        //Only one cell name should be in set
        List<string> cells = (List<string>)basicSheet.SetContentsOfCell("B4", "1");
        foreach (string s in cells)
        {
            Assert.AreEqual("B4", s);
        }
    }

    /// <summary>
    /// Sets cell content with no dependencies
    /// </summary>
    [TestMethod]
    public void SetCellContentsNoDependencyParams()
    {
        //Only one cell name should be in set
        List<string> cells = (List<string>)sheetWithParams.SetContentsOfCell("B4", "1");
        foreach (string s in cells)
        {
            Assert.AreEqual("b4", s);
        }
    }

    /// <summary>
    /// Sets cell content with one dependencies
    /// </summary>
    [TestMethod]
    public void SetCellContentsOneDependencyBasic()
    {
        List<string> cells = (List<string>)basicSheet.SetContentsOfCell("A3", "1");
        IEnumerator<string> expected = cells.GetEnumerator();
        expected.MoveNext();
        Assert.AreEqual("A3", expected.Current);
        expected.MoveNext();
        Assert.AreEqual("A5", expected.Current);
    }

    /// <summary>
    /// Sets cell content with one dependencies
    /// </summary>
    [TestMethod]
    public void SetCellContentsOneDependencyParams()
    {
        List<string> cells = (List<string>)sheetWithParams.SetContentsOfCell("A3", "1");
        IEnumerator<string> expected = cells.GetEnumerator();
        expected.MoveNext();
        Assert.AreEqual("a3", expected.Current);
        expected.MoveNext();
        Assert.AreEqual("a5", expected.Current);
    }

    /// <summary>
    /// Sets cell content with two dependencies
    /// </summary>
    [TestMethod]
    public void SetCellContentsTwoDependencies()
    {
        basicSheet.SetContentsOfCell("B4", "=A3 + 3");
        //Only one cell name should be in set
        List<string> cells = (List<string>)basicSheet.SetContentsOfCell("A3", "1");
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
        emptySheet.SetContentsOfCell("C4", "hello");
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
        emptySheet.SetContentsOfCell("C4", "3.0");
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
        emptySheet.SetContentsOfCell("C4", "3.0");
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
        for (int i = 0; i < 5; i++) // Letters
        {
            for (int j = 1; j <= 5; j++) // Numbers
            {
                cellName = "" + (char)('A' + i) + j;
                if (j % 2 == 1) // Odd number cell
                {
                    emptySheet.SetContentsOfCell(cellName, j + "");
                }
                else // Even number cell
                {
                    // Depend on the previous cell
                    string dependency = "" + (char)('A' + i) + (j - 1);
                    emptySheet.SetContentsOfCell(cellName, dependency);
                }
            }
        }
        //Verify dependencies
        for (int i = 0; i < 5; i++) // Letters
        {
            for (int j = 1; j <= 5; j++) // Numbers
            {
                cellName = "" + (char)('A' + i) + j;
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
    public void GetNamesOfAllNonemptyCellsCellExistsBasic()
    {
        List<string> names = new List<string> { "A1", "A2", "A3", "A4", "A5", "B1", "B2", "B3", "B4", "B5" };
        IEnumerator<string> actual = names.GetEnumerator();
        foreach (string expected in basicSheet.GetNamesOfAllNonemptyCells())
        {
            actual.MoveNext();
            Assert.AreEqual(actual.Current, expected);
        }
    }

    /// <summary>
    /// Returns names of all nonempty cells in 'sheet' Spreadsheet
    /// </summary>
    [TestMethod]
    public void GetNamesOfAllNonemptyCellsCellExistsParams()
    {
        List<string> names = new List<string> { "a1", "a2", "a3", "a4", "a5", "b1", "b2", "b3", "b4", "b5" };
        IEnumerator<string> actual = names.GetEnumerator();
        foreach (string expected in sheetWithParams.GetNamesOfAllNonemptyCells())
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
        foreach (string expected in basicSheet.GetNamesOfAllNonemptyCells())
        {
            actual.MoveNext();
            Assert.AreEqual(actual.Current, expected);
        }

        //Adding more cells
        basicSheet.SetContentsOfCell("B6", "4");
        basicSheet.SetContentsOfCell("A6", "12");
        names.Add("B6");
        names.Add("A6");
        actual = names.GetEnumerator();

        foreach (string expected in basicSheet.GetNamesOfAllNonemptyCells())
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
            emptySheet.SetContentsOfCell("A" + i, i + "");
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
            emptySheet.SetContentsOfCell("B" + i, i + "");
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
            emptySheet.SetContentsOfCell("C" + i, i + "");
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
            emptySheet.SetContentsOfCell("D" + i, i + "");
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
            emptySheet.SetContentsOfCell("E" + i, i + "");
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
            emptySheet.SetContentsOfCell("F" + i, i + "");
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
            emptySheet.SetContentsOfCell("G" + i, i + "");
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
            emptySheet.SetContentsOfCell("H" + i, i + "");
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

    //GetXML Testing: -----------------------------

    /// <summary>
    /// Getting XML of a spreadsheet with no nonempty cells.
    /// </summary>
    [TestMethod]
    public void GetXMLEmpty()
    {
        AbstractSpreadsheet emptySheet = new Spreadsheet();
        Assert.AreEqual("<spreadsheet version='default'>\n\n</spreadsheet>", emptySheet.GetXML());
    }

    [TestMethod]
    public void GetXMLSimple()
    {
        AbstractSpreadsheet simpleSheet = new Spreadsheet();
        simpleSheet.SetContentsOfCell("A1", "1");
        simpleSheet.GetXML();
    }
    /// <summary>
    /// Getting XML of the basic spreadsheet
    /// </summary>
    [TestMethod]
    public void GetXMLBasic()
    {
        string expected = "<spreadsheet version='default'>\n\n<cell>\n<name>A1</name>\n<contents>5</contents>\n</cell>" +
            "\n\n<cell>\n<name>A2</name>\n<contents>2</contents>\n</cell>" +
            "\n\n<cell>\n<name>A3</name>\n<contents>=A1+A2</contents>\n</cell>" +
            "\n\n<cell>\n<name>A4</name>\n<contents>=5/A2</contents>\n</cell>" +
            "\n\n<cell>\n<name>A5</name>\n<contents>=A3*A1</contents>\n</cell>" +
            "\n\n<cell>\n<name>B1</name>\n<contents>=3*2</contents>\n</cell>" +
            "\n\n<cell>\n<name>B2</name>\n<contents>=B1+A1</contents>\n</cell>" +
            "\n\n<cell>\n<name>B3</name>\n<contents>=A1+A2/A4-(B1*B2)</contents>" +
            "\n</cell>\n\n<cell>\n<name>B4</name>\n<contents>10</contents>" +
            "\n</cell>\n\n<cell>\n<name>B5</name>\n<contents>=5E2</contents>" +
            "\n</cell>\n\n</spreadsheet>";
        string actual = basicSheet.GetXML();
        Console.WriteLine(basicSheet.GetXML());
        Assert.AreEqual(expected, actual);
    }
    //Save Testing: --------------------------
    /// <summary>
    /// Will do nothing; Changed is false, reaches the return statement
    /// </summary>
    [TestMethod]
    public void SaveChangedFalse()
    {
        basicSheet.Save("file");
        basicSheet.Save("file");

    }

    /// <summary>
    /// Edits content then saves basicSheet to a file.
    /// </summary>
    [TestMethod]
    public void SaveChangedTrueGetVersionBasic()
    {
        basicSheet.SetContentsOfCell("A2", "10");
        basicSheet.Save("C://Users/Owner/source/repos/CS3500/Spreadsheet/Spreadsheet/bin/filename.XML");
        Assert.AreEqual("default", basicSheet.GetSavedVersion("C://Users/Owner/source/repos/CS3500/Spreadsheet/Spreadsheet/bin/filename.XML"));
    }

    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void SaveBadPathFile()
    {
        basicSheet.Save("C://This/Path/Does/Not/Exist?");
    }
    //GetSavedVersion Testing: --------------------------

    /// <summary>
    /// Gets version name of file in 'basicSheet'.
    /// </summary>
    [TestMethod]
    public void GetSavedVersionBasic()
    {
        basicSheet.Save("C://Users/Owner/source/repos/CS3500/Spreadsheet/Spreadsheet/bin/filename.XML");
        Assert.AreEqual("default", basicSheet.GetSavedVersion("C://Users/Owner/source/repos/CS3500/Spreadsheet/Spreadsheet/bin/filename.XML"));
    }

    /// <summary>
    /// Gets version name of file in 'sheetWithParams'.
    /// </summary>
    [TestMethod]
    public void GetSavedVersionParams()
    {
        sheetWithParams.SetContentsOfCell("A2", "10");
        sheetWithParams.Save("C://Users/Owner/source/repos/CS3500/Spreadsheet/Spreadsheet/bin/filename.XML");
        Assert.AreEqual("V1.0", sheetWithParams.GetSavedVersion("C://Users/Owner/source/repos/CS3500/Spreadsheet/Spreadsheet/bin/filename.XML"));
    }

    /// <summary>
    /// Gets version name of savedFile in 'PreSavedSheet'.
    /// </summary>
    [TestMethod]
    public void GetSavedVersionPreSaved()
    {
        Assert.AreEqual("V2.0", PreSavedSheet.GetSavedVersion("C:/Users/Owner/source/repos/CS3500/Spreadsheet/Spreadsheet/bin/savedFile.XML"));
    }

    /// <summary>
    /// File doesnt exist.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void GetSavedVersionDoesntExist()
    {
        basicSheet.GetSavedVersion("InvalidFileName");
    }

    //GetCellValue Testing: ----------------------------

    /// <summary>
    /// Getting value of cell with basic double content.
    /// </summary>
    [TestMethod]
    public void GetCellValueBasicContent()
    {
        Assert.AreEqual(5.0, basicSheet.GetCellValue("A1"));
    }

    /// <summary>
    /// Getting value of cell with basic double content.
    /// </summary>
    [TestMethod]
    public void GetCellValueComplexContent()
    {
        Assert.AreEqual(-60.2, basicSheet.GetCellValue("B3"));
    }

    /// <summary>
    /// Getting value of cell that doesnt exist.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetCellValueCellDoesntExist()
    {
        basicSheet.GetCellValue("C3");
    }
    //Extra Testing: ----------------------------

    /// <summary>
    /// Trying to use line that throws circular exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(CircularException))]

    public void CircularException()
    {
        basicSheet.SetContentsOfCell("A1", "=A2");           
        basicSheet.SetContentsOfCell("A2", "=A1");
    }

    /// <summary>
    /// Trying to set contents of cell to create a non-direct CircularException
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(CircularException))]

    public void ComplexCircularException()
    {
        basicSheet.SetContentsOfCell("A2", "=A1");
        basicSheet.SetContentsOfCell("A3", "=A2");
        basicSheet.SetContentsOfCell("A4", "=A3");
        basicSheet.SetContentsOfCell("A5", "=A4");
        basicSheet.SetContentsOfCell("A1", "=A5");
    }

    /// <summary>
    /// Empty files throws SpreadsheetReadWriteException
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void PassEmptyFileInSpreadsheet()
    {
        AbstractSpreadsheet simpleSheet = new Spreadsheet("C://Users/Owner/source/repos/CS3500/Spreadsheet/Spreadsheet/bin/emptyFile.XML", (s) => true, (s) => s.ToLower(), "V1.0");
    }

    /// <summary>
    /// Cell name being referenced in formula does not exist in 'Cells'.
    /// </summary>
    [TestMethod]
    [ExpectedException (typeof(InvalidNameException))]
    public void ReferenceCellNameDoesntExist()
    {
        basicSheet.SetContentsOfCell("A1", "=D5");
    }

    /// <summary>
    /// Setting the contents of a cell as the given string.
    /// </summary>
    [TestMethod]
    public void SetContentsOfCellString()
    {
        basicSheet.SetContentsOfCell("A1", "Testing string.");
        Assert.AreEqual("TESTING STRING.", basicSheet.GetCellContents("A1"));
        Assert.AreEqual("TESTING STRING.", basicSheet.GetCellValue("A1"));
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetCellValueInvalidName()
    {
        basicSheet.GetCellValue("F10");
    }

    /// <summary>
    /// Same as whats provided in Initialize Test.
    /// Proves correct XML documentation.
    /// </summary>
    [TestMethod]
    public void ParseFileIntoSpreadsheet()
    {
        string actual = "<spreadsheet version='V2.0'>\n\n" +
            "<cell>\n<name>A1</name>\n<contents>5</contents>\n</cell>" +
            "\n\n<cell>\n<name>A2</name>\n<contents>10</contents>\n</cell>" +
            "\n\n<cell>\n<name>A3</name>\n<contents>=A1+A2</contents>\n</cell>" +
            "\n\n<cell>\n<name>A4</name>\n<contents>=5/A2</contents>\n</cell>" +
            "\n\n<cell>\n<name>A5</name>\n<contents>=A3*A1</contents>\n</cell>" +
            "\n\n<cell>\n<name>B1</name>\n<contents>=3*2</contents>\n</cell>" +
            "\n\n<cell>\n<name>B2</name>\n<contents>=B1+A1</contents>\n</cell>" +
            "\n\n<cell>\n<name>B3</name>\n<contents>=A1+A2/A4-(B1*B2)</contents>\n</cell>" +
            "\n\n<cell>\n<name>B4</name>\n<contents>10</contents>\n</cell>" +
            "\n\n<cell>\n<name>B5</name>\n<contents>=5E2</contents>\n</cell>" +
            "\n\n</spreadsheet>";
        PreSavedSheet = new Spreadsheet("C://Users/Owner/source/repos/CS3500/Spreadsheet/Spreadsheet/bin/savedFile.XML", (s) => true, (s) => s.ToUpper(), "V2.0");
        Assert.AreEqual(actual, PreSavedSheet.GetXML());
    }
}
