using SpreadsheetUtilities;
using System.Text.RegularExpressions;
using System.Text;

///<summary>
///Spreadsheet object; Create, store, and edit cells along with their dependencies.
///Valid forms of cells include types int, double, text, or Formula.
///</summary>
namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {
        Dictionary<string, Cell> cells;
        DependencyGraph dependentCells;

        private bool changed;
        public override bool Changed { get => Changed; protected set => changed = false; }
        private Func<string, string> normalize;
        public Func<string, bool> isValid;
        private string version;

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
                    throw new FormulaFormatException("Variable must start with a Letter A -> Z.");
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
                            throw new FormulaFormatException("Cannot follow doubles by letters in variables.");
                }

                //If no int value at end -> throw
                if (!isDouble)
                    throw new FormulaFormatException("Variable must end with an double value.");
                return true;
            };
        }

        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            cells = new Dictionary<string, Cell>(); // <cell name, cell object>
            dependentCells = new DependencyGraph();
            this.isValid = isValid;
            this.normalize = normalize;
            this.version = version;
        }


        public Spreadsheet(string file, Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            cells = new Dictionary<string, Cell>(); // <cell name, cell object>
            dependentCells = new DependencyGraph();
            this.isValid = isValid;
            this.normalize = normalize;
            this.version = version;
            ParseSpreadsheetFile(file);
            this.Save(file);
        }

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
            if (!cells.ContainsKey(name))
                throw new InvalidNameException();
            return dependentCells.GetDependees(name);
        }

        public override IList<string> SetContentsOfCell(string name, string content)
        {
            name = normalize(name);
            if (!isValid(name))
                throw new InvalidNameException();
            content = normalize(content);
            if (!cells.ContainsKey(name))
                cells.Add(name, new Cell(name, content));

            Formula f = new Formula(content, normalize, isValid);
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
                    //Changing content and value in cell
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
                    //Not possible to be null
                    f = new Formula(cells[s].GetContent().ToString());
                    cells[s].SetValue((double)f.Evaluate(lookup));
                    dependentNames.Add(s);
                }
            }
            changed = true;
            return dependentNames;
        }

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

        public override object GetCellValue(string name)
        {
            if (!cells.ContainsKey(name))
                throw new InvalidNameException();
            return cells[name].GetValue();
        }
    }
}
