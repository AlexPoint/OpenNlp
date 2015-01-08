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
    /// Wrapper for TreeReaderFactory.  Any IOException in the readTree() method
    /// of the TreeReader will result in a null tree returned.
    /// 
    /// @author Roger Levy (rog@stanford.edu)
    /// @author javanlp
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public class TreeTokenizerFactory : ITokenizerFactory<Tree>
    {
        /// <summary>
        /// Create a TreeTokenizerFactory from a TreeReaderFactory.
        /// </summary>
        public TreeTokenizerFactory(ITreeReaderFactory trf)
        {
            this.trf = trf;
        }

        private ITreeReaderFactory trf;

        /// <summary>
        /// Gets a tokenizer from a reader
        /// </summary>
        public ITokenizer<Tree> GetTokenizer(TextReader r)
        {
            /*return new AbstractTokenizer<Tree>() {
              TreeReader tr = trf.newTreeReader(r);
              @Override
              public Tree getNext() {
                try {
                  return tr.readTree();
                }
                catch(IOException e) {
                  return null;
                }
              }
            };*/
            throw new NotImplementedException();
        }

        public ITokenizer<Tree> GetTokenizer(TextReader r, string extraOptions)
        {
            // Silently ignore extra options
            return GetTokenizer(r);
        }

        /// <summary>
        /// Same as getTokenizer()
        /// </summary>
        public IEnumerator<Tree> GetIterator(TextReader r)
        {
            return null;
        }

        public void SetOptions(string options)
        {
            //Silently ignore
        }
    }
}