using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Ling;

namespace OpenNLP.Tools.Trees
{
    /// <summary>
    /// A <code>TreeGraphNodeFactory</code> acts as a factory for creating
    /// nodes in a {@link TreeGraph <code>TreeGraph</code>}.
    /// Unless another {@link LabelFactory <code>LabelFactory</code>} is
    /// supplied, it will use a CoreLabelFactory by default.
    /// 
    /// @author Bill MacCartney
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public class TreeGraphNodeFactory : ITreeFactory
    {
        private readonly ILabelFactory mlf;

        /// <summary>
        /// Make a <code>TreeFactory</code> that produces <code>TreeGraphNode</code>s.
        /// The labels are of class <code>CoreLabel</code>.
        /// </summary>
        public TreeGraphNodeFactory() :
            this(CoreLabel.Factory())
        {
        }

        public TreeGraphNodeFactory(ILabelFactory mlf)
        {
            this.mlf = mlf;
        }

        public Tree NewLeaf(string word)
        {
            return NewLeaf(mlf.NewLabel(word));
        }

        public Tree NewLeaf(ILabel label)
        {
            return new TreeGraphNode(label);
        }

        public Tree NewTreeNode(string parent, List<Tree> children)
        {
            return NewTreeNode(mlf.NewLabel(parent), children);
        }

        public Tree NewTreeNode(ILabel parentLabel, List<Tree> children)
        {
            return new TreeGraphNode(parentLabel, children);
        }
    }
}