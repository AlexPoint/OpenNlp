using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Ling;
using OpenNLP.Tools.Parser;

namespace OpenNLP.Tools.Trees
{
    public class ParseTree : Tree
    {
        private readonly Parse parse;

        public ParseTree(Parse p)
        {
            this.parse = p;
        }

        public override Tree[] Children()
        {
            return this.parse.GetChildren().Select(ch => new ParseTree(ch)).ToArray();
        }

        public override ILabel Label()
        {
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

        public override ITreeFactory TreeFactory()
        {
            ILabelFactory lf = (Label() == null) ? CoreLabel.Factory() : Label().LabelFactory();
            return new ParseTreeFactory(lf);
        }
    }
}