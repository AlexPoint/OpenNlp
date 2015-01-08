using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Process
{
    /// <summary>
    /// An abstract tokenizer.
    /// Tokenizers extending AbstractTokenizer need only
    /// implement the <code>getNext()</code> method. This implementation does not
    /// allow null tokens, since null is used in the protected nextToken 
    /// field to signify that no more tokens are available.
    /// 
    /// @author Teg Grenager (grenager@stanford.edu)
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public abstract class AbstractTokenizer<T> : ITokenizer<T>
    {
        protected T NextToken; // = null;

        /// <summary>
        /// Internally fetches the next token.
        /// </summary>
        /// <returns>the next token in the token stream, or null if none exists</returns>
        protected abstract T GetNext();

        /// <summary>
        /// Returns the next token from this Tokenizer.
        /// </summary>
        /// <exception cref="Exception">if the token stream has no more tokens</exception>
        public T Next()
        {
            if (NextToken == null)
            {
                NextToken = GetNext();
            }
            T result = NextToken;
            NextToken = default(T);
            if (result == null)
            {
                throw new Exception();
                //throw new NoSuchElementException();
            }
            return result;
        }

        /// <summary>
        /// Returns <code>true</code> if this Tokenizer has more elements
        /// </summary>
        public bool HasNext()
        {
            if (NextToken == null)
            {
                NextToken = GetNext();
            }
            return NextToken != null;
        }

        /// <summary>
        /// This is an optional operation, by default not supported
        /// </summary>
        public void Remove()
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// This is an optional operation, by default supported
        /// </summary>
        /// <exception cref="Exception">if the token stream has no more tokens</exception>
        public T Peek()
        {
            if (NextToken == null)
            {
                NextToken = GetNext();
            }
            if (NextToken == null)
            {
                throw new Exception();
                //throw new NoSuchElementException();
            }
            return NextToken;
        }

        /// <summary>
        /// Returns A list of all tokens remaining in the underlying Reader
        /// </summary>
        public List<T> Tokenize()
        {
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