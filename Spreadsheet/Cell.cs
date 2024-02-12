﻿using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS

    ///<summary>
    ///Cell object; contains cell name; contents; and evaluation of contents, value.
    ///Has getter and helper methods to edit and get cell information.
    ///</summary>
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

            //Name must begin with letter and end with number to be valid name/variable
            Formula.evaluateVariable(name);
            this.name = name;
            this.content = content; //until content evaluated
            this.value = 0; //until content is evaluated
            EvaluateContent(content);
        }

        /// <summary>
        /// Returns value.
        /// </summary>
        /// <returns></returns>
        public object GetValue()
        { return this.value; }

        public void SetValue(double value)
        {
            this.value = value;
        }
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
            if (content is int || content is double || content is string)
                this.content = content;
            else
                throw new ArgumentException("Invalid type of content.");
        }
    }
}
