using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees.TRegex
{
    public class TregexParseException : SystemException
    {
        public TregexParseException(String message) : base(message)
        {
        }
    }
}