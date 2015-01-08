using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Process
{
    /// <summary>
    /// Tokenizers break up text into individual Objects. These objects may be
    /// Strings, Words, or other Objects.  A Tokenizer extends the Iterator
    /// interface, but provides a lookahead operation <code>peek()</code>.  An
    /// implementation of this interface is expected to have a constructor that
    /// takes a single argument, a Reader.
    /// 
    /// @author Teg Grenager (grenager@stanford.edu)
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public interface ITokenizer<T> : IEnumerator<T>
    {
        
        /// <summary>
        /// Returns the next token from this Tokenizer.
        /// </summary>
        T Next();

        /// <summary>
        /// Returns <code>true</code> if and only if this Tokenizer has more elements.
        /// </summary>
        bool HasNext();

        /// <summary>
        /// Removes from the underlying collection the last element returned by
        /// the iterator.  This is an optional operation for Iterators - a
        /// Tokenizer normally would not support it. This method can be called
        /// only once per call to next.
        /// </summary>
        void Remove();

        /// <summary>
        /// Returns the next token, without removing it, from the Tokenizer, so
        /// that the same token will be again returned on the next call to next() or peek().
        /// </summary>
        /// <returns>the next token in the token stream.</returns>
        T Peek();

        /// <summary>
        /// Returns all tokens of this Tokenizer as a List for convenience
        /// </summary>
        List<T> Tokenize();
    }
}