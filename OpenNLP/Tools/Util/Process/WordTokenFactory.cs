using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Ling;

namespace OpenNLP.Tools.Util.Process
{
    /**
 * Constructs a Word from a string. This is the default
 * TokenFactory for PTBLexer. It discards the positional information.
 *
 * @author Jenny Finkel
 */

    public class WordTokenFactory : LexedTokenFactory<Word>
    {
        //@Override
        public Word makeToken(string str, int begin, int length)
        {
            return new Word(str, begin, begin + length);
        }
    }
}