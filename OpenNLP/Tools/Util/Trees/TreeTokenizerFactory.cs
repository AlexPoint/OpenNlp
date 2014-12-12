using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Process;

namespace OpenNLP.Tools.Util.Trees
{
    public class TreeTokenizerFactory: TokenizerFactory<Tree>
    {
        /** Create a TreeTokenizerFactory from a TreeReaderFactory. */
  public TreeTokenizerFactory(TreeReaderFactory trf) {
    this.trf = trf;
  }

  private TreeReaderFactory trf;

  /** Gets a tokenizer from a reader.*/
  public Tokenizer<Tree> getTokenizer(/*final*/ TextReader r) {
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

  public Tokenizer<Tree> getTokenizer(/*final*/ TextReader r, String extraOptions) {
    // Silently ignore extra options
    return getTokenizer(r);
  }

  /** Same as getTokenizer().  */
  public IEnumerator<Tree> getIterator(TextReader r) {
    return null;
  }

  public void setOptions(String options) {
    //Silently ignore
  }
    }
}
