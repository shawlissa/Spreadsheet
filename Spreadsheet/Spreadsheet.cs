/// <summary>
/// Author:      Alissa Shaw
/// Partner:     None
/// Start Date:  01/11/2024
/// Course:      CS 3500, University of Utah, School of Computing
/// Copyright:   CS 3500 and Alissa Shaw - This work may not be copied 
///             for use in Academic Coursework.
///             
/// I, Alissa Shaw, certify that I wrote this code from scratch and
/// did not copy it in part or whole from another source. All references 
/// used in the completion of the assignments are cited in my README file.
/// 
/// File Contents:
///             Creation of spreadsheet may include:
///             Empty spreadsheet.
///             Spreadsheet with validator, normalizer, and version params.
///             Spreadsheet with file, validator, normalizer, and version params.
///             
///             Adding a cell requires the name to be validated. Basic validator
///             states a valid cell name begins with a letter and ends with a value.
///             Name and content of cell are normalized so all data is in same formatting.
///             To change a cells content -> ensure no circular dependencies.
///             Changing a cells content will change any of its dependents as well.
///             Changing a cells content will indirectly change its value.
///             Cannot directly change the cells value, but can get the value.
///             
///             May save the cell data in the spreadsheet to an XML file if the data
///             has been changed since last saved.
///             May get the version of a saved spreadsheet per request of file.
///             Can return a string version of whats saved in the XML file.
/// </summary>
using SpreadsheetUtilities;
using System.Text.RegularExpressions;
using System.Text;

namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {
        Dictionary<string, Cell> cells; // Contains every non empty cell in spreadsheet by <Name, Cell>
        DependencyGraph dependentCells; // Graphs dependencies between non empty cells

        private bool changed; // bool field for Changed func
        public override bool Changed { get => Changed; protected set => changed = false; } // Determines if spreadsheet data has changed
        private Func<string, string> normalize; // Sets names and contents to same formatting
        public Func<string, bool> isValid; // Verifies validity of cell name
        private string version; // Version of the spreadsheet

        /// <summary>
        /// Empty Spreadsheet object.
        /// </summary>
        public Spreadsheet() :
            this((s) => true, (s) => s.ToUpper(), "default")
        {
            cells = new Dictionary<string, Cell>(); // <cell name, cell object>
            dependentCells = new DependencyGraph();
            this.normalize = (s) => s.ToUpper();
            version = "default";
            this.isValid = (s) => {
                bool isDouble = false;
                string[] substrings = Regex.Split(s, "");

                //First substring is not a letter -> throw argumentException
                if (Regex.Matches(substrings[1], @"[a-zA-Z]").Count == 0)
                    throw new InvalidNameException();
                foreach (string str in substrings)
                {
                    if (str.Contains(" ") || s == "")
                        continue;
                    //Int value found
                    if (Regex.Matches(str, @"[0-9]").Count > 0)
                    {
                        isDouble = true;
                    }
                    //If variable already has int values but switches back to letters -> throw (ex A1A)
                    if (isDouble)
                        if (Regex.Matches(str, @"[a-zA-Z]").Count > 0)
                            throw new InvalidNameException();
                }

                //If no int value at end -> throw
                if (!isDouble)
                    throw new InvalidNameException();
                return true;
            };
        }

        /// <summary>
        /// Spreadsheet object with user provided isValid, normalize, and version.
        /// isValid verifies the validity of the given cell name.
        /// Normalize forces names, contents, and their values to be in the same format.
        /// Version is the version of the spreadsheet.
        /// </summary>
        /// <param name="isValid"></param>
        /// <param name="normalize"></param>
        /// <param name="version"></param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            cells = new Dictionary<string, Cell>(); // <cell name, cell object>
            dependentCells = new DependencyGraph();
            this.isValid = isValid;
            this.normalize = normalize;
            this.version = version;
        }

        /// <summary>
        /// Spreadsheet object with user provided file, isValid, normalize, and version.
        /// Provided file must be in correct XML format.
        /// File is read into cells, their names and content.
        /// isValid verifies the validity of the given cell name.
        /// Normalize forces names, contents, and their values to be in the same format.
        /// Version is the version of the spreadsheet.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="isValid"></param>
        /// <param name="normalize"></param>
        /// <param name="version"></param>
        public Spreadsheet(string file, Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            cells = new Dictionary<string, Cell>(); // <cell name, cell object>
            dependentCells = new DependencyGraph();
            this.isValid = isValid;
            this.normalize = normalize;
            this.version = version;
            ParseSpreadsheetFile(file);
        }

        /// <summary>
        /// Helper method for Spreadsheet constructor with parameter of a file.
        /// Reads the given XML file, searches and adds any cells to the 'cells' dictionary
        /// along with their name and content.
        /// </summary>
        /// <param name="file"></param>
        /// <exception cref="SpreadsheetReadWriteException"></exception>
        private void ParseSpreadsheetFile(string file)
        {
            bool isContent = false, isName = false, isCell = false;
            string name = "", content = "";
            string line;
            string[] substrings;
            try{
                using (StreamReader sr = new StreamReader(file))
                {
                    if (sr.Peek() == -1)
                        throw new SpreadsheetReadWriteException("Empty file.");
                    while (sr.Peek() != null)
                    {
                        line = sr.ReadLine();
                        if (line == "")
                            continue;
                        substrings = Regex.Split(line, "(\\>)|(\\<)");
                        foreach(string s in substrings)
                        {
                            if (s == "<" || s == ">" || s == "/")
                                continue;
                            //New cell object
                            if (s == "cell")
                            {
                                isCell = true;
                                break;
                            }
                            //Evaluating cell object
                            if (isCell)
                            {
                                if (s == "name")
                                {
                                    isName = true;
                                    continue;
                                }
                                else if (s == "contents")
                                {
                                    isContent = true;
                                    continue;
                                }

                                //Evaluating cell name
                                if (isName)
                                {
                                    name = s;
                                    isName = false;
                                    break;
                                }
                                //Evaluating cell content and creation of cell object
                                else if (isContent)
                                {
                                    content = s;
                                    isContent = false;
                                    break;
                                }
                                if (s == "/cell")
                                {
                                    SetContentsOfCell(name, content);
                                    isCell = false;
                                }
                            }
                            if (s == "/spreadsheet")
                            {
                                sr.Close();
                                return;
                            }
                        }
                    }
                }
            } catch (Exception)
            {
                throw new SpreadsheetReadWriteException("Failed to open/write/close the file.");
            }
        }

        /// <summary>
        /// Trys to find cell in dictionary, returns content if found,
        /// else throws InvalidNameException();
        /// </summary>
        /// <param name="name"></param
        /// <throw> InvalidNameException </throw>
        /// <returns></returns>
        public override object GetCellContents(string name)
        {
            if (!cells.ContainsKey(name))
            {
                throw new InvalidNameException();
            }
            return cells[name].GetContent();
        }

        /// <summary>
        /// Returns all non-empty cell names in an enumerator.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            foreach (string cellName in cells.Keys)
            {
                yield return cellName;
            }
        }

        /// <summary>
        /// Uses helper method 'SetCellContentsGeneric' to set the contents to a double.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        protected override IList<string> SetCellContents(string name, double number)
        {
            return SetContentsOfCell(normalize(name), number.ToString());

        }

        /// <summary>
        /// Uses helper method 'SetCellContentsGeneric' to set the contents to a text.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        protected override IList<string> SetCellContents(string name, string text)
        {
            return SetContentsOfCell(normalize(name), text);
        }

        /// <summary>
        /// Uses helper method 'SetCellContentsGeneric' to set the contents to a Formula.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="formula"></param>
        /// <returns></returns>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            return SetContentsOfCell(normalize(name), formula.ToString());
        }

        /// <summary>
        /// Gives an enumerator of all direct dependents from given cell name.
        /// </summary>
        /// <param name="name"></param>
        /// <throw> InvalidNameException </throw> 
        /// <returns></returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return dependentCells.GetDependees(name);
        }

        public override IList<string> SetContentsOfCell(string name, string content)
        {
            Formula f;
            name = normalize(name);
            isValid(name);
            content = normalize(content);
            if (!cells.ContainsKey(name))
                cells.Add(name, new Cell(name, content));
            List<string> dependentNames = new List<string>();

            //Lookup cell value for formula evaluator
            Func<string, double> lookup = cellName =>
            {
                if (!cells.ContainsKey(cellName))
                    throw new InvalidNameException();
                return (Double)cells[cellName].GetValue();
            };

            if (double.TryParse(content, out double result))
            {
                cells[name].EditContent(result);
                cells[name].SetValue(result);
            }
            //Evaluates content as formula and any dependees
            else if (content is string)
            {
                //Evaluate for formula
                if (content.StartsWith("="))
                {
                    f = new Formula(content, normalize, isValid);

                    cells[name].EditContent(f.ToString());
                    cells[name].SetValue((double)f.Evaluate(lookup)); 

                    //Adding any dependencies in formula
                    foreach (string dependentCell in f.GetVariables())
                    {
                        dependentCells.AddDependency(name, dependentCell);
                    }
                }
                else
                {
                    cells[name].EditContent(content);
                    cells[name].SetValue(content);
                }
            }
            if (dependentCells.HasDependents(name))
            {
                //Editing all dependencies                
                foreach (string s in GetCellsToRecalculate(name))
                {
                    //Not possible for content to be null
                    f = new Formula(cells[s].GetContent().ToString());
                    cells[s].SetValue((double)f.Evaluate(lookup));
                    dependentNames.Add(s);
                }
            }
            changed = true;
            return dependentNames;
        }

        /// <summary>
        /// Parses XML of spreadsheet to find version information.
        /// If file cannot be opened, read, or closed -> throws
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        /// <exception cref="SpreadsheetReadWriteException"></exception>
        public override string GetSavedVersion(string filename)
        {
            string version = "";
            try
            {
                FileStream inFile = new FileStream(filename, FileMode.Open, FileAccess.Read);
                using (StreamReader sr = new StreamReader(inFile))
                {
                    string line = sr.ReadLine();
                    if (line.Contains("version"))
                    {
                        string[] substrings = Regex.Split(line, "(\\s+)|(\\')");
                        for (int i = 0; i < substrings.Length; i++)
                        {
                            if (substrings[i].Equals("'"))
                            {
                                //version='' -> substring after ' is version info
                                version = substrings[i + 1];
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw new SpreadsheetReadWriteException("Failed to open/read/close the file.");
            }
            return version;
        }

        /// <summary>
        /// Saves spreadsheet information in an XML file.
        /// If unable to open, write, or close file -> throws
        /// </summary>
        /// <param name="filename"></param>
        /// <exception cref="SpreadsheetReadWriteException"></exception>
        public override void Save(string filename)
        {
            if (!changed)
                return;
            try
            {
                using (FileStream fs = File.Create(filename))
                {
                    byte[] spreadsheetXML = new UTF8Encoding(true).GetBytes(this.GetXML());
                    fs.Write(spreadsheetXML);
                }
            } catch (Exception)
            {
                throw new SpreadsheetReadWriteException("Failed to open/write/close the file.");
            }
            changed = false;
        }

        /// <summary>
        /// Returns an XML formatted string version of 'this' spreadsheet.
        /// </summary>
        /// <returns></returns>
        public override string GetXML()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<spreadsheet version='" + this.version + "'>\n\n");
            foreach (Cell c in this.cells.Values)
            {
                sb.Append("<cell>\n");

                    sb.Append("<name>");
                        sb.Append(c.GetName() + "");
                    sb.Append("</name>\n");

                    sb.Append("<contents>");
                        sb.Append(c.GetContent() + "");
                    sb.Append("</contents>\n");

                sb.Append("</cell>\n\n");
            }
            sb.Append("</spreadsheet>");
            return sb.ToString();
        }

        /// <summary>
        /// Returns value of cell name requested.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="InvalidNameException"></exception>
        public override object GetCellValue(string name)
        {
            if (!cells.ContainsKey(name))
                throw new InvalidNameException();
            return cells[name].GetValue();
        }
    }
}