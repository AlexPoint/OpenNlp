using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Ling;

namespace OpenNLP.Tools.Util.Trees
{
    public class SimpleTreeFactory : TreeFactory
    {
        /**
   * Creates a new <code>TreeFactory</code>.  A
   * <code>SimpleTree</code> stores no <code>Label</code>, so no
   * <code>LabelFactory</code> is built.
   */

        public SimpleTreeFactory()
        {
        }

        //@Override
        public virtual Tree NewLeaf( /*final*/ string word)
        {
            return new SimpleTree();
        }

        //@Override
        public virtual Tree NewLeaf( /*final*/ Label word)
        {
            return new SimpleTree();
        }

        //@Override
        public virtual Tree NewTreeNode( /*final*/ string parent, /*final */List<Tree> children)
        {
            return new SimpleTree(null, children);
        }

        //@Override
        public virtual Tree NewTreeNode( /*final */ Label parentLabel, /*final */List<Tree> children)
        {
            return new SimpleTree(parentLabel, children);
        }
    }
}