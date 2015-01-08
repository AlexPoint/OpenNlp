using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util;

namespace OpenNLP.Tools.Trees.TRegex
{
    public class CoordinationPattern : TregexPattern
    {
        private readonly bool isConj;
        private readonly List<TregexPattern> children;

        /// <summary>
        /// if isConj is true, then it is an "AND" ; if it is false, it is an "OR"
        /// </summary>
        public CoordinationPattern(List<TregexPattern> children, bool isConj)
        {
            if (children.Count < 2)
            {
                throw new SystemException("Coordination node must have at least 2 children.");
            }
            this.children = children;
            this.isConj = isConj;
        }

        public override List<TregexPattern> GetChildren()
        {
            return children;
        }

        public override string LocalString()
        {
            return (isConj ? "and" : "or");
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (isConj)
            {
                if (IsNegated())
                {
                    sb.Append("!(");
                }
                foreach (TregexPattern node in children)
                {
                    sb.Append(node.ToString());
                }
                if (IsNegated())
                {
                    sb.Append(")");
                }
            }
            else
            {
                if (IsNegated())
                {
                    sb.Append("!");
                }
                sb.Append('[');
                sb.Append(string.Join(" |", children));
                /*for (Iterator<TregexPattern> iter = children.iterator(); iter.hasNext();) {
                TregexPattern node = iter.next();
                sb.Append(node.ToString());
                if (iter.hasNext()) {
                    sb.Append(" |");
                }
                }*/
                sb.Append(']');
            }
            return sb.ToString();
        }

        public override TregexMatcher Matcher(Tree root, Tree tree,
            IdentityDictionary<Tree, Tree> nodesToParents,
            Dictionary<string, Tree> namesToNodes,
            VariableStrings variableStrings,
            IHeadFinder headFinder)
        {
            return new CoordinationMatcher(this, root, tree, nodesToParents, namesToNodes, variableStrings, headFinder);
        }

        private class CoordinationMatcher : TregexMatcher
        {
            private readonly TregexMatcher[] children;
            private readonly CoordinationPattern myNode;
            private int currChild;
            private readonly bool considerAll;
            // do all con/dis-juncts have to be considered to determine a match?
            // i.e. true if conj and not negated or disj and negated

            public CoordinationMatcher(CoordinationPattern n, Tree root, Tree tree,
                IdentityDictionary<Tree, Tree> nodesToParents,
                Dictionary<string, Tree> namesToNodes,
                VariableStrings variableStrings,
                IHeadFinder headFinder) :
                    base(root, tree, nodesToParents, namesToNodes, variableStrings, headFinder)
            {
                myNode = n;
                children = new TregexMatcher[myNode.children.Count];
                // lazy initialize the children... don't set children[i] yet

                //for (int i = 0; i < children.length; i++) {
                //  TregexPattern node = myNode.children.get(i);
                //  children[i] = node.matcher(root, tree, nodesToParents,
                //                             namesToNodes, variableStrings);
                //}
                currChild = 0;
                considerAll = myNode.isConj ^ myNode.IsNegated();
            }

            public override void ResetChildIter()
            {
                currChild = 0;
                foreach (TregexMatcher child in children)
                {
                    if (child != null)
                    {
                        child.ResetChildIter();
                    }
                }
            }

            public override void ResetChildIter(Tree tree)
            {
                this.tree = tree;
                currChild = 0;
                foreach (TregexMatcher child in children)
                {
                    if (child != null)
                    {
                        child.ResetChildIter(tree);
                    }
                }
            }

            /// <summary>
            /// find the next local match
            /// </summary>
            public override bool Matches()
            {
                // also known as "FUN WITH LOGIC"
                //Console.WriteLine("matches()");
                if (considerAll)
                {
                    // these are the cases where all children must be considered to match
                    if (currChild < 0)
                    {
                        // a past call to this node either got that it failed
                        // matching or that it was a negative match that succeeded,
                        // which we only want to accept once
                        return myNode.IsOptional();
                    }

                    // we must have happily reached the end of a match the last
                    // time we were here
                    if (currChild == children.Length)
                    {
                        --currChild;
                    }

                    while (true)
                    {
                        if (children[currChild] == null)
                        {
                            children[currChild] = myNode.children[currChild].Matcher(root, tree, nodesToParents,
                                namesToNodes, variableStrings, headFinder);
                            children[currChild].ResetChildIter(tree);
                        }
                        if (myNode.IsNegated() != children[currChild].Matches())
                        {
                            // This node is set correctly.  Move on to the next node
                            ++currChild;

                            if (currChild == children.Length)
                            {
                                // yay, all nodes matched.
                                if (myNode.IsNegated())
                                {
                                    // a negated node should only match once (before being reset)
                                    currChild = -1;
                                }
                                return true;
                            }
                        }
                        else
                        {
                            // oops, this didn't work.
                            children[currChild].ResetChildIter();
                            // go backwards to see if we can continue matching from an
                            // earlier location.
                            --currChild;
                            if (currChild < 0)
                            {
                                return myNode.IsOptional();
                            }
                        }
                    }
                }
                else
                {
                    // these are the cases where a single child node can make you match
                    for (; currChild < children.Length; currChild++)
                    {
                        if (children[currChild] == null)
                        {
                            children[currChild] = myNode.children[currChild].Matcher(root, tree, nodesToParents,
                                namesToNodes, variableStrings, headFinder);
                            children[currChild].ResetChildIter(tree);
                        }
                        if (myNode.IsNegated() != children[currChild].Matches())
                        {
                            // a negated node should only match once (before being reset)
                            // otherwise you get repeated matches for every node that
                            // causes the negated match to pass, which would be silly
                            if (myNode.IsNegated())
                            {
                                currChild = children.Length;
                            }
                            return true;
                        }
                    }
                    if (myNode.IsNegated())
                    {
                        currChild = children.Length;
                    }
                    for (int resetChild = 0; resetChild < currChild; ++resetChild)
                    {
                        // clean up variables that may have been set in previously
                        // accepted nodes
                        if (children[resetChild] != null)
                        {
                            children[resetChild].ResetChildIter();
                        }
                    }
                    return myNode.IsOptional();
                }
            }

            public override Tree GetMatch()
            {
                // in general, only DescriptionNodes can match
                // exception: if we are a positive disjunction, we care about
                // exactly one of the children, so we return its match
                if (!myNode.isConj && !myNode.IsNegated())
                {
                    if (currChild >= children.Length || currChild < 0 || children[currChild] == null)
                    {
                        return null;
                    }
                    else
                    {
                        return children[currChild].GetMatch();
                    }
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        } // end private class CoordinationMatcher

    }
}