using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenNLP.Tools.Util;

namespace OpenNLP.Tools.Trees.TRegex.Tsurgeon
{
    public class AuxiliaryTree
    {
        private readonly string originalTreeString;
        public readonly Tree Tree;
        public Tree Foot;
        private readonly IdentityDictionary<Tree, string> nodesToNames;
        private readonly Dictionary<string, Tree> pnamesToNodes;


        public AuxiliaryTree(Tree tree, bool mustHaveFoot)
        {
            originalTreeString = tree.ToString();
            this.Tree = tree;
            this.Foot = FindFootNode(tree);
            if (Foot == null && mustHaveFoot)
            {
                throw new TsurgeonParseException("Error -- no foot node found for " + originalTreeString);
            }
            pnamesToNodes = new Dictionary<string, Tree>();
            nodesToNames = new IdentityDictionary<Tree, string>();
            InitializeNamesNodesMaps(tree);
        }

        private AuxiliaryTree(Tree tree, Tree foot, Dictionary<string, Tree> namesToNodes, string originalTreeString)
        {
            this.originalTreeString = originalTreeString;
            this.Tree = tree;
            this.Foot = foot;
            this.pnamesToNodes = namesToNodes;
            nodesToNames = null;
        }

        public Dictionary<string, Tree> NamesToNodes()
        {
            return pnamesToNodes;
        }

        public override string ToString()
        {
            return originalTreeString;
        }

        /// <summary>
        /// Copies the Auxiliary tree.  Also, puts the new names->nodes map in the TsurgeonMatcher that called copy.
        /// </summary>
        public AuxiliaryTree Copy(TsurgeonMatcher matcher)
        {
            var newNamesToNodes = new Dictionary<string, Tree>();
            Tuple<Tree, Tree> result = CopyHelper(Tree, newNamesToNodes);
            foreach (var entry in newNamesToNodes)
            {
                matcher.NewNodeNames.Add(entry.Key, entry.Value);
            }
            return new AuxiliaryTree(result.Item1, result.Item2, newNamesToNodes, originalTreeString);
        }

        private Tuple<Tree, Tree> CopyHelper(Tree node, Dictionary<string, Tree> newNamesToNodes)
        {
            Tree clone;
            Tree newFoot = null;
            if (node.IsLeaf())
            {
                if (node == Foot)
                {
                    // found the foot node; pass it up.
                    clone = node.TreeFactory().NewTreeNode(node.Label(), new List<Tree>(0));
                    newFoot = clone;
                }
                else
                {
                    clone = node.TreeFactory().NewLeaf(node.Label().LabelFactory().NewLabel(node.Label()));
                }
            }
            else
            {
                var newChildren = new List<Tree>(node.Children().Length);
                foreach (Tree child in node.Children())
                {
                    Tuple<Tree, Tree> newChild = CopyHelper(child, newNamesToNodes);
                    newChildren.Add(newChild.Item1);
                    if (newChild.Item2 != null)
                    {
                        newFoot = newChild.Item2;
                    }
                }
                clone = node.TreeFactory().NewTreeNode(node.Label().LabelFactory().NewLabel(node.Label()), newChildren);
            }

            if (nodesToNames.ContainsKey(node))
            {
                newNamesToNodes.Add(nodesToNames[node], clone);
            }

            return new Tuple<Tree, Tree>(clone, newFoot);
        }



        /***********************************************************/
        /* below here is init stuff for finding the foot node.     */
        /***********************************************************/


        private const string FootNodeCharacter = "@";

        private static readonly Regex FootNodeLabelPattern = new Regex("^(.*)" + FootNodeCharacter + '$',
            RegexOptions.Compiled);

        private static readonly Regex EscapedFootNodeCharacter = new Regex('\\' + FootNodeCharacter,
            RegexOptions.Compiled);

        /// <summary>
        /// Returns the foot node of the adjunction tree, which is the terminal node
        /// that ends in @.  In the process, turns the foot node into a TreeNode
        /// (rather than a leaf), and destructively un-escapes all the escaped
        /// instances of @ in the tree.  Note that readonly @ in a non-terminal node is
        /// ignored, and left in.
        /// </summary>
        private static Tree FindFootNode(Tree t)
        {
            Tree footNode = FindFootNodeHelper(t);
            Tree result = footNode;
            if (footNode != null)
            {
                Tree newFootNode = footNode.TreeFactory().NewTreeNode(footNode.Label(), new List<Tree>());

                Tree parent = footNode.Parent(t);
                if (parent != null)
                {
                    int i = parent.ObjectIndexOf(footNode);
                    parent.SetChild(i, newFootNode);
                }

                result = newFootNode;
            }
            return result;
        }

        private static Tree FindFootNodeHelper(Tree t)
        {
            Tree foundDtr = null;
            if (t.IsLeaf())
            {
                var match = FootNodeLabelPattern.Match(t.Label().Value());
                if (match.Success)
                {
                    t.Label().SetValue(match.Groups[1].Value);
                    return t;
                }
                else
                {
                    return null;
                }
            }
            foreach (Tree child in t.Children())
            {
                Tree thisFoundDtr = FindFootNodeHelper(child);
                if (thisFoundDtr != null)
                {
                    if (foundDtr != null)
                    {
                        throw new TsurgeonParseException("Error -- two foot nodes in subtree" + t.ToString());
                    }
                    else
                    {
                        foundDtr = thisFoundDtr;
                    }
                }
            }
            /*Matcher m = escapedFootNodeCharacter.matcher(t.label().value());
            t.label().setValue(m.replaceAll(footNodeCharacter));*/
            var newS = EscapedFootNodeCharacter.Replace(t.Label().Value(), FootNodeCharacter);
            t.Label().SetValue(newS);
            return foundDtr;
        }


        /***********************************************************
        * below here is init stuff for getting node -> names maps *
        ***********************************************************/

        /// <summary>
        /// There are two ways in which you can can match the start of a name expression.
        /// The first is if you have any number of non-escaping characters
        /// preceding an "=" and a name.  This is the ([^\\\\]*) part.
        /// The second is if you have any number of any characters, followed
        /// by a non-"\" character, as "\" is used to escape the "=".  After
        /// that, any number of pairs of "\" are allowed, as we let "\" also
        /// escape itself.  After that comes "=" and a name.
        /// </summary>
        private static readonly Regex NamePattern = new Regex(
            "^((?:[^\\\\]*)|(?:(?:.*[^\\\\])?)(?:\\\\\\\\)*)=([^=]+)$", RegexOptions.Compiled);

        /// <summary>
        /// Looks for new names, destructively strips them out.
        /// Destructively unescapes escaped chars, including "=", as well.
        /// </summary>
        private void InitializeNamesNodesMaps(Tree t)
        {
            foreach (Tree node in t.SubTreeList())
            {
                var m = NamePattern.Match(node.Label().Value());
                if (m.Success)
                {
                    pnamesToNodes.Add(m.Groups[2].Value, node);
                    nodesToNames.Add(node, m.Groups[2].Value);
                    node.Label().SetValue(m.Groups[1].Value);
                }
                node.Label().SetValue(Unescape(node.Label().Value()));
            }
        }

        private static string Unescape(string input)
        {
            return Regex.Replace(input, "\\\\(.)", "$1");
            //return input.Replace("\\\\(.)", "$1");
        }
    }
}