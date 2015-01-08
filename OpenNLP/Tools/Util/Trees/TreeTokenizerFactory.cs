using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Process;

namespace OpenNLP.Tools.Util.Trees
{
    /// <summary>
    /// Wrapper for TreeReaderFactory.  Any IOException in the readTree() method
    /// of the TreeReader will result in a null tree returned.
    /// 
    /// @author Roger Levy (rog@stanford.edu)
    /// @author javanlp
    /// 
    /// Code...
    /// </summary>
    public class TreeTokenizerFactory : TokenizerFactory<Tree>
    {
        /// <summary>
        /// Create a TreeTokenizerFactory from a TreeReaderFactory.
        /// </summary>
        public TreeTokenizerFactory(TreeReaderFactory trf)
        {
            this.trf = trf;
        }

        private TreeReaderFactory trf;

        /// <summary>
        /// Gets a tokenizer from a reader
        /// </summary>
        public Tokenizer<Tree> GetTokenizer(TextReader r)
        {
            /*return new AbstractTokenizer<Tree>() {
              TreeReader tr = trf.newTreeReader(r);
              @Override
              public Tree getNext() {
                try {
                  return tr.readTree();
                }
                catch(IOException e) {
                  System.err.println("Error in reading tree.");
                  return null;
                }
              }
            };*/
            throw new NotImplementedException();
        }

        public Tokenizer<Tree> GetTokenizer(TextReader r, string extraOptions)
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