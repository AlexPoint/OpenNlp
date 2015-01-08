using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Ling;

namespace OpenNLP.Tools.Util.Process
{
    /// <summary>
    /// Constructs a Word from a string. This is the default
    /// TokenFactory for PTBLexer. It discards the positional information.
    /// 
    /// @author Jenny Finkel
    /// 
    /// Code...
    /// </summary>
    public class WordTokenFactory : ILexedTokenFactory<Word>
    {
        
        public Word MakeToken(string str, int begin, int length)
        {
            return new Word(str, begin, begin + length);
        }
    }
}