using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Parser;
using OpenNLP.Tools.Util.Ling;

namespace OpenNLP.Tools.Util.Trees
{
    public class ParseTree : Tree
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
            /*var label = this.parse.IsLeaf ? this.parse.Value : this.parse.Type;
            var coreLabel = (CoreLabel)new CoreLabel.CoreLabelFactory().newLabel(label);*/

            // TODO: move this CoreLabel construction logic somewhere appropriate
            var cLabel = new CoreLabel();
            if (this.parse.IsLeaf)
            {
                cLabel.setWord(this.parse.Value);
                cLabel.setBeginPosition(this.parse.Span.Start);
                cLabel.setEndPosition(this.parse.Span.End);
                cLabel.setValue(this.parse.Value);
            }
            else
            {
                cLabel.setCategory(this.parse.Type);
                cLabel.setValue(this.parse.Type);
                if (this.depth() == 1)
                {
                    cLabel.setTag(this.parse.Type);
                }
            }
            return cLabel;
        }

        public override TreeFactory treeFactory()
        {
            LabelFactory lf = (label() == null) ? CoreLabel.factory() : label().labelFactory();
            //return new LabeledScoredTreeFactory(lf);
            return new ParseTreeFactory(lf);
        }
    }
}