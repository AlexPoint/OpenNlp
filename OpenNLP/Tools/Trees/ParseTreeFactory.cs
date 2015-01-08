using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Ling;

namespace OpenNLP.Tools.Trees
{
    public class ParseTreeFactory : LabeledScoredTreeFactory
    {
        public ParseTreeFactory(ILabelFactory lf) : base(lf)
        {
        }
    }
}