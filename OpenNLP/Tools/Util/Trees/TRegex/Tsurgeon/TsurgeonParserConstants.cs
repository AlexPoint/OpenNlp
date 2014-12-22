using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees.TRegex.Tsurgeon
{
    public abstract class TsurgeonParserConstants
    {
        /** End of File. */
        public const int EOF = 0;
        /** RegularExpression Id. */
        public const int OPEN_BRACKET = 5;
        /** RegularExpression Id. */
        public const int IF = 6;
        /** RegularExpression Id. */
        public const int NOT = 7;
        /** RegularExpression Id. */
        public const int EXISTS = 8;
        /** RegularExpression Id. */
        public const int DELETE = 9;
        /** RegularExpression Id. */
        public const int PRUNE = 10;
        /** RegularExpression Id. */
        public const int RELABEL = 11;
        /** RegularExpression Id. */
        public const int EXCISE = 12;
        /** RegularExpression Id. */
        public const int INSERT = 13;
        /** RegularExpression Id. */
        public const int MOVE = 14;
        /** RegularExpression Id. */
        public const int REPLACE = 15;
        /** RegularExpression Id. */
        public const int CREATE_SUBTREE = 16;
        /** RegularExpression Id. */
        public const int ADJOIN = 17;
        /** RegularExpression Id. */
        public const int ADJOIN_TO_HEAD = 18;
        /** RegularExpression Id. */
        public const int ADJOIN_TO_FOOT = 19;
        /** RegularExpression Id. */
        public const int COINDEX = 20;
        /** RegularExpression Id. */
        public const int NAME = 21;
        /** RegularExpression Id. */
        public const int CLOSE_BRACKET = 22;
        /** RegularExpression Id. */
        public const int SELECTION = 23;
        /** RegularExpression Id. */
        public const int GENERAL_RELABEL = 24;
        /** RegularExpression Id. */
        public const int IDENTIFIER = 25;
        /** RegularExpression Id. */
        public const int LOCATION_RELATION = 26;
        /** RegularExpression Id. */
        public const int REGEX = 27;
        /** RegularExpression Id. */
        public const int QUOTEX = 28;
        /** RegularExpression Id. */
        public const int HASH_INTEGER = 29;
        /** RegularExpression Id. */
        public const int TREE_NODE_TERMINAL_LABEL = 30;
        /** RegularExpression Id. */
        public const int TREE_NODE_NONTERMINAL_LABEL = 31;
        /** RegularExpression Id. */
        public const int CLOSE_PAREN = 32;

        /** Lexical state. */
        public const int OPERATION = 0;
        /** Lexical state. */
        public const int CONDITIONAL = 1;
        /** Lexical state. */
        public const int DEFAULT = 2;

        /** Literal token values. */
        public String[] tokenImage = {
    "<EOF>",
    "\" \"",
    "\"\\r\"",
    "\"\\t\"",
    "\"\\n\"",
    "\"[\"",
    "\"if\"",
    "\"not\"",
    "\"exists\"",
    "\"delete\"",
    "\"prune\"",
    "\"relabel\"",
    "\"excise\"",
    "\"insert\"",
    "\"move\"",
    "\"replace\"",
    "\"createSubtree\"",
    "\"adjoin\"",
    "\"adjoinH\"",
    "\"adjoinF\"",
    "\"coindex\"",
    "<NAME>",
    "\"]\"",
    "<SELECTION>",
    "<GENERAL_RELABEL>",
    "<IDENTIFIER>",
    "<LOCATION_RELATION>",
    "<REGEX>",
    "<QUOTEX>",
    "<HASH_INTEGER>",
    "<TREE_NODE_TERMINAL_LABEL>",
    "<TREE_NODE_NONTERMINAL_LABEL>",
    "\")\"",
  };
    }
}
