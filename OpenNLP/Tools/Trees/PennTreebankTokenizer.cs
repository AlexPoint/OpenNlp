using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Process;

namespace OpenNLP.Tools.Trees
{
    /// <summary>
    /// Builds a tokenizer for English PennTreebank (release 2) trees.
    /// This is currently internally implemented via a java.io.StreamTokenizer.
    /// 
    /// @author Christopher Manning
    /// @author Roger Levy
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public class PennTreebankTokenizer : TokenizerAdapter
    {
        /// <summary>
        /// A StreamTokenizer for PennTreebank trees.
        /// </summary>
        private class EnglishTreebankStreamTokenizer : StreamTokenizer
        {
            /// <summary>
            /// Create a StreamTokenizer for PennTreebank trees.
            /// This sets up all the character meanings for treebank trees
            /// </summary>
            public EnglishTreebankStreamTokenizer(TextReader r) : base(r)
            {
                // start with new tokenizer syntax -- everything ordinary
                this.ResetSyntax();
                // treat parens as symbols themselves -- done by reset
                // ordinaryChar(')');
                // ordinaryChar('(');

                // treat chars in words as words, like a-zA-Z
                // treat all the typewriter keyboard symbols as parts of words
                // You need to look at an ASCII table to understand this!
                this.WordChars('!', '\''); // non-space non-ctrl symbols before '('
                this.WordChars('*', '/'); // after ')' till before numbers
                this.WordChars('0', '9'); // numbers
                this.WordChars(':', '@'); // symbols between numbers, letters
                this.WordChars('A', 'Z'); // uppercase letters
                this.WordChars('[', '`'); // symbols between ucase and lcase
                this.WordChars('a', 'z'); // lowercase letters
                this.WordChars('{', '~'); // symbols before DEL
                this.WordChars(128, 255); // C.Thompson, added 11/02

                // take the normal white space charaters, including tab, return,
                // space
                this.WhitespaceChars(0, ' ');
            }
        }

        public PennTreebankTokenizer(TextReader r) : base(new EnglishTreebankStreamTokenizer(r))
        {
        }
    }
}