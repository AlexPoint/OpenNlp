using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Ling;

namespace OpenNLP.Tools.Util.Trees
{
    public class LabeledScoredTreeFactory : SimpleTreeFactory
    {
        private LabelFactory lf;

        /**
   * Make a TreeFactory that produces LabeledScoredTree trees.
   * The labels are of class <code>CoreLabel</code>.
   */

        public LabeledScoredTreeFactory() : this(CoreLabel.Factory())
        {
        }

        /**
   * Make a TreeFactory that uses LabeledScoredTree trees, where the
   * labels are as specified by the user.
   *
   * @param lf the <code>LabelFactory</code> to be used to create labels
   */

        public LabeledScoredTreeFactory(LabelFactory lf)
        {
            this.lf = lf;
        }

        //@Override
        public override Tree newLeaf( /*final*/ string word)
        {
            return new LabeledScoredTreeNode(lf.NewLabel(word));
        }

        /**
   * Create a new leaf node with the given label
   *
   * @param label the label for the leaf node
   * @return A new tree leaf
   */
        //@Override
        public override Tree newLeaf(Label label)
        {
            return new LabeledScoredTreeNode(lf.NewLabel(label));
        }

        //@Override
        public override Tree newTreeNode( /*final*/ string parent, /*final */List<Tree> children)
        {
            return new LabeledScoredTreeNode(lf.NewLabel(parent), children);
        }

        /**
   * Create a new non-leaf tree node with the given label
   *
   * @param parentLabel The label for the node
   * @param children    A <code>List</code> of the children of this node,
   *                    each of which should itself be a <code>LabeledScoredTree</code>
   * @return A new internal tree node
   */
        //@Override
        public override Tree newTreeNode(Label parentLabel, List<Tree> children)
        {
            return new LabeledScoredTreeNode(lf.NewLabel(parentLabel), children);
        }
    }
}