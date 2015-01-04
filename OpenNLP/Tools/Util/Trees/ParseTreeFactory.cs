using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Ling;

namespace OpenNLP.Tools.Util.Trees
{
    public class ParseTreeFactory : LabeledScoredTreeFactory
    {
        public ParseTreeFactory(LabelFactory lf):base(lf){}
    }
}
