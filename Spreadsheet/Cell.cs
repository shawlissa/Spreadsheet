using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spreadsheet
{
    internal class Cell
    {
        private object content;
        private string name;
        private double value;
        /// <summary>
        /// Constructs new cell object
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        public Cell(string name, object content)
        {
            if (content == null || name == null)
                throw new ArgumentNullException("Content or name is null.)");
            this.name = name;
            this.content = content;
            this.value = 0; //until content is evaluated
        }

        /// <summary>
        /// Returns name of cell.
        /// </summary>
        /// <returns></returns>
        public string GetName() 
        { return this.name; }

        /// <summary>
        /// Returns value.
        /// </summary>
        /// <returns></returns>
        public object GetValue()
        { return this.value; }

        /// <summary>
        /// Returns content.
        /// </summary>
        /// <returns></returns>
        public object GetContent()
        { return this.content; }

        /// <summary>
        /// Edits content and changes value.
        /// </summary>
        /// <param name="newContent"></param>
        public void EditContent(object newContent)
        {
            EvaluateContent(newContent);
        }

        /// <summary>
        /// Determines and assigns type to content.
        /// Type either is int, double, or Formula.
        /// If neither then throws an argument exception for invalid type.
        /// </summary>
        /// <param name="content"></param>
        /// <exception cref="ArgumentException"></exception>
        public void EvaluateContent(object content)
        {
            if (content is int)
            {
                if (int.TryParse((string?)content, out int intContent))
                {
                    this.content = intContent;
                    this.value = intContent;
                }
                else if (content is double)
                {
                    if (double.TryParse((string?)content, out double DBContent))
                    {
                        this.content = DBContent;
                        this.value = DBContent;
                    }
                    else if (content is Formula)
                    {
                        Formula f = new Formula(content.ToString());
                        this.content = f;
                        this.value = (double)f.Evaluate((s) => 1.0);

                    } else
                    {
                        throw new ArgumentException("Content must be of type int, double, or formula");
                    }
                }

            }
        }

    }
}
