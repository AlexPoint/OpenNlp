using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees.TRegex.Tsurgeon
{
    public class TsurgeonParseException : SystemException
    {
        private static readonly long serialVersionUID = -4417368416943652737L;

        public TsurgeonParseException(string message) : base(message)
        {
        }

        public TsurgeonParseException(string message, Exception cause) : base(message, cause)
        {
        }
    }
}