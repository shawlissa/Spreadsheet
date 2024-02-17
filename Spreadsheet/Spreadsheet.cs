using SpreadsheetUtilities;
using System.Text.RegularExpressions;
using System.Text;

using SS;
using SpreadsheetUtilities;
using static System.Runtime.InteropServices.JavaScript.JSType;
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

        public override bool Changed { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
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

            this.Changed = false;
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
            this.Changed = false;
            this.isValid = isValid;
            this.normalize = normalize;
            this.version = version;
        }


        public Spreadsheet(string file, Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            cells = new Dictionary<string, Cell>(); // <cell name, cell object>
            dependentCells = new DependencyGraph();
            this.Changed = false;
            this.isValid = isValid;
            this.normalize = normalize;
            this.version = version; //ADD THE EVALUATING FILE SHIT
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
            return SetContentsOfCell(name, number.ToString());

        }

        /// <summary>
        /// Uses helper method 'SetCellContentsGeneric' to set the contents to a text.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        protected override IList<string> SetCellContents(string name, string text)
        {
            return SetContentsOfCell(name, text);
        }

        /// <summary>
        /// Uses helper method 'SetCellContentsGeneric' to set the contents to a Formula.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="formula"></param>
        /// <returns></returns>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            return SetContentsOfCell(name, formula.ToString());
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
            Formula f = new Formula(content, normalize, isValid);
            List<string> dependentNames = new List<string> { name };

            //Lookup cell value for formula evaluator
            Func<string, double> lookup = cellName =>
            {
                if (!cells.ContainsKey(cellName))
                    throw new ArgumentException("Invalid cell name.");
                return (Double)cells[cellName].GetValue();
            };


            if (!cells.ContainsKey(name))
                cells.Add(name, new Cell(name, content));

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
            return dependentNames;
        }

        public override string GetSavedVersion(string filename)
        {
            string version = "";
            FileStream inFile = new FileStream(filename, FileMode.Open, FileAccess.Read);
            using (StreamReader sr = new StreamReader(inFile))
            {
                string line = sr.ReadLine();
                if (line.Contains("version"))
                {
                    string[] substrings = line.Split('=');
                    for (int i = 0; i < substrings.Length; i++)
                    {
                        if (substrings[i].Equals("="))
                        {
                            version = substrings[i + 1];
                            break;
                        }
                    }
                }
            }
            return version; //ADD ALL THE FUCKING THROWS N SHIT
        }

        public override void Save(string filename)
        {
            if (!Changed)
                return;
            using (FileStream fs = File.Create(filename))
            {
                byte[] spreadsheetXML = new UTF8Encoding(true).GetBytes(this.GetXML());
                fs.Write(spreadsheetXML);
            }
        }

        public override string GetXML()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<spreadsheet version='" + this.version + "'>\n");
            foreach (Cell c in this.cells.Values)
            {
                sb.Append("\t<cell>\n");

                sb.Append("\t\t<name>\n");
                sb.Append("\t\t\t" + c.GetName() + "\n");
                sb.Append("\t\t</name>\n");

                sb.Append("\t\t<contents>\n");
                sb.Append("\t\t\t" + c.GetContent() + "\n");
                sb.Append("\t\t</contents>\n");

                sb.Append("\t</cell>\n");

            }
            sb.Append("</spreadsheet>\n");
            return sb.ToString();
        }

        public override object GetCellValue(string name)
        {
            return cells[name].GetValue();
        }
    }
}
