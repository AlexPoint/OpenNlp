using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;
using OpenNLP.Tools.Util;

namespace OpenNLP.Tools.Trees.TRegex
{
    /// <summary>
    /// An abstract base class for relations between tree nodes in tregex. There are
    /// two types of subclasses: static anonymous singleton instantiations for
    /// relations that do not require arguments, and private subclasses for those
    /// with arguments. All invocations should be made through the static factory
    /// methods, which insure that there is only a single instance of each relation.
    /// Thus == can be used instead of .equals.
    /// 
    /// If you want to add a new relation, you just have to fill in the definition of satisfies and
    /// searchNodeIterator. Also be careful to make the appropriate adjustments to
    /// getRelation and SIMPLE_RELATIONS. Finally, if you are using the TregexParser,
    /// you need to add the new relation symbol to the list of tokens.
    /// 
    /// @author Galen Andrew
    /// @author Roger Levy
    /// @author Christopher Manning
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public abstract class Relation
    {
        private readonly string _symbol;

        /// <summary>
        /// Whether this relationship is satisfied between two trees
        /// </summary>
        /// <param name="t1">The tree that is the left operand</param>
        /// <param name="t2">The tree that is the right operand</param>
        /// <param name="root">The common root of t1 and t2</param>
        /// <returns>Whether this relationship is satisfied</returns>
        public abstract bool Satisfies(Tree t1, Tree t2, Tree root,TregexMatcher matcher);

        /// <summary>
        /// For a given node, returns an {@link Iterator} over the nodes
        /// of the tree containing the node that satisfy the relation.
        /// </summary>
        /// <param name="t">A node in a Tree</param>
        /// <param name="matcher">The matcher that nodes have to satisfy</param>
        /// <returns>An Iterator over the nodes of the root tree that satisfy the relation.</returns>
        public abstract IEnumerator<Tree> GetSearchNodeIterator(Tree t,
           TregexMatcher matcher);

        private static readonly Regex ParentOfLastChild = new Regex("(<-|<`)");

        private static readonly Regex LastChildOfParent = new Regex("(>-|>`)");

        /// <summary>
        /// Static factory method for all relations with no arguments. Includes:
        /// DOMINATES, DOMINATED_BY, PARENT_OF, CHILD_OF, PRECEDES,
        /// IMMEDIATELY_PRECEDES, HAS_LEFTMOST_DESCENDANT, HAS_RIGHTMOST_DESCENDANT,
        /// LEFTMOST_DESCENDANT_OF, RIGHTMOST_DESCENDANT_OF, SISTER_OF, LEFT_SISTER_OF,
        /// RIGHT_SISTER_OF, IMMEDIATE_LEFT_SISTER_OF, IMMEDIATE_RIGHT_SISTER_OF,
        /// HEADS, HEADED_BY, IMMEDIATELY_HEADS, IMMEDIATELY_HEADED_BY, ONLY_CHILD_OF,
        /// HAS_ONLY_CHILD, EQUALS
        /// </summary>
        /// <param name="s">The string representation of the relation</param>
        /// <returns>The singleton static relation of the specified type</returns>
        /// <exception cref="ParseException">If bad relation s</exception>
        public static Relation GetRelation(string s, Func<string, string> basicCatFunction, IHeadFinder headFinder)
            /*throws ParseException*/
        {
            if (SimpleRelationsMap.ContainsKey(s))
            {
                return SimpleRelationsMap[s];
            }

            // these are shorthands for relations with arguments
            if (s.Equals("<,"))
            {
                return GetRelation("<", "1", basicCatFunction, headFinder);
            }
            else if (ParentOfLastChild.IsMatch(s))
            {
                return GetRelation("<", "-1", basicCatFunction, headFinder);
            }
            else if (s.Equals(">,"))
            {
                return GetRelation(">", "1", basicCatFunction, headFinder);
            }
            else if (LastChildOfParent.IsMatch(s))
            {
                return GetRelation(">", "-1", basicCatFunction, headFinder);
            }

            // readonlyly try relations with headFinders
            Relation r;
            switch (s)
            {
                case ">>#":
                    r = new Heads(headFinder);
                    break;
                case "<<#":
                    r = new HeadedBy(headFinder);
                    break;
                case ">#":
                    r = new ImmediatelyHeads(headFinder);
                    break;
                case "<#":
                    r = new ImmediatelyHeadedBy(headFinder);
                    break;
                default:
                    throw new ParseException("Unrecognized simple relation " + s);
            }

            return Interner<Relation>.GlobalIntern(r);
        }

        /// <summary>
        /// Static factory method for relations requiring an argument, including
        /// HAS_ITH_CHILD, ITH_CHILD_OF, UNBROKEN_CATEGORY_DOMINATES,
        /// UNBROKEN_CATEGORY_DOMINATED_BY.
        /// </summary>
        /// <param name="s">The string representation of the relation</param>
        /// <param name="arg">The argument to the relation, as a string; could be a node description or an integer</param>
        /// <returns>
        /// The singleton static relation of the specified type with the specified argument. Uses Interner to insure singleton-ity
        /// </returns>
        /// <exception cref="ParseException">If bad relation s</exception>
        public static Relation GetRelation(string s, string arg,
            Func<string, string> basicCatFunction,
            IHeadFinder headFinder)
        {
            if (arg == null)
            {
                return GetRelation(s, basicCatFunction, headFinder);
            }
            Relation r;
            switch (s)
            {
                case "<":
                    r = new HasIthChild(int.Parse(arg));
                    break;
                case ">":
                    r = new IthChildOf(int.Parse(arg));
                    break;
                case "<+":
                    r = new UnbrokenCategoryDominates(arg, basicCatFunction);
                    break;
                case ">+":
                    r = new UnbrokenCategoryIsDominatedBy(arg, basicCatFunction);
                    break;
                case ".+":
                    r = new UnbrokenCategoryPrecedes(arg, basicCatFunction);
                    break;
                case ",+":
                    r = new UnbrokenCategoryFollows(arg, basicCatFunction);
                    break;
                default:
                    throw new ParseException("Unrecognized compound relation " + s + ' '
                                             + arg);
            }
            return Interner<Relation>.GlobalIntern(r);
        }

        /// <summary>
        /// Produce a TregexPattern which represents the given MULTI_RELATION and its children
        /// </summary>
        public static TregexPattern ConstructMultiRelation(string s, List<DescriptionPattern> children,
            Func<string, string> basicCatFunction,
            IHeadFinder headFinder)
        {
            if (s.Equals("<..."))
            {
                var newChildren = new List<TregexPattern>();
                for (int i = 0; i < children.Count; ++i)
                {
                    Relation rel1 = GetRelation("<", (i + 1).ToString(), basicCatFunction, headFinder);
                    DescriptionPattern oldChild = children[i];
                    TregexPattern newChild = new DescriptionPattern(rel1, oldChild);
                    newChildren.Add(newChild);
                }
                Relation rel = GetRelation("<", (children.Count + 1).ToString(), basicCatFunction, headFinder);
                TregexPattern noExtraChildren = new DescriptionPattern(rel, false, "__", null, false, basicCatFunction,
                    new List<Tuple<int, string>>(), false, null);
                noExtraChildren.Negate();
                newChildren.Add(noExtraChildren);
                return new CoordinationPattern(newChildren, true);
            }
            else
            {
                throw new ParseException("Unknown multi relation " + s);
            }
        }

        private Relation(string symbol)
        {
            this._symbol = symbol;
        }

        public override string ToString()
        {
            return _symbol;
        }

        /// <summary>
        /// This abstract Iterator implements a NULL iterator, but by subclassing and
        /// overriding advance and/or initialize, it is an efficient implementation.
        /// </summary>
        private class SearchNodeIterator : IEnumerator<Tree>
        {

            /// <summary>
            /// This is the next tree to be returned by the iterator, or null if there are no more items.
            /// </summary>
            private Tree _next; // = null;

            public SearchNodeIterator()
            {
                _next = null;
            }

            public SearchNodeIterator(Tree tree)
            {
                _next = tree;
            }

            public void Dispose()
            {
                // do nothing
            }

            public bool MoveNext()
            {
                if (_next == null)
                {
                    return false;
                }
                else
                {
                    this.Current = _next;
                    _next = null;
                    return true;
                }
            }

            public void Reset()
            {
                throw new InvalidOperationException("Cannot reset this Enumerator");
            }

            public Tree Current { get; private set; }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }

        private class RootRelation : Relation
        {
            public RootRelation(string symbol) : base(symbol)
            {
            }

            public override bool Satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                return t1 == t2;
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return new SearchNodeIterator(t);
            }
        }

        /// <summary>Used in TregexParser</summary>
        public static readonly Relation Root = new RootRelation("Root");

        private class EqualsRelation : Relation
        {
            public EqualsRelation(string symbol) : base(symbol)
            {
            }

            public override bool Satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                return t1 == t2;
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t,
                TregexMatcher matcher)
            {
                return new List<Tree>() {t}.GetEnumerator();
            }

        }

        private static readonly Relation EqualsRel = new EqualsRelation("==");

        private class PatternSplitterRelation : Relation
        {
            public PatternSplitterRelation(string symbol) : base(symbol)
            {
            }

            public override bool Satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                return true;
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return matcher.GetRoot().Iterator();
            }
        }

        /// <summary>
        /// This is a "dummy" relation that allows you to segment patterns
        /// </summary>
        private static readonly Relation PatternSplitter = new PatternSplitterRelation(":");

        private class DominatesRelation : Relation
        {
            public DominatesRelation(string symbol) : base(symbol)
            {
            }

            public override bool Satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                return t1 != t2 && t1.Dominates(t2);
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return new StackSearchNodeIterator(t);
            }
        }

        private class StackSearchNodeIterator : IEnumerator<Tree>
        {
            private readonly Stack<Tree> _searchStack;

            public StackSearchNodeIterator(Tree t)
            {
                _searchStack = new Stack<Tree>();
                for (int i = t.NumChildren() - 1; i >= 0; i--)
                {
                    _searchStack.Push(t.GetChild(i));
                }
            }

            public void Dispose()
            {
                // do nothing
            }

            public bool MoveNext()
            {
                if (!_searchStack.Any())
                {
                    return false;
                }
                else
                {
                    var elt = _searchStack.Pop();
                    for (int i = elt.NumChildren() - 1; i >= 0; i--)
                    {
                        _searchStack.Push(elt.GetChild(i));
                    }
                    this.Current = elt;
                    return true;
                }
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public Tree Current { get; private set; }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }

        private static readonly Relation Dominates = new DominatesRelation("<<");

        private class UpSearchNodeIterator : IEnumerator<Tree>
        {
            private readonly Tree _tree;
            private readonly TregexMatcher _matcher;

            public UpSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                this._tree = t;
                this._matcher = matcher;
            }

            public void Dispose()
            {
                // do nothing
            }

            public bool MoveNext()
            {
                var parent = this._matcher.GetParent(this._tree);
                if (parent == null)
                {
                    return false;
                }
                else
                {
                    this.Current = parent;
                    return true;
                }
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public Tree Current { get; private set; }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }

        private class DominatedByRelation : Relation
        {
            public DominatedByRelation(string symbol) : base(symbol)
            {
            }

            public override bool Satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                return Dominates.Satisfies(t2, t1, root, matcher);
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return new UpSearchNodeIterator(t, matcher);
            }
        }

        private static readonly Relation DominatedBy = new DominatedByRelation(">>");

        private class ParentRelation : Relation
        {
            public ParentRelation(string symbol) : base(symbol)
            {
            }

            public override bool Satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                Tree[] kids = t1.Children();
                for (int i = 0, n = kids.Length; i < n; i++)
                {
                    if (kids[i] == t2)
                    {
                        return true;
                    }
                }
                return false;
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return t.GetChildrenAsList().GetEnumerator();
            }
        }

        private static readonly Relation ParentOf = new ParentRelation("<");

        private class ChildOfRelation : Relation
        {
            public ChildOfRelation(string symbol) : base(symbol)
            {
            }

            public override bool Satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                return ParentOf.Satisfies(t2, t1, root, matcher);
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return new List<Tree>() {matcher.GetParent(t)}.GetEnumerator();
            }
        }

        private static readonly Relation ChildOf = new ChildOfRelation(">");

        private class PrecedeSearchNodeIterator : IEnumerator<Tree>
        {
            private readonly Stack<Tree> _searchStack;

            public PrecedeSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                _searchStack = new Stack<Tree>();

                Tree current = t;
                Tree parent = matcher.GetParent(t);
                while (parent != null)
                {
                    for (int i = parent.NumChildren() - 1; parent.GetChild(i) != current; i--)
                    {
                        _searchStack.Push(parent.GetChild(i));
                    }
                    current = parent;
                    parent = matcher.GetParent(parent);
                }
            }

            public void Dispose()
            {
                // do nothing
            }

            public bool MoveNext()
            {
                if (!_searchStack.Any())
                {
                    return false;
                }
                else
                {
                    this.Current = _searchStack.Pop();
                    for (int i = this.Current.NumChildren() - 1; i >= 0; i--)
                    {
                        _searchStack.Push(this.Current.GetChild(i));
                    }
                    return true;
                }
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public Tree Current { get; private set; }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }

        private class PrecedesRelation : Relation
        {
            public PrecedesRelation(string symbol) : base(symbol)
            {
            }

            public override bool Satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                return Trees.RightEdge(t1, root) <= Trees.LeftEdge(t2, root);
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return new PrecedeSearchNodeIterator(t, matcher);
            }
        }

        private static readonly Relation Precedes = new PrecedesRelation("..");

        private class ImmediatelyPrecedesSearchNodeIterator : IEnumerator<Tree>
        {
            private readonly Tree _firstNode;

            public ImmediatelyPrecedesSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                Tree current;
                Tree parent = t;
                do
                {
                    current = parent;
                    parent = matcher.GetParent(parent);
                    if (parent == null)
                    {
                        _firstNode = null;
                        return;
                    }
                } while (parent.LastChild() == current);

                for (int i = 1, n = parent.NumChildren(); i < n; i++)
                {
                    if (parent.GetChild(i - 1) == current)
                    {
                        _firstNode = parent.GetChild(i);
                        return;
                    }
                }
            }

            public void Dispose()
            {
                // do nothing
            }

            public bool MoveNext()
            {
                if (this.Current == null)
                {
                    // first call
                    this.Current = _firstNode;
                    return true;
                }
                else if (this.Current.IsLeaf())
                {
                    this.Current = null;
                    return false;
                }
                else
                {
                    this.Current = this.Current.FirstChild();
                    return true;
                }
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public Tree Current { get; private set; }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }

        private class ImmediatelyPrecedesRelation : Relation
        {
            public ImmediatelyPrecedesRelation(string symbol) : base(symbol)
            {
            }

            public override bool Satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                return Trees.LeftEdge(t2, root) == Trees.RightEdge(t1, root);
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return new ImmediatelyPrecedesSearchNodeIterator(t, matcher);
            }
        }

        private static readonly Relation ImmediatelyPrecedes = new ImmediatelyPrecedesRelation(".");

        private class FollowsSearchNodeIterator : IEnumerator<Tree>
        {
            private readonly Stack<Tree> searchStack;

            public FollowsSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                searchStack = new Stack<Tree>();
                Tree current = t;
                Tree parent = matcher.GetParent(t);
                while (parent != null)
                {
                    for (int i = 0; parent.GetChild(i) != current; i++)
                    {
                        searchStack.Push(parent.GetChild(i));
                    }
                    current = parent;
                    parent = matcher.GetParent(parent);
                }
            }

            public void Dispose()
            {
                // do nothing
            }

            public bool MoveNext()
            {
                if (!searchStack.Any())
                {
                    this.Current = null;
                    return false;
                }
                else
                {
                    this.Current = searchStack.Pop();
                    for (int i = this.Current.NumChildren() - 1; i >= 0; i--)
                    {
                        searchStack.Push(this.Current.GetChild(i));
                    }
                    return true;
                }
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public Tree Current { get; private set; }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }

        private class FollowsRelation : Relation
        {
            public FollowsRelation(string symbol) : base(symbol)
            {
            }

            public override bool Satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                return Trees.RightEdge(t2, root) <= Trees.LeftEdge(t1, root);
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                throw new NotImplementedException();
            }
        }

        private static readonly Relation Follows = new FollowsRelation(",,");

        public class ImmediatelyFollowsSearchNodeIterator : IEnumerator<Tree>
        {
            private readonly Tree _firstNode;

            public ImmediatelyFollowsSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                Tree current;
                Tree parent = t;
                do
                {
                    current = parent;
                    parent = matcher.GetParent(parent);
                    if (parent == null)
                    {
                        _firstNode = null;
                        return;
                    }
                } while (parent.FirstChild() == current);

                for (int i = 0, n = parent.NumChildren() - 1; i < n; i++)
                {
                    if (parent.GetChild(i + 1) == current)
                    {
                        _firstNode = parent.GetChild(i);
                        return;
                    }
                }
            }

            public void Dispose()
            {
                // do nothing
            }

            public bool MoveNext()
            {
                if (this.Current == null)
                {
                    this.Current = _firstNode;
                    return true;
                }
                if (this.Current.IsLeaf())
                {
                    this.Current = null;
                    return false;
                }
                else
                {
                    this.Current = this.Current.LastChild();
                    return true;
                }
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public Tree Current { get; private set; }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }

        private class ImmediatelyFolllowsRelation : Relation
        {
            public ImmediatelyFolllowsRelation(string symbol) : base(symbol)
            {
            }

            public override bool Satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                return Trees.LeftEdge(t1, root) == Trees.RightEdge(t2, root);
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return new ImmediatelyFollowsSearchNodeIterator(t, matcher);
            }
        }

        private static readonly Relation ImmediatelyFollows = new ImmediatelyFolllowsRelation(",");

        private class HasLeftmostDescendantSearchNodeIterator : IEnumerator<Tree>
        {

            public HasLeftmostDescendantSearchNodeIterator(Tree t)
            {
                this.Current = t;
            }

            public void Dispose()
            {
                // do nothing
            }

            public bool MoveNext()
            {
                if (this.Current == null)
                {
                    return false;
                }
                else if (this.Current.IsLeaf())
                {
                    this.Current = null;
                    return false;
                }
                else
                {
                    this.Current = this.Current.FirstChild();
                    return true;
                }
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public Tree Current { get; private set; }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }

        private class HasLeftmostDescendantRelation : Relation
        {
            public HasLeftmostDescendantRelation(string symbol) : base(symbol)
            {
            }

            public override bool Satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                if (t1.IsLeaf())
                {
                    return false;
                }
                else
                {
                    return (t1.Children()[0] == t2) || Satisfies(t1.Children()[0], t2, root, matcher);
                }
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return new HasLeftmostDescendantSearchNodeIterator(t);
            }
        }

        private static readonly Relation HasLeftmostDescendant = new HasLeftmostDescendantRelation("<<,");

        private class HasRighmostDescendantSearchNodeIterator : IEnumerator<Tree>
        {
            public HasRighmostDescendantSearchNodeIterator(Tree t)
            {
                this.Current = t;
            }

            public void Dispose()
            {
                // do nothing
            }

            public bool MoveNext()
            {
                if (this.Current == null)
                {
                    return false;
                }
                else if (this.Current.IsLeaf())
                {
                    this.Current = null;
                    return false;
                }
                else
                {
                    this.Current = this.Current.LastChild();
                    return true;
                }
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public Tree Current { get; private set; }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }

        private class HasRightmostDescendantRelation : Relation
        {
            public HasRightmostDescendantRelation(string symbol) : base(symbol)
            {
            }

            public override bool Satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                if (t1.IsLeaf())
                {
                    return false;
                }
                else
                {
                    Tree lastKid = t1.Children()[t1.Children().Length - 1];
                    return (lastKid == t2) || Satisfies(lastKid, t2, root, matcher);
                }
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return new HasRighmostDescendantSearchNodeIterator(t);
            }
        }

        private static readonly Relation HasRightmostDescendant = new HasRightmostDescendantRelation("<<-");

        private class LeftmostDescendantOfSearchNodeIterator : IEnumerator<Tree>
        {
            private Tree _next;
            private readonly TregexMatcher _matcher;

            public LeftmostDescendantOfSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                _next = t;
                this._matcher = matcher;
            }

            public void Dispose()
            {
                // do nothing
            }

            public bool MoveNext()
            {
                if (_next == null)
                {
                    return false;
                }
                else
                {
                    this.Current = _next;
                    _next = this._matcher.GetParent(_next);
                    if (_next != null && _next.FirstChild() != this.Current)
                    {
                        _next = null;
                    }
                    return true;
                }
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public Tree Current { get; private set; }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }

        private class LeftmostDescendantOfRelation : Relation
        {
            public LeftmostDescendantOfRelation(string symbol) : base(symbol)
            {
            }

            public override bool Satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                return HasLeftmostDescendant.Satisfies(t2, t1, root, matcher);
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return new LeftmostDescendantOfSearchNodeIterator(t, matcher);
            }
        }

        private static readonly Relation LeftmostDescendantOf = new LeftmostDescendantOfRelation(">>,");

        private class RightmostDescendantOfSearchNodeIterator : IEnumerator<Tree>
        {
            private Tree _next;
            private readonly TregexMatcher _matcher;

            public RightmostDescendantOfSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                _next = t;
                this._matcher = matcher;
            }

            public void Dispose()
            {
                // do nothing
            }

            public bool MoveNext()
            {
                if (_next == null)
                {
                    return false;
                }
                else
                {
                    this.Current = _next;
                    _next = this._matcher.GetParent(_next);
                    if (_next != null && _next.LastChild() != this.Current)
                    {
                        _next = null;
                    }
                    return true;
                }
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public Tree Current { get; private set; }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }

        private class RightmostDescendantOfRelation : Relation
        {
            public RightmostDescendantOfRelation(string symbol) : base(symbol)
            {
            }

            public override bool Satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                return HasRightmostDescendant.Satisfies(t2, t1, root, matcher);
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return new RightmostDescendantOfSearchNodeIterator(t, matcher);
            }
        }

        private static readonly Relation RightmostDescendantOf = new RightmostDescendantOfRelation(">>-");

        private class SisterOfSearchNodeIterator : IEnumerator<Tree>
        {
            private readonly Tree _parent;
            private int _nextNum;
            private readonly Tree _originalNode;

            public SisterOfSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                _originalNode = t;
                _parent = matcher.GetParent(t);
                if (_parent != null)
                {
                    _nextNum = 0;
                }
            }

            public void Dispose()
            {
                // do nothing
            }

            public bool MoveNext()
            {
                if (_nextNum < _parent.NumChildren())
                {
                    this.Current = _parent.GetChild(_nextNum++);
                    if (this.Current == _originalNode)
                    {
                        return MoveNext();
                    }
                    return true;
                }
                else
                {
                    this.Current = null;
                    return false;
                }
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public Tree Current { get; private set; }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }

        private class SisterOfRelation : Relation
        {
            public SisterOfRelation(string symbol) : base(symbol)
            {
            }

            public override bool Satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                if (t1 == t2 || t1 == root)
                {
                    return false;
                }
                Tree parent = t1.Parent(root);
                return ParentOf.Satisfies(parent, t2, root, matcher);
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return new SisterOfSearchNodeIterator(t, matcher);
            }
        }

        private static readonly Relation SisterOf = new SisterOfRelation("$");

        private class LeftSisterOfSearchNodeIterator : IEnumerator<Tree>
        {
            private readonly Tree _parent;
            private int _nextNum;
            private readonly Tree _originalNode;

            public LeftSisterOfSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                _originalNode = t;
                _parent = matcher.GetParent(t);
                if (_parent != null)
                {
                    _nextNum = _parent.NumChildren() - 1;
                }
            }

            public void Dispose()
            {
                // do nothing
            }

            public bool MoveNext()
            {
                this.Current = _parent.GetChild(_nextNum--);
                if (this.Current == _originalNode)
                {
                    this.Current = null;
                    return false;
                }
                return true;
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public Tree Current { get; private set; }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }

        private class LeftSisterOfRelation : Relation
        {
            public LeftSisterOfRelation(string symbol) : base(symbol)
            {
            }

            public override bool Satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                if (t1 == t2 || t1 == root)
                {
                    return false;
                }
                Tree parent = t1.Parent(root);
                Tree[] kids = parent.Children();
                for (int i = kids.Length - 1; i > 0; i--)
                {
                    if (kids[i] == t1)
                    {
                        return false;
                    }
                    if (kids[i] == t2)
                    {
                        return true;
                    }
                }
                return false;
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return new LeftSisterOfSearchNodeIterator(t, matcher);
            }
        }

        private static readonly Relation LeftSisterOf = new LeftSisterOfRelation("$++");

        private class RightSisterOfSearchNodeIterator : IEnumerator<Tree>
        {
            private readonly Tree _parent;
            private readonly Tree _originalNode;
            private int _nextNum;

            public RightSisterOfSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                _originalNode = t;
                _parent = matcher.GetParent(t);
                if (_parent != null)
                {
                    _nextNum = 0;
                }
            }

            public void Dispose()
            {
                // do nothing
            }

            public bool MoveNext()
            {
                this.Current = _parent.GetChild(_nextNum++);
                if (this.Current == _originalNode)
                {
                    this.Current = null;
                    return false;
                }
                return true;
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public Tree Current { get; private set; }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }

        private class RightSisterOfRelation : Relation
        {
            public RightSisterOfRelation(string symbol) : base(symbol)
            {
            }

            public override bool Satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                return LeftSisterOf.Satisfies(t2, t1, root, matcher);
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return new RightSisterOfSearchNodeIterator(t, matcher);
            }
        }

        private static readonly Relation RightSisterOf = new RightSisterOfRelation("$--");

        private class ImmediateLeftSisterOfRelation : Relation
        {
            public ImmediateLeftSisterOfRelation(string symbol) : base(symbol)
            {
            }

            public override bool Satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                if (t1 == t2 || t1 == root)
                {
                    return false;
                }
                Tree[] sisters = t1.Parent(root).Children();
                for (int i = sisters.Length - 1; i > 0; i--)
                {
                    if (sisters[i] == t1)
                    {
                        return false;
                    }
                    if (sisters[i] == t2)
                    {
                        return sisters[i - 1] == t1;
                    }
                }
                return false;
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                if (t != matcher.GetRoot())
                {
                    Tree parent = matcher.GetParent(t);
                    int i = 0;
                    while (parent.GetChild(i) != t)
                    {
                        i++;
                    }
                    if (i + 1 < parent.NumChildren())
                    {
                        var node = parent.GetChild(i + 1);
                        return new List<Tree>() {node}.GetEnumerator();
                    }
                }
                return new List<Tree>().GetEnumerator();
            }
        }

        private static readonly Relation ImmediateLeftSisterOf = new ImmediateLeftSisterOfRelation("$+");

        private class ImmediateRightSisterOfRelation : Relation
        {
            public ImmediateRightSisterOfRelation(string symbol) : base(symbol)
            {
            }

            public override bool Satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                return ImmediateLeftSisterOf.Satisfies(t2, t1, root, matcher);
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                if (t != matcher.GetRoot())
                {
                    Tree parent = matcher.GetParent(t);
                    int i = 0;
                    while (parent.GetChild(i) != t)
                    {
                        i++;
                    }
                    if (i > 0)
                    {
                        var node = parent.GetChild(i - 1);
                        return new List<Tree>() {node}.GetEnumerator();
                    }
                }

                return new List<Tree>().GetEnumerator();
            }
        }

        private static readonly Relation ImmediateRightSisterOf = new ImmediateRightSisterOfRelation("$-");

        private class OnlyChildOfRelation : Relation
        {
            public OnlyChildOfRelation(string symbol) : base(symbol)
            {
            }

            public override bool Satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                return t2.Children().Length == 1 && t2.FirstChild() == t1;
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                if (t != matcher.GetRoot())
                {
                    var next = matcher.GetParent(t);
                    if (next.NumChildren() == 1)
                    {
                        return new List<Tree>() {next}.GetEnumerator();
                    }
                }
                return new List<Tree>().GetEnumerator();
            }
        }

        private static readonly Relation OnlyChildOf = new OnlyChildOfRelation(">:");

        private class HasOnlyChildRelation : Relation
        {
            public HasOnlyChildRelation(string symbol) : base(symbol)
            {
            }

            public override bool Satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                return t1.Children().Length == 1 && t1.FirstChild() == t2;
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                if (!t.IsLeaf() && t.NumChildren() == 1)
                {
                    var next = t.FirstChild();
                    return new List<Tree>() {next}.GetEnumerator();
                }
                else
                {
                    return new List<Tree>().GetEnumerator();
                }
            }
        }

        private static readonly Relation HasOnlyChild = new HasOnlyChildRelation("<:");

        private class UnaryPathAncestorSeachNodeIterator : IEnumerator<Tree>
        {
            private readonly Stack<Tree> _searchStack;

            public UnaryPathAncestorSeachNodeIterator(Tree t)
            {
                _searchStack = new Stack<Tree>();
                if (!t.IsLeaf() && t.Children().Length == 1)
                    _searchStack.Push(t.GetChild(0));
            }

            public void Dispose()
            {
                // do nothing
            }

            public bool MoveNext()
            {
                if (!_searchStack.Any())
                {
                    this.Current = null;
                    return false;
                }
                else
                {
                    this.Current = _searchStack.Pop();
                    if (!this.Current.IsLeaf() && this.Current.Children().Length == 1)
                    {
                        _searchStack.Push(this.Current.GetChild(0));
                    }
                    return true;
                }
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public Tree Current { get; private set; }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }

        private class UnaryPathAncestorOfRelation : Relation
        {
            public UnaryPathAncestorOfRelation(string symbol) : base(symbol)
            {
            }

            public override bool Satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                if (t1.IsLeaf() || t1.Children().Length > 1)
                    return false;
                Tree onlyDtr = t1.Children()[0];
                if (onlyDtr == t2)
                    return true;
                else
                    return Satisfies(onlyDtr, t2, root, matcher);
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return new UnaryPathAncestorSeachNodeIterator(t);
            }
        }

        private static readonly Relation UnaryPathAncestorOf = new UnaryPathAncestorOfRelation("<<:");

        private class UnaryPathDescendantSearchNodeIterator : IEnumerator<Tree>
        {
            private readonly Stack<Tree> _searchStack;
            private readonly TregexMatcher _matcher;

            public UnaryPathDescendantSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                this._matcher = matcher;
                _searchStack = new Stack<Tree>();
                Tree parent = matcher.GetParent(t);
                if (parent != null && !parent.IsLeaf() && parent.Children().Length == 1)
                {
                    _searchStack.Push(parent);
                }
            }

            public void Dispose()
            {
                // do nothing
            }

            public bool MoveNext()
            {
                if (!_searchStack.Any())
                {
                    this.Current = null;
                    return false;
                }
                else
                {
                    this.Current = _searchStack.Pop();
                    Tree parent = _matcher.GetParent(this.Current);
                    if (parent != null && !parent.IsLeaf() && parent.Children().Length == 1)
                    {
                        _searchStack.Push(parent);
                    }
                    return true;
                }
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public Tree Current { get; private set; }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }

        private class UnaryPathDescendantOfRelation : Relation
        {
            public UnaryPathDescendantOfRelation(string symbol) : base(symbol)
            {
            }

            public override bool Satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                if (t2.IsLeaf() || t2.Children().Length > 1)
                {
                    return false;
                }
                Tree onlyDtr = t2.Children()[0];
                if (onlyDtr == t1)
                {
                    return true;
                }
                else
                {
                    return Satisfies(t1, onlyDtr, root, matcher);
                }
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return new UnaryPathDescendantSearchNodeIterator(t, matcher);
            }
        }

        private static readonly Relation UnaryPathDescendantOf = new UnaryPathDescendantOfRelation(">>:");

        private class ParentEqualsRelation : Relation
        {
            public ParentEqualsRelation(string symbol) : base(symbol)
            {
            }

            public override bool Satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                if (t1 == t2)
                {
                    return true;
                }
                return ParentOf.Satisfies(t1, t2, root, matcher);
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                // return parent and children
                var elements = new List<Tree>(){t};
                elements.AddRange(t.GetChildrenAsList());
                return elements.GetEnumerator();
            }
        }

        private static readonly Relation ParentEquals = new ParentEqualsRelation("<=");

        private static readonly Relation[] SimpleRelations =
        {
            Dominates, DominatedBy, ParentOf, ChildOf, Precedes,
            ImmediatelyPrecedes, Follows, ImmediatelyFollows,
            HasLeftmostDescendant, HasRightmostDescendant,
            LeftmostDescendantOf, RightmostDescendantOf, SisterOf,
            LeftSisterOf, RightSisterOf, ImmediateLeftSisterOf,
            ImmediateRightSisterOf, OnlyChildOf, HasOnlyChild, EqualsRel,
            PatternSplitter, UnaryPathAncestorOf, UnaryPathDescendantOf,
            ParentEquals
        };

        private static readonly Dictionary<string, Relation> AdditionalRelationMap = new Dictionary<string, Relation>()
        {
            {"<<`", HasRightmostDescendant},
            {"<<,", HasLeftmostDescendant},
            {">>`", RightmostDescendantOf},
            {">>,", LeftmostDescendantOf},
            {"$..", LeftSisterOf},
            {"$,,", RightSisterOf},
            {"$.", ImmediateLeftSisterOf},
            {"$,", ImmediateRightSisterOf}
        };

        private static readonly Dictionary<string, Relation> SimpleRelationsMap = SimpleRelations
            .ToDictionary(r => r._symbol, r => r)
            .Union(AdditionalRelationMap)
            .ToDictionary(ent => ent.Key, ent => ent.Value);

        public override bool Equals(Object o)
        {
            if (this == o)
            {
                return true;
            }
            if (!(o is Relation))
            {
                return false;
            }

            var relation = (Relation) o;
            return _symbol.Equals(relation._symbol);
        }

        public override int GetHashCode()
        {
            return _symbol.GetHashCode();
        }


        private class Heads : Relation
        {
            public readonly IHeadFinder hf;

            public Heads(IHeadFinder hf) : base(">>#")
            {
                this.hf = hf;
            }

            public override bool Satisfies(Tree t1, Tree t2, Tree root,TregexMatcher matcher)
            {
                if (t2.IsLeaf())
                {
                    return false;
                }
                else if (t2.IsPreTerminal())
                {
                    return (t2.FirstChild() == t1);
                }
                else
                {
                    IHeadFinder headFinder = matcher.GetHeadFinder();
                    if (headFinder == null) headFinder = this.hf;
                    Tree head = headFinder.DetermineHead(t2);
                    if (head == t1)
                    {
                        return true;
                    }
                    else
                    {
                        return Satisfies(t1, head, root, matcher);
                    }
                }
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t,
               TregexMatcher matcher)
            {
                var next = t;

                IHeadFinder headFinder = matcher.GetHeadFinder();
                if (headFinder == null)
                {
                    headFinder = this.hf;
                }

                Tree last = next;
                next = matcher.GetParent(next);
                if (next != null && headFinder.DetermineHead(next) != last)
                {
                    next = null;
                    return new List<Tree>().GetEnumerator();
                }
                else
                {
                    return new List<Tree>() {next}.GetEnumerator();
                }
            }

            public override bool Equals(Object o)
            {
                if (this == o)
                {
                    return true;
                }
                if (!(o is Heads))
                {
                    return false;
                }
                if (!base.Equals(o))
                {
                    return false;
                }

                var heads = (Heads) o;
                if (hf != null ? !hf.Equals(heads.hf) : heads.hf != null)
                {
                    return false;
                }
                return true;
            }

            public override int GetHashCode()
            {
                int result = base.GetHashCode();
                result = 29*result + (hf != null ? hf.GetHashCode() : 0);
                return result;
            }
        }

        private class HeadedByIterator : IEnumerator<Tree>
        {
            private readonly Tree _initialNode;
            private readonly IHeadFinder _hf;
            private bool _initialized;

            public HeadedByIterator(Tree t, TregexMatcher matcher, IHeadFinder hf)
            {
                this._initialNode = t;
                this._hf = hf;
                this._initialized = false;
            }

            public void Dispose()
            {
                // do nothing
            }

            public bool MoveNext()
            {
                if (!_initialized)
                {
                    this._initialized = true;
                    if (this._initialNode.IsLeaf())
                    {
                        this.Current = null;
                        return false;
                    }
                    else
                    {
                        this.Current = this._hf.DetermineHead(this._initialNode);
                        return true;
                    }
                }
                else
                {
                    if (this.Current.IsLeaf())
                    {
                        this.Current = null;
                        return false;
                    }
                    else
                    {
                        this.Current = this._hf.DetermineHead(this.Current);
                        return true;
                    }
                }
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public Tree Current { get; private set; }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }

        private class HeadedBy : Relation
        {
            private readonly Heads _heads;

            public HeadedBy(IHeadFinder hf) : base("<<#")
            {
                this._heads = Interner<Heads>.GlobalIntern(new Heads(hf));
            }

            public override bool Satisfies(Tree t1, Tree t2, Tree root,TregexMatcher matcher)
            {
                return _heads.Satisfies(t2, t1, root, matcher);
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t,
               TregexMatcher matcher)
            {
                var finder = matcher.GetHeadFinder() != null ? matcher.GetHeadFinder() : this._heads.hf;
                return new HeadedByIterator(t, matcher, finder);
            }

            public override bool Equals(Object o)
            {
                if (this == o)
                {
                    return true;
                }
                if (!(o is HeadedBy))
                {
                    return false;
                }
                if (!base.Equals(o))
                {
                    return false;
                }

                var headedBy = (HeadedBy) o;
                if (_heads != null
                    ? !_heads.Equals(headedBy._heads)
                    : headedBy._heads != null)
                {
                    return false;
                }

                return true;
            }
            
            public override int GetHashCode()
            {
                int result = base.GetHashCode();
                result = 29*result + (_heads != null ? _heads.GetHashCode() : 0);
                return result;
            }
        }


        private class ImmediatelyHeads : Relation
        {
            public readonly IHeadFinder Hf;

            public ImmediatelyHeads(IHeadFinder hf) : base(">#")
            {
                this.Hf = hf;
            }

            public override bool Satisfies(Tree t1, Tree t2, Tree root,TregexMatcher matcher)
            {
                if (matcher.GetHeadFinder() != null)
                {
                    return matcher.GetHeadFinder().DetermineHead(t2) == t1;
                }
                else
                {
                    return Hf.DetermineHead(t2) == t1;
                }
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t,
               TregexMatcher matcher)
            {
                if (t != matcher.GetRoot())
                {
                    var next = matcher.GetParent(t);
                    IHeadFinder headFinder = matcher.GetHeadFinder() == null ? Hf : matcher.GetHeadFinder();
                    if (headFinder.DetermineHead(next) == t)
                    {
                        return new List<Tree>() {next}.GetEnumerator();
                    }
                }
                return new List<Tree>().GetEnumerator();
            }

            public override bool Equals(Object o)
            {
                if (this == o)
                {
                    return true;
                }
                if (!(o is ImmediatelyHeads))
                {
                    return false;
                }
                if (!base.Equals(o))
                {
                    return false;
                }

                var immediatelyHeads = (ImmediatelyHeads) o;
                if (Hf != null ? !Hf.Equals(immediatelyHeads.Hf) : immediatelyHeads.Hf != null)
                {
                    return false;
                }

                return true;
            }
            
            public override int GetHashCode()
            {
                int result = base.GetHashCode();
                result = 29*result + (Hf != null ? Hf.GetHashCode() : 0);
                return result;
            }
        }


        private class ImmediatelyHeadedBy : Relation
        {
            private readonly ImmediatelyHeads _immediatelyHeads;

            public ImmediatelyHeadedBy(IHeadFinder hf) : base("<#")
            {
                this._immediatelyHeads = Interner<ImmediatelyHeads>
                    .GlobalIntern(new ImmediatelyHeads(hf));
            }
            
            public override bool Satisfies(Tree t1, Tree t2, Tree root,TregexMatcher matcher)
            {
                return _immediatelyHeads.Satisfies(t2, t1, root, matcher);
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t,
               TregexMatcher matcher)
            {
                if (!t.IsLeaf())
                {
                    var headFinder = matcher.GetHeadFinder() != null ? matcher.GetHeadFinder() : _immediatelyHeads.Hf;
                    var next = headFinder.DetermineHead(t);
                    return new List<Tree>() {next}.GetEnumerator();
                }
                return new List<Tree>().GetEnumerator();
            }
            
            public override bool Equals(Object o)
            {
                if (this == o)
                {
                    return true;
                }
                if (!(o is ImmediatelyHeadedBy))
                {
                    return false;
                }
                if (!base.Equals(o))
                {
                    return false;
                }

                var immediatelyHeadedBy = (ImmediatelyHeadedBy) o;
                if (_immediatelyHeads != null
                    ? !_immediatelyHeads
                        .Equals(immediatelyHeadedBy._immediatelyHeads)
                    : immediatelyHeadedBy._immediatelyHeads != null)
                {
                    return false;
                }
                return true;
            }

            public override int GetHashCode()
            {
                int result = base.GetHashCode();
                result = 29*result
                         + (_immediatelyHeads != null ? _immediatelyHeads.GetHashCode() : 0);
                return result;
            }
        }


        private class IthChildOf : Relation
        {
            public readonly int ChildNum;

            public IthChildOf(int i) : base(">" + i)
            {
                if (i == 0)
                {
                    throw new ArgumentException(
                        "Error -- no such thing as zeroth child!");
                }
                else
                {
                    ChildNum = i;
                }
            }
            
            public override bool Satisfies(Tree t1, Tree t2, Tree root,TregexMatcher matcher)
            {
                Tree[] kids = t2.Children();
                if (kids.Length < Math.Abs(ChildNum))
                {
                    return false;
                }
                if (ChildNum > 0 && kids[ChildNum - 1] == t1)
                {
                    return true;
                }
                if (ChildNum < 0 && kids[kids.Length + ChildNum] == t1)
                {
                    return true;
                }
                return false;
            }
            
            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t,
               TregexMatcher matcher)
            {
                if (t != matcher.GetRoot())
                {
                    var next = matcher.GetParent(t);
                    if (ChildNum > 0
                        && (next.NumChildren() < ChildNum || next
                            .GetChild(ChildNum - 1) != t)
                        || ChildNum < 0
                        && (next.NumChildren() < -ChildNum || next.GetChild(next
                            .NumChildren()
                                                                            + ChildNum) != t))
                    {
                        next = null;
                    }
                    if (next != null)
                    {
                        return new List<Tree>() {next}.GetEnumerator();
                    }
                }
                return new List<Tree>().GetEnumerator();
            }

            public override bool Equals(Object o)
            {
                if (this == o)
                {
                    return true;
                }
                if (!(o is IthChildOf))
                {
                    return false;
                }

                var ithChildOf = (IthChildOf) o;
                if (ChildNum != ithChildOf.ChildNum)
                {
                    return false;
                }

                return true;
            }
            
            public override int GetHashCode()
            {
                return ChildNum;
            }

        }


        private class HasIthChild : Relation
        {
            private readonly IthChildOf _ithChildOf;

            public HasIthChild(int i) : base("<" + i)
            {
                _ithChildOf = Interner<IthChildOf>.GlobalIntern(new IthChildOf(i));
            }
            
            public override bool Satisfies(Tree t1, Tree t2, Tree root,TregexMatcher matcher)
            {
                return _ithChildOf.Satisfies(t2, t1, root, matcher);
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t,
               TregexMatcher matcher)
            {
                int childNum = _ithChildOf.ChildNum;
                if (t.NumChildren() >= Math.Abs(childNum))
                {
                    Tree next;
                    if (childNum > 0)
                    {
                        next = t.GetChild(childNum - 1);
                    }
                    else
                    {
                        next = t.GetChild(t.NumChildren() + childNum);
                    }
                    return new List<Tree>() {next}.GetEnumerator();
                }
                return new List<Tree>().GetEnumerator();
            }

            public override bool Equals(Object o)
            {
                if (this == o)
                {
                    return true;
                }
                if (!(o is HasIthChild))
                {
                    return false;
                }
                if (!base.Equals(o))
                {
                    return false;
                }

                var hasIthChild = (HasIthChild) o;
                if (_ithChildOf != null
                    ? !_ithChildOf.Equals(hasIthChild._ithChildOf)
                    : hasIthChild._ithChildOf != null)
                {
                    return false;
                }

                return true;
            }

            public override int GetHashCode()
            {
                int result = base.GetHashCode();
                result = 29*result + (_ithChildOf != null ? _ithChildOf.GetHashCode() : 0);
                return result;
            }
        }

        private class UnbrokenCategoryDominatesIterator : IEnumerator<Tree>
        {
            private readonly Stack<Tree> _searchStack;
            private readonly Func<Tree, bool> _patchMatchesNode;

            public UnbrokenCategoryDominatesIterator(Tree t, Func<Tree, bool> patchMatchesNode)
            {
                _searchStack = new Stack<Tree>();
                for (int i = t.NumChildren() - 1; i >= 0; i--)
                {
                    _searchStack.Push(t.GetChild(i));
                }
                this._patchMatchesNode = patchMatchesNode;
            }

            public void Dispose()
            {
                // do nothing
            }

            public bool MoveNext()
            {
                if (!_searchStack.Any())
                {
                    this.Current = null;
                    return false;
                }
                else
                {
                    this.Current = _searchStack.Pop();
                    if (this._patchMatchesNode(this.Current))
                    {
                        for (int i = this.Current.NumChildren() - 1; i >= 0; i--)
                        {
                            _searchStack.Push(this.Current.GetChild(i));
                        }
                    }
                    return true;
                }
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public Tree Current { get; private set; }

            object IEnumerator.Current
            {
                get { return Current; }
            }

        }


        private class UnbrokenCategoryDominates : Relation
        {
            private readonly Regex _pattern;
            private readonly bool _negatedPattern;
            private readonly bool _basicCat;
            private readonly Func<string, string> _basicCatFunction;


            /// <summary>
            /// 
            /// </summary>
            /// <param name="arg">
            /// This may have a ! and then maybe a @ and then either an identifier or regex
            /// </param>
            public UnbrokenCategoryDominates(string arg,
                Func<string, string> basicCatFunction) : base("<+(" + arg + ')')
            {
                if (arg.StartsWith("!"))
                {
                    _negatedPattern = true;
                    arg = arg.Substring(1);
                }
                else
                {
                    _negatedPattern = false;
                }
                if (arg.StartsWith("@"))
                {
                    _basicCat = true;
                    this._basicCatFunction = basicCatFunction;
                    arg = arg.Substring(1);
                }
                else
                {
                    _basicCat = false;
                }
                if (Regex.IsMatch(arg, "/.*/"))
                {
                    //pattern = new Regex(arg.Substring(1, arg.Length - 1));
                    _pattern = new Regex(arg.Substring(1, arg.Length - 2));
                }
                else if (Regex.IsMatch(arg, "__"))
                {
                    _pattern = new Regex("^.*$");
                }
                else
                {
                    _pattern = new Regex("^(?:" + arg + ")$");
                }
            }
            
            public override bool Satisfies(Tree t1, Tree t2, Tree root,TregexMatcher matcher)
            {
                foreach (Tree kid in t1.Children())
                {
                    if (kid == t2)
                    {
                        return true;
                    }
                    else
                    {
                        if (PathMatchesNode(kid) && Satisfies(kid, t2, root, matcher))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            public bool PathMatchesNode(Tree node)
            {
                string lab = node.Value();
                // added this code to not crash if null node, even though there probably should be null nodes in the tree
                if (lab == null)
                {
                    // Say that a null label matches no positive pattern, but any negated patern
                    return _negatedPattern;
                }
                else
                {
                    if (_basicCat)
                    {
                        lab = _basicCatFunction(lab);
                    }
                    var m = _pattern.Match(lab);
                    return m.Success != _negatedPattern;
                }
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t,
               TregexMatcher matcher)
            {
                return new UnbrokenCategoryDominatesIterator(t, this.PathMatchesNode);
            }

            public override bool Equals(Object o)
            {
                if (this == o)
                {
                    return true;
                }
                if (!(o is UnbrokenCategoryDominates))
                {
                    return false;
                }
                
                var unbrokenCategoryDominates = (UnbrokenCategoryDominates) o;
                if (_negatedPattern != unbrokenCategoryDominates._negatedPattern)
                {
                    return false;
                }
                if (!_pattern.Equals(unbrokenCategoryDominates._pattern))
                {
                    return false;
                }

                return true;
            }

            public override int GetHashCode()
            {
                int result = _pattern.GetHashCode();
                result = 29*result + (_negatedPattern ? 1 : 0);
                return result;
            }

        } // end class UnbrokenCategoryDominates


        private class UnbrokenCategoryIsDominatedBy : Relation
        {
            private readonly UnbrokenCategoryDominates _unbrokenCategoryDominates;

            public UnbrokenCategoryIsDominatedBy(string arg,
                Func<string, string> basicCatFunction) : base(">+(" + arg + ')')
            {
                _unbrokenCategoryDominates = Interner<UnbrokenCategoryDominates>
                    .GlobalIntern((new UnbrokenCategoryDominates(arg, basicCatFunction)));
            }

            public override bool Satisfies(Tree t1, Tree t2, Tree root,TregexMatcher matcher)
            {
                return _unbrokenCategoryDominates.Satisfies(t2, t1, root, matcher);
            }
            
            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t,
               TregexMatcher matcher)
            {
                var nodes = new List<Tree>();
                var node = t;
                while (_unbrokenCategoryDominates.PathMatchesNode(node))
                {
                    node = matcher.GetParent(node);
                    nodes.Add(node);
                }
                return nodes.GetEnumerator();
            }

            public override bool Equals(Object o)
            {
                if (this == o)
                {
                    return true;
                }
                if (!(o is UnbrokenCategoryIsDominatedBy))
                {
                    return false;
                }
                if (!base.Equals(o))
                {
                    return false;
                }

                var unbrokenCategoryIsDominatedBy = (UnbrokenCategoryIsDominatedBy) o;
                return _unbrokenCategoryDominates.Equals(unbrokenCategoryIsDominatedBy._unbrokenCategoryDominates);
            }

            public override int GetHashCode()
            {
                int result = base.GetHashCode();
                result = 29*result + _unbrokenCategoryDominates.GetHashCode();
                return result;
            }
        }

        /// <summary>
        /// Note that this only works properly for context-free trees.
        /// Also, the use of initialize and advance is not very efficient just yet.  Finally, each node in the tree
        /// is added only once, even if there is more than one unbroken-category precedence path to it.
        /// </summary>
        private class UnbrokenCategoryPrecedes : Relation
        {
            private readonly Regex _pattern;
            private readonly bool _negatedPattern;
            private readonly bool _basicCat;
            private readonly Func<string, string> _basicCatFunction;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="arg">The pattern to match, perhaps preceded by ! and/or @</param>
            public UnbrokenCategoryPrecedes(string arg,
                Func<string, string> basicCatFunction) : base(".+(" + arg + ')')
            {
                if (arg.StartsWith("!"))
                {
                    _negatedPattern = true;
                    arg = arg.Substring(1);
                }
                else
                {
                    _negatedPattern = false;
                }
                if (arg.StartsWith("@"))
                {
                    _basicCat = true;
                    this._basicCatFunction = basicCatFunction;
                    // TODO -- this was missing a this. which must be testable in a unit test!!! Make one
                    arg = arg.Substring(1);
                }
                else
                {
                    _basicCat = false;
                }
                if (Regex.IsMatch(arg, "/.*/"))
                {
                    //pattern = new Regex(arg.Substring(1, arg.Length - 1));
                    _pattern = new Regex(arg.Substring(1, arg.Length - 2));
                }
                else if (Regex.IsMatch(arg, "__"))
                {
                    _pattern = new Regex("^.*$");
                }
                else
                {
                    _pattern = new Regex("^(?:" + arg + ")$");
                }
            }

            public override bool Satisfies(Tree t1, Tree t2, Tree root,TregexMatcher matcher)
            {
                // shouldn't have to do anything here.
                return true;
            }

            private bool PathMatchesNode(Tree node)
            {
                string lab = node.Value();
                // added this code to not crash if null node, even though there probably should be null nodes in the tree
                if (lab == null)
                {
                    // Say that a null label matches no positive pattern, but any negated pattern
                    return _negatedPattern;
                }
                else
                {
                    if (_basicCat)
                    {
                        lab = _basicCatFunction(lab);
                    }
                    var m = _pattern.Match(lab);
                    return m.Success != _negatedPattern;
                }
            }

            private void InitializeHelper(Stack<Tree> stack, Tree node, Tree root, TregexMatcher matcher,
                IdentityHashSet<Tree> nodesToSearch)
            {
                if (node == root)
                {
                    return;
                }
                Tree parent = matcher.GetParent(node);
                int i = parent.ObjectIndexOf(node);
                while (i == parent.Children().Length - 1 && parent != root)
                {
                    node = parent;
                    parent = matcher.GetParent(parent);
                    i = parent.ObjectIndexOf(node);
                }
                Tree followingNode;
                if (i + 1 < parent.Children().Length)
                {
                    followingNode = parent.Children()[i + 1];
                }
                else
                {
                    followingNode = null;
                }
                while (followingNode != null)
                {
                    if (! nodesToSearch.Contains(followingNode))
                    {
                        stack.Push(followingNode);
                        nodesToSearch.Add(followingNode);
                    }
                    if (PathMatchesNode(followingNode))
                    {
                        InitializeHelper(stack, followingNode, root, matcher, nodesToSearch);
                    }
                    if (! followingNode.IsLeaf())
                    {
                        followingNode = followingNode.Children()[0];
                    }
                    else
                    {
                        followingNode = null;
                    }
                }
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t,
               TregexMatcher matcher)
            {
                var stack = new Stack<Tree>();
                InitializeHelper(stack, t, matcher.GetRoot(), matcher, new IdentityHashSet<Tree>());

                return stack.GetEnumerator();
            }
        }

        /// <summary>
        /// Note that this only works properly for context-free trees.
        /// Also, the use of initialize and advance is not very efficient just yet.  Finally, each node in the tree
        /// is added only once, even if there is more than one unbroken-category precedence path to it.
        /// </summary>
        private class UnbrokenCategoryFollows : Relation
        {
            private readonly Regex _pattern;
            private readonly bool _negatedPattern;
            private readonly bool _basicCat;
            private readonly Func<string, string> _basicCatFunction;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="arg">The pattern to match, perhaps preceded by ! and/or @</param>
            public UnbrokenCategoryFollows(string arg,
                Func<string, string> basicCatFunction) : base(",+(" + arg + ')')
            {
                if (arg.StartsWith("!"))
                {
                    _negatedPattern = true;
                    arg = arg.Substring(1);
                }
                else
                {
                    _negatedPattern = false;
                }
                if (arg.StartsWith("@"))
                {
                    _basicCat = true;
                    this._basicCatFunction = basicCatFunction;
                    arg = arg.Substring(1);
                }
                else
                {
                    _basicCat = false;
                }
                if (Regex.IsMatch(arg, "/.*/"))
                {
                    //pattern = new Regex(arg.Substring(1, arg.Length - 1));
                    _pattern = new Regex(arg.Substring(1, arg.Length - 2));
                }
                else if (Regex.IsMatch(arg, "__"))
                {
                    _pattern = new Regex("^.*$");
                }
                else
                {
                    _pattern = new Regex("^(?:" + arg + ")$");
                }
            }

            public override bool Satisfies(Tree t1, Tree t2, Tree root,TregexMatcher matcher)
            {
                // shouldn't have to do anything here
                return true;
            }

            private bool PathMatchesNode(Tree node)
            {
                string lab = node.Value();
                // added this code to not crash if null node, even though there probably should be null nodes in the tree
                if (lab == null)
                {
                    // Say that a null label matches no positive pattern, but any negated pattern
                    return _negatedPattern;
                }
                else
                {
                    if (_basicCat)
                    {
                        lab = _basicCatFunction(lab);
                    }
                    var m = _pattern.Match(lab);
                    return m.Success != _negatedPattern;
                }
            }

            private void InitializeHelper(Stack<Tree> stack, Tree node, Tree root, TregexMatcher matcher,
                IdentityHashSet<Tree> nodesToSearch)
            {
                if (node == root)
                {
                    return;
                }
                Tree parent = matcher.GetParent(node);
                int i = parent.ObjectIndexOf(node);
                while (i == 0 && parent != root)
                {
                    node = parent;
                    parent = matcher.GetParent(parent);
                    i = parent.ObjectIndexOf(node);
                }
                Tree precedingNode;
                if (i > 0)
                {
                    precedingNode = parent.Children()[i - 1];
                }
                else
                {
                    precedingNode = null;
                }
                while (precedingNode != null)
                {
                    if (! nodesToSearch.Contains(precedingNode))
                    {
                        stack.Push(precedingNode);
                        nodesToSearch.Add(precedingNode);
                    }
                    if (PathMatchesNode(precedingNode))
                    {
                        InitializeHelper(stack, precedingNode, root, matcher, nodesToSearch);
                    }
                    if (! precedingNode.IsLeaf())
                    {
                        precedingNode = precedingNode.Children()[0];
                    }
                    else
                    {
                        precedingNode = null;
                    }
                }
            }

            public override IEnumerator<Tree> GetSearchNodeIterator(Tree t,
               TregexMatcher matcher)
            {
                var searchStack = new Stack<Tree>();
                InitializeHelper(searchStack, t, matcher.GetRoot(), matcher, new IdentityHashSet<Tree>());

                return searchStack.GetEnumerator();
            }
        }
    }
}