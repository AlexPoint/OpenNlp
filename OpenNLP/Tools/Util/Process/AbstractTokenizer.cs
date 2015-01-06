using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Process
{
    public abstract class AbstractTokenizer<T> : Tokenizer<T>
    {
        protected T nextToken; // = null;

        /**
   * Internally fetches the next token.
   *
   * @return the next token in the token stream, or null if none exists.
   */
        protected abstract T getNext();

        /**
   * Returns the next token from this Tokenizer.
   *
   * @return the next token in the token stream.
   * @throws java.util.NoSuchElementException
   *          if the token stream has no more tokens.
   */
        //@Override
        public T next()
        {
            if (nextToken == null)
            {
                nextToken = getNext();
            }
            T result = nextToken;
            nextToken = default(T);
            if (result == null)
            {
                throw new Exception();
                //throw new NoSuchElementException();
            }
            return result;
        }

        /**
   * Returns <code>true</code> if this Tokenizer has more elements.
   */
        //@Override
        public bool hasNext()
        {
            if (nextToken == null)
            {
                nextToken = getNext();
            }
            return nextToken != null;
        }

        /**
   * This is an optional operation, by default not supported.
   */
        //@Override
        public void remove()
        {
            throw new InvalidOperationException();
        }

        /**
   * This is an optional operation, by default supported.
   *
   * @return The next token in the token stream.
   * @throws java.util.NoSuchElementException
   *          if the token stream has no more tokens.
   */
        //@Override
        public T peek()
        {
            if (nextToken == null)
            {
                nextToken = getNext();
            }
            if (nextToken == null)
            {
                throw new Exception();
                //throw new NoSuchElementException();
            }
            return nextToken;
        }

        /**
   * Returns text as a List of tokens.
   *
   * @return A list of all tokens remaining in the underlying Reader
   */
        //@Override
        public List<T> tokenize()
        {
            // System.out.println("tokenize called");
            List<T> result = new List<T>();
            while (hasNext())
            {
                result.Add(next());
            }
            return result;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public T Current { get; private set; }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}