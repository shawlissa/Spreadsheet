using SS;
using SpreadsheetUtilities;
using static System.Runtime.InteropServices.JavaScript.JSType;
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

        /// <summary>
        /// Empty Spreadsheet object.
        /// </summary>
        public Spreadsheet()
        {
            cells = new Dictionary<string, Cell>(); // <cell name, cell object>
            dependentCells = new DependencyGraph();
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
            if (!cells.ContainsKey(name.ToUpper()))
            {
                throw new InvalidNameException();
            }
            return cells[name.ToUpper()].GetContent();
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
        public override ISet<string> SetCellContents(string name, double number)
        {
            return SetCellContentsGeneric(name.ToUpper(), number);
        }

        /// <summary>
        /// Uses helper method 'SetCellContentsGeneric' to set the contents to a text.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public override ISet<string> SetCellContents(string name, string text)
        {
            return SetCellContentsGeneric(name.ToUpper(), text);
        }

        /// <summary>
        /// Uses helper method 'SetCellContentsGeneric' to set the contents to a Formula.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="formula"></param>
        /// <returns></returns>
        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            return SetCellContentsGeneric(name.ToUpper(), formula);
        }

        /// <summary>
        /// Helper method for implementing each SetCellContents method.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private ISet<string> SetCellContentsGeneric(string name, object content)
        {
            Formula f = new Formula((string)content.ToString(), (s) => s.ToUpper(), (s) => true);
            HashSet<string> dependentNames = new HashSet<string> { name };

            //Lookup cell value for formula evaluator
            Func<string, double> lookup = cellName =>
            {
                if (!cells.ContainsKey(cellName))
                    throw new ArgumentException("Invalid cell name.");
                return (Double)cells[cellName].GetValue();
            };

            if (!cells.ContainsKey(name))
                cells.Add(name, new Cell(name, content));

            //Evaluates content as formula and any dependees
            if (content is string)
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
                cells[name].SetValue((Double)content);
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

        /// <summary>
        /// Gives an enumerator of all direct dependents from given cell name.
        /// </summary>
        /// <param name="name"></param>
        /// <throw> InvalidNameException </throw> 
        /// <returns></returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if (!cells.ContainsKey(name.ToUpper()))
                throw new InvalidNameException();
            return dependentCells.GetDependees(name.ToUpper());
        }
    }
}
