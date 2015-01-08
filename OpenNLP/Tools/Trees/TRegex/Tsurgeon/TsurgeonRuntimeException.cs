using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees.TRegex.Tsurgeon
{
    /// <summary>
    /// Something has gone wrong internally in Tsurgeon
    /// 
    /// @author John Bauer
    /// </summary>
    public class TsurgeonRuntimeException : SystemException
    {
        /// <summary>
        /// Creates a new exception with a message.
        /// </summary>
        /// <param name="message">the message for the exception</param>
        public TsurgeonRuntimeException(string message) : base(message)
        {
        }
    }
}