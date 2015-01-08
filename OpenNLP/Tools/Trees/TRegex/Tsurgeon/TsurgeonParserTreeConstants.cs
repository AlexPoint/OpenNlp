using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees.TRegex.Tsurgeon
{
    public abstract class TsurgeonParserTreeConstants : TsurgeonParserConstants
    {
        public int JjtRoot = 0;
        public int JjtOperation = 1;
        public int JjtLocation = 2;
        public int JjtNodeSelectionList = 3;
        public int JjtNodeSelection = 4;
        public int JjtNodeName = 5;
        public int JjtTreeList = 6;
        public int JjtTreeRoot = 7;
        public int JjtTreeNode = 8;
        public int JjtTreeDtrs = 9;

        public static string[] JjtNodeNames =
        {
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