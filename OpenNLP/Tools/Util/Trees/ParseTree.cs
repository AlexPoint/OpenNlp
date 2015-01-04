using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Parser;
using OpenNLP.Tools.Util.Ling;

namespace OpenNLP.Tools.Util.Trees
{
    public class ParseTree: Tree
    {
        private Parse parse;
        public ParseTree(Parse p)
        {
            this.parse = p;
        }

        public override Tree[] children()
        {
            return this.parse.GetChildren().Select(ch => new ParseTree(ch)).ToArray();
        }

        public override Label label()
        {
            return new CoreLabel.CoreLabelFactory().newLabel(this.parse.Label);
        }

        public override TreeFactory treeFactory()
        {
            LabelFactory lf = (label() == null) ? CoreLabel.factory() : label().labelFactory();
            //return new LabeledScoredTreeFactory(lf);
            return new ParseTreeFactory(lf);
        }
    }
}
