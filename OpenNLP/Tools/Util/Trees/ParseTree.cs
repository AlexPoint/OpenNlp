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

        public override Tree[] Children()
        {
            return this.parse.GetChildren().Select(ch => new ParseTree(ch)).ToArray();
        }

        public override Label Label()
        {
            /*var label = this.parse.IsLeaf ? this.parse.Value : this.parse.Type;
            var coreLabel = (CoreLabel)new CoreLabel.CoreLabelFactory().newLabel(label);*/

            // TODO: move this CoreLabel construction logic somewhere appropriate
            var cLabel = new CoreLabel();
            if (this.parse.IsLeaf)
            {
                cLabel.SetWord(this.parse.Value);
                cLabel.SetBeginPosition(this.parse.Span.Start);
                cLabel.SetEndPosition(this.parse.Span.End);
                cLabel.SetValue(this.parse.Value);
            }
            else
            {
                cLabel.SetCategory(this.parse.Type);
                cLabel.SetValue(this.parse.Type);
                if (this.Depth() == 1)
                {
                    cLabel.SetTag(this.parse.Type);
                }
            }
            return cLabel;
        }

        public override TreeFactory TreeFactory()
        {
            LabelFactory lf = (Label() == null) ? CoreLabel.Factory() : Label().LabelFactory();
            //return new LabeledScoredTreeFactory(lf);
            return new ParseTreeFactory(lf);
        }
    }
}