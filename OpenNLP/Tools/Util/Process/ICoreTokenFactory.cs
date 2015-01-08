using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Process
{
    /// <summary>
    /// To make tokens like CoreMap or CoreLabel. An alternative to LexedTokenFactory
    /// since this one has option to make tokens differently, which would have been
    /// an overhead for LexedTokenFactory
    /// 
    /// @author Sonal Gupta
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public interface ICoreTokenFactory<IN> where IN : ICoreMap
    {
        IN MakeToken();

        //IN makeToken(string[] keys, string[] values);

        IN MakeToken(IN tokenToBeCopied);

    }
}