using SS;
using SpreadsheetUtilities;
using static System.Runtime.InteropServices.JavaScript.JSType;
///<summary>
///Spreadsheet object; Create, store, and edit cells along with their dependencies.
///Valid forms of cells include types int, double, text, or Formula.
///</summary>
namespace Spreadsheet
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
            if (cells.TryGetValue(name, out Cell c))
            {
                return c.GetContent();
            } else
            {
                return new InvalidNameException();
            }
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
            return SetCellContentsGeneric(name, number);
        }

        /// <summary>
        /// Uses helper method 'SetCellContentsGeneric' to set the contents to a text.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public override ISet<string> SetCellContents(string name, string text)
        {
            return SetCellContentsGeneric(name, text);
        }

        /// <summary>
        /// Uses helper method 'SetCellContentsGeneric' to set the contents to a Formula.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="formula"></param>
        /// <returns></returns>
        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            return SetCellContentsGeneric(name, formula);
        }

        /// <summary>
        /// Helper method for implementing each SetCellContents method.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private ISet<string> SetCellContentsGeneric(string name, object content)
        {
            HashSet<string> dependentNames = new HashSet<string> { name };

            //Editing cell 'name' and all its dependents based off new content.
            cells[name].EditContent(content);
            if (cells[name].GetType() == typeof(Formula))
            {
                foreach (string cell in GetCellsToRecalculate(name))
                { 
                    dependentNames.Add(cell);
                    cells[cell].EvaluateContent(cells[cell].GetContent());
                    //Add back cell with new 
                }
                dependentCells.ReplaceDependees(name, dependentNames);
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
            if (dependentCells.GetDependents(name) == Enumerable.Empty<string>())
                throw new InvalidNameException();
            return dependentCells.GetDependents(name);
        }
    }
}
