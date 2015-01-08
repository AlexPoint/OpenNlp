using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Ling;

namespace OpenNLP.Tools.Util.Trees
{
    /// <summary>
    /// A {@code SimpleTreeFactory} acts as a factory for creating objects of class {@code SimpleTree}.
    /// 
    /// NB: A SimpleTree stores tree geometries but no node labels.  Make sure
    /// this is what you really want.
    /// 
    /// @author Christopher Manning
    /// 
    /// Code...
    /// </summary>
    public class SimpleTreeFactory : TreeFactory
    {
        public virtual Tree NewLeaf(string word)
        {
            return new SimpleTree();
        }

        public virtual Tree NewLeaf(Label word)
        {
            return new SimpleTree();
        }

        public virtual Tree NewTreeNode(string parent,List<Tree> children)
        {
            return new SimpleTree(null, children);
        }

        public virtual Tree NewTreeNode(Label parentLabel,List<Tree> children)
        {
            return new SimpleTree(parentLabel, children);
        }
    }
}