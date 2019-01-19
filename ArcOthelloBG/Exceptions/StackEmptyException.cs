using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcOthelloBG.Exceptions
{
    /// <summary>
    /// exception in case a stack of a undo/redo is empty
    /// </summary>
    class StackEmptyException : Exception
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="message">error message</param>
        public StackEmptyException(string message) : base(message)
        {
        // nothing
        }
    }
}
