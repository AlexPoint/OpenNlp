using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Parser;

namespace OpenNLP.Tools.Util.Trees
{
    public interface GrammaticalStructureFactory
    {
        /**
   * Vend a new {@link GrammaticalStructure} based on the given {@link Tree}.
   *
   * @param t the tree to analyze
   * @return a GrammaticalStructure based on the tree
   */
        GrammaticalStructure NewGrammaticalStructure(Tree t);
    }
}