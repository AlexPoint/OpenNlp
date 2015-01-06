using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees.TRegex
{
    /**
 * A TregexMatcher can be used to match a {@link TregexPattern} against a {@link edu.stanford.nlp.trees.Tree}.
 * Usage should be similar to a {@link java.util.regex.Matcher}.
 *
 * @author Galen Andrew
 */

    public abstract class TregexMatcher
    {
        public readonly Tree root;
        public Tree tree;
        public IdentityDictionary<Tree, Tree> nodesToParents;
        public readonly Dictionary<string, Tree> namesToNodes;
        public readonly VariableStrings variableStrings;

        // these things are used by "find"
        private Tree.TreeIterator findIterator;
        private Tree findCurrent;

        public readonly HeadFinder headFinder;

        public TregexMatcher(Tree root, Tree tree, IdentityDictionary<Tree, Tree> nodesToParents,
            Dictionary<string, Tree> namesToNodes, VariableStrings variableStrings, HeadFinder headFinder)
        {
            this.root = root;
            this.tree = tree;
            this.nodesToParents = nodesToParents;
            this.namesToNodes = namesToNodes;
            this.variableStrings = variableStrings;
            this.headFinder = headFinder;
        }

        public HeadFinder GetHeadFinder()
        {
            return this.headFinder;
        }

        /**
   * Resets the matcher so that its search starts over.
   */

        public void Reset()
        {
            findIterator = null;
            findCurrent = null;
            namesToNodes.Clear();
            variableStrings.Reset();
        }

        /**
   * Resets the matcher to start searching on the given tree for matching subexpressions.
   *
   * @param tree The tree to start searching on
   */

        public virtual void ResetChildIter(Tree tree)
        {
            this.tree = tree;
            ResetChildIter();
        }

        /**
   * Resets the matcher to restart search for matching subexpressions
   */

        public virtual void ResetChildIter()
        {
        }

        /**
   * Does the pattern match the tree?  It's actually closer to java.util.regex's
   * "lookingAt" in that the root of the tree has to match the root of the pattern
   * but the whole tree does not have to be "accounted for".  Like with lookingAt
   * the beginning of the string has to match the pattern, but the whole string
   * doesn't have to be "accounted for".
   *
   * @return whether the tree matches the pattern
   */
        public abstract bool Matches();

        /** Rests the matcher and tests if it matches on the tree when rooted at <code>node</code>.
   *
   *  @param node The node where the match is checked
   *  @return whether the matcher matches at node
   */

        public bool MatchesAt(Tree node)
        {
            ResetChildIter(node);
            return Matches();
        }

        /**
   * Get the last matching tree -- that is, the tree node that matches the root node of the pattern.
   * Returns null if there has not been a match.
   *
   * @return last match
   */
        public abstract Tree GetMatch();


        /**
   * Find the next match of the pattern on the tree
   *
   * @return whether there is a match somewhere in the tree
   */

        public bool Find()
        {
            if (findIterator == null)
            {
                findIterator = root.Iterator();
            }
            if (findCurrent != null && Matches())
            {
                return true;
            }
            while (findIterator.MoveNext())
            {
                findCurrent = findIterator.Current;
                ResetChildIter(findCurrent);
                if (Matches())
                {
                    return true;
                }
            }
            return false;
        }

        /**
   * Similar to {@code find()}, but matches only if {@code node} is
   * the root of the match.  All other matches are ignored.  If you
   * know you are looking for matches with a particular root, this is
   * much faster than iterating over all matches and taking only the
   * ones that work and faster than altering the tregex to match only
   * the correct node.
   * <br>
   * If called multiple times with the same node, this will return
   * subsequent matches in the same manner as find() returns
   * subsequent matches in the same tree.  If you want to call this using
   * the same TregexMatcher on more than one node, call reset() first;
   * otherwise, an AssertionError will be thrown.
   */

        public bool FindAt(Tree node)
        {
            //Console.WriteLine("findAt() on '" + node);
            if (findCurrent != null && findCurrent != node)
            {
                throw new InvalidOperationException(
                    "Error: must call reset() before changing nodes for a call to findAt");
            }
            if (findCurrent != null)
            {
                return Matches();
            }
            findCurrent = node;
            ResetChildIter(findCurrent);
            return Matches();
        }

        /**
   * Find the next match of the pattern on the tree such that the
   * matching node (that is, the tree node matching the root node of
   * the pattern) differs from the previous matching node.
   * @return true iff another matching node is found.
   */

        public bool FindNextMatchingNode()
        {
            Tree lastMatchingNode = GetMatch();
            while (Find())
            {
                if (GetMatch() != lastMatchingNode)
                    return true;
            }
            return false;
        }

        /**
   * Returns the node labeled with <code>name</code> in the pattern.
   *
   * @param name the name of the node, specified in the pattern.
   * @return node labeled by the name
   */

        public Tree GetNode(string name)
        {
            return namesToNodes[name];
        }

        public List<string> GetNodeNames()
        {
            return namesToNodes.Keys.ToList();
        }

        public Tree GetParent(Tree node)
        {
            if (node is HasParent)
            {
                return node.Parent();
            }
            if (nodesToParents == null)
            {
                nodesToParents = new IdentityDictionary<Tree, Tree>();
            }
            if (nodesToParents.Count == 0)
            {
                FillNodesToParents(root, null);
            }
            return nodesToParents[node];
        }

        private void FillNodesToParents(Tree node, Tree parent)
        {
            nodesToParents.Add(node, parent);
            foreach (Tree child in node.Children())
            {
                FillNodesToParents(child, node);
            }
        }

        public Tree GetRoot()
        {
            return root;
        }

        /**
   * If there is a current match, and that match involves setting this
   * particular variable string, this returns that string.  Otherwise,
   * it returns null.
   */

        public string GetVariableString(string var)
        {
            return variableStrings.GetString(var);
        }
    }
}