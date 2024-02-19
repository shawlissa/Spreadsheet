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
///            Creating a cell requires a string name and any object as content.
///                 Cell then sets its name, content, and value fields.
///                 
///            Besides constructor, only helper and setter methods;
///                 GetName, GetValue, GetContent
///                 SetValue, EditContent
/// </summary>
using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SS

{
    internal class Cell
    {
        private object content, value;
        private string name;

        /// <summary>
        /// Constructs new cell object
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        public Cell(string name, object content)
        {
            if (content == null || name == null)
                throw new NullReferenceException("Content or name is null.)");

            this.name = name;
            this.content = content; //until content evaluated
            this.value = 0; //until content is evaluated
        }

        /// <summary>
        /// Returns name of cell
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

        public void SetValue(object value)
        { this.value = value; }
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
        { this.content = newContent; }
    }
}
