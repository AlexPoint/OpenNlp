using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees.TRegex.Tsurgeon
{
    public class AuxiliaryTree
    {
        private readonly string originalTreeString;
        public readonly Tree tree;
        public Tree foot;
        private readonly IdentityDictionary<Tree, string> nodesToNames; // no one else should be able to get this one.
        private readonly Dictionary<string, Tree> pnamesToNodes; // this one has a getter.


        public AuxiliaryTree(Tree tree, bool mustHaveFoot)
        {
            originalTreeString = tree.ToString();
            this.tree = tree;
            this.foot = FindFootNode(tree);
            if (foot == null && mustHaveFoot)
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
            this.tree = tree;
            this.foot = foot;
            this.pnamesToNodes = namesToNodes;
            nodesToNames = null;
        }

        public Dictionary<string, Tree> NamesToNodes()
        {
            return pnamesToNodes;
        }

        //@Override
        public override string ToString()
        {
            return originalTreeString;
        }

        /**
   * Copies the Auxiliary tree.  Also, puts the new names->nodes map in the TsurgeonMatcher that called copy.
   */

        public AuxiliaryTree Copy(TsurgeonMatcher matcher)
        {
            var newNamesToNodes = new Dictionary<string, Tree>();
            Tuple<Tree, Tree> result = CopyHelper(tree, newNamesToNodes);
            //if(! result.Item1.dominates(result.Item2))
            //System.err.println("Error -- aux tree copy doesn't dominate foot copy.");
            foreach (var entry in newNamesToNodes)
            {
                matcher.newNodeNames.Add(entry.Key, entry.Value);
            }
            return new AuxiliaryTree(result.Item1, result.Item2, newNamesToNodes, originalTreeString);
        }

        // returns Pair<node,foot>
        private Tuple<Tree, Tree> CopyHelper(Tree node, Dictionary<string, Tree> newNamesToNodes)
        {
            Tree clone;
            Tree newFoot = null;
            if (node.IsLeaf())
            {
                if (node == foot)
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
                        if (newFoot != null)
                        {
                            //System.err.println("Error -- two feet found when copying auxiliary tree " + tree.ToString() + "; using last foot found.");
                        }
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

        /**
   * Returns the foot node of the adjunction tree, which is the terminal node
   * that ends in @.  In the process, turns the foot node into a TreeNode
   * (rather than a leaf), and destructively un-escapes all the escaped
   * instances of @ in the tree.  Note that readonly @ in a non-terminal node is
   * ignored, and left in.
   */

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

        // There are two ways in which you can can match the start of a name
        // expression.
        // The first is if you have any number of non-escaping characters
        // preceding an "=" and a name.  This is the ([^\\\\]*) part.
        // The second is if you have any number of any characters, followed
        // by a non-"\" character, as "\" is used to escape the "=".  After
        // that, any number of pairs of "\" are allowed, as we let "\" also
        // escape itself.  After that comes "=" and a name.
        private static readonly Regex NamePattern = new Regex(
            "^((?:[^\\\\]*)|(?:(?:.*[^\\\\])?)(?:\\\\\\\\)*)=([^=]+)$", RegexOptions.Compiled);

        /**
   * Looks for new names, destructively strips them out.
   * Destructively unescapes escaped chars, including "=", as well.
   */

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