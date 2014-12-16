using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees.TRegex.Tsurgeon
{
    public abstract class TsurgeonParserTreeConstants : TsurgeonParserConstants
    {
        public int JJTROOT = 0;
        public int JJTOPERATION = 1;
        public int JJTLOCATION = 2;
        public int JJTNODESELECTIONLIST = 3;
        public int JJTNODESELECTION = 4;
        public int JJTNODENAME = 5;
        public int JJTTREELIST = 6;
        public int JJTTREEROOT = 7;
        public int JJTTREENODE = 8;
        public int JJTTREEDTRS = 9;


        public static String[] jjtNodeName = {
    "Root",
    "Operation",
    "Location",
    "NodeSelectionList",
    "NodeSelection",
    "NodeName",
    "TreeList",
    "TreeRoot",
    "TreeNode",
    "TreeDtrs",
  };
    }
}
