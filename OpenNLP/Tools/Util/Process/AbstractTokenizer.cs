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
        protected abstract T GetNext();

        /**
   * Returns the next token from this Tokenizer.
   *
   * @return the next token in the token stream.
   * @throws java.util.NoSuchElementException
   *          if the token stream has no more tokens.
   */
        //@Override
        public T Next()
        {
            if (nextToken == null)
            {
                nextToken = GetNext();
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
        public bool HasNext()
        {
            if (nextToken == null)
            {
                nextToken = GetNext();
            }
            return nextToken != null;
        }

        /**
   * This is an optional operation, by default not supported.
   */
        //@Override
        public void Remove()
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
        public T Peek()
        {
            if (nextToken == null)
            {
                nextToken = GetNext();
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
        public List<T> Tokenize()
        {
            // System.out.println("tokenize called");
            var result = new List<T>();
            while (HasNext())
            {
                result.Add(Next());
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