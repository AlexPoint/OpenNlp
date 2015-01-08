using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees.TRegex
{
    /// <summary>
    /// A runtime exception that indicates something went wrong parsing a
    /// tregex expression.  The purpose is to make those exceptions
    /// unchecked exceptions, as there are only a few circumstances in
    /// which one could recover.
    /// 
    /// @author John Bauer
    /// 
    /// Code...
    /// </summary>
    public class TregexParseException : SystemException
    {
        public TregexParseException(string message) : base(message)
        {
        }
    }
}