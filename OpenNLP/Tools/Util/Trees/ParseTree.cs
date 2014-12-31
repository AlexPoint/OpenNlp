using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Parser;

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

        public override TreeFactory treeFactory()
        {
            throw new NotImplementedException();
        }
    }
}
