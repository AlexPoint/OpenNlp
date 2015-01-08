using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Ling;

namespace OpenNLP.Tools.Trees
{
    /// <summary>
    /// A {@code SimpleTreeFactory} acts as a factory for creating objects of class {@code SimpleTree}.
    /// 
    /// NB: A SimpleTree stores tree geometries but no node labels.  Make sure
    /// this is what you really want.
    /// 
    /// @author Christopher Manning
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public class SimpleTreeFactory : ITreeFactory
    {
        public virtual Tree NewLeaf(string word)
        {
            return new SimpleTree();
        }

        public virtual Tree NewLeaf(ILabel word)
        {
            return new SimpleTree();
        }

        public virtual Tree NewTreeNode(string parent,List<Tree> children)
        {
            return new SimpleTree(null, children);
        }

        public virtual Tree NewTreeNode(ILabel parentLabel,List<Tree> children)
        {
            return new SimpleTree(parentLabel, children);
        }
    }
}