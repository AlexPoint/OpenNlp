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
        public readonly Dictionary<String, Tree> namesToNodes;
        public readonly VariableStrings variableStrings;

        // these things are used by "find"
        private Tree.TreeIterator findIterator;
        private Tree findCurrent;

        public readonly HeadFinder headFinder;

        public TregexMatcher(Tree root, Tree tree, IdentityDictionary<Tree, Tree> nodesToParents,
            Dictionary<String, Tree> namesToNodes, VariableStrings variableStrings, HeadFinder headFinder)
        {
            this.root = root;
            this.tree = tree;
            this.nodesToParents = nodesToParents;
            this.namesToNodes = namesToNodes;
            this.variableStrings = variableStrings;
            this.headFinder = headFinder;
        }

        public HeadFinder getHeadFinder()
        {
            return this.headFinder;
        }

        /**
   * Resets the matcher so that its search starts over.
   */

        public void reset()
        {
            findIterator = null;
            findCurrent = null;
            namesToNodes.Clear();
            variableStrings.reset();
        }

        /**
   * Resets the matcher to start searching on the given tree for matching subexpressions.
   *
   * @param tree The tree to start searching on
   */

        public virtual void resetChildIter(Tree tree)
        {
            this.tree = tree;
            resetChildIter();
        }

        /**
   * Resets the matcher to restart search for matching subexpressions
   */

        public virtual void resetChildIter()
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
        public abstract bool matches();

        /** Rests the matcher and tests if it matches on the tree when rooted at <code>node</code>.
   *
   *  @param node The node where the match is checked
   *  @return whether the matcher matches at node
   */

        public bool matchesAt(Tree node)
        {
            resetChildIter(node);
            return matches();
        }

        /**
   * Get the last matching tree -- that is, the tree node that matches the root node of the pattern.
   * Returns null if there has not been a match.
   *
   * @return last match
   */
        public abstract Tree getMatch();


        /**
   * Find the next match of the pattern on the tree
   *
   * @return whether there is a match somewhere in the tree
   */

        public bool find()
        {
            if (findIterator == null)
            {
                findIterator = root.iterator();
            }
            if (findCurrent != null && matches())
            {
                return true;
            }
            while (findIterator.MoveNext())
            {
                findCurrent = findIterator.Current;
                resetChildIter(findCurrent);
                if (matches())
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

        public bool findAt(Tree node)
        {
            //Console.WriteLine("findAt() on '" + node);
            if (findCurrent != null && findCurrent != node)
            {
                throw new InvalidOperationException(
                    "Error: must call reset() before changing nodes for a call to findAt");
            }
            if (findCurrent != null)
            {
                return matches();
            }
            findCurrent = node;
            resetChildIter(findCurrent);
            return matches();
        }

        /**
   * Find the next match of the pattern on the tree such that the
   * matching node (that is, the tree node matching the root node of
   * the pattern) differs from the previous matching node.
   * @return true iff another matching node is found.
   */

        public bool findNextMatchingNode()
        {
            Tree lastMatchingNode = getMatch();
            while (find())
            {
                if (getMatch() != lastMatchingNode)
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

        public Tree getNode(String name)
        {
            return namesToNodes[name];
        }

        public List<String> getNodeNames()
        {
            return namesToNodes.Keys.ToList();
        }

        public Tree getParent(Tree node)
        {
            if (node is HasParent)
            {
                return node.parent();
            }
            if (nodesToParents == null)
            {
                nodesToParents = new IdentityDictionary<Tree, Tree>();
            }
            if (nodesToParents.Count == 0)
            {
                fillNodesToParents(root, null);
            }
            return nodesToParents[node];
        }

        private void fillNodesToParents(Tree node, Tree parent)
        {
            nodesToParents.Add(node, parent);
            foreach (Tree child in node.children())
            {
                fillNodesToParents(child, node);
            }
        }

        public Tree getRoot()
        {
            return root;
        }

        /**
   * If there is a current match, and that match involves setting this
   * particular variable string, this returns that string.  Otherwise,
   * it returns null.
   */

        public String getVariableString(String var)
        {
            return variableStrings.getString(var);
        }
    }
}