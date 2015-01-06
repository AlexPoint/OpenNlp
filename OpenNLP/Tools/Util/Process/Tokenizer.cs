using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Process
{
    public interface Tokenizer<T> : IEnumerator<T>
    {
        /**
   * Returns the next token from this Tokenizer.
   *
   * @return the next token in the token stream.
   * @throws java.util.NoSuchElementException
   *          if the token stream has no more tokens.
   */
        //@Override
        T next();

        /**
   * Returns <code>true</code> if and only if this Tokenizer has more elements.
   */
        //@Override
        bool hasNext();

        /**
   * Removes from the underlying collection the last element returned by
   * the iterator.  This is an optional operation for Iterators - a
   * Tokenizer normally would not support it. This method can be called
   * only once per call to next.
   */
        //@Override
        void remove();

        /**
   * Returns the next token, without removing it, from the Tokenizer, so
   * that the same token will be again returned on the next call to
   * next() or peek().
   *
   * @return the next token in the token stream.
   * @throws java.util.NoSuchElementException
   *          if the token stream has no more tokens.
   */
        T peek();

        /**
   * Returns all tokens of this Tokenizer as a List for convenience.
   *
   * @return A list of all the tokens
   */
        List<T> tokenize();
    }
}