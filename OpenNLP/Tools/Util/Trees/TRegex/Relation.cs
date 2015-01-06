using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;

namespace OpenNLP.Tools.Util.Trees.TRegex
{
    /**
 * An abstract base class for relations between tree nodes in tregex. There are
 * two types of subclasses: static anonymous singleton instantiations for
 * relations that do not require arguments, and private subclasses for those
 * with arguments. All invocations should be made through the static factory
 * methods, which insure that there is only a single instance of each relation.
 * Thus == can be used instead of .equals.
 * <p/>
 * If you want to add a new
 * relation, you just have to fill in the definition of satisfies and
 * searchNodeIterator. Also be careful to make the appropriate adjustments to
 * getRelation and SIMPLE_RELATIONS. Finally, if you are using the TregexParser,
 * you need to add the new relation symbol to the list of tokens.
 *
 * @author Galen Andrew
 * @author Roger Levy
 * @author Christopher Manning
 */

    public abstract class Relation
    {
        /**
   *
   */
        private static readonly long serialVersionUID = -1564793674551362909L;

        private readonly string symbol;

        /** Whether this relationship is satisfied between two trees.
   *
   * @param t1 The tree that is the left operand.
   * @param t2 The tree that is the right operand.
   * @param root The common root of t1 and t2
   * @return Whether this relationship is satisfied.
   */
        public abstract bool satisfies(Tree t1, Tree t2, Tree root, /*readonly*/ TregexMatcher matcher);

        /**
   * For a given node, returns an {@link Iterator} over the nodes
   * of the tree containing the node that satisfy the relation.
   *
   * @param t A node in a Tree
   * @param matcher The matcher that nodes have to satisfy
   * @return An Iterator over the nodes
   *     of the root tree that satisfy the relation.
   */

        public abstract IEnumerator<Tree> searchNodeIterator( /*readonly*/ Tree t,
            /*readonly*/ TregexMatcher matcher);

        private static readonly Regex parentOfLastChild = new Regex("(<-|<`)");

        private static readonly Regex lastChildOfParent = new Regex("(>-|>`)");

        /**
   * Static factory method for all relations with no arguments. Includes:
   * DOMINATES, DOMINATED_BY, PARENT_OF, CHILD_OF, PRECEDES,
   * IMMEDIATELY_PRECEDES, HAS_LEFTMOST_DESCENDANT, HAS_RIGHTMOST_DESCENDANT,
   * LEFTMOST_DESCENDANT_OF, RIGHTMOST_DESCENDANT_OF, SISTER_OF, LEFT_SISTER_OF,
   * RIGHT_SISTER_OF, IMMEDIATE_LEFT_SISTER_OF, IMMEDIATE_RIGHT_SISTER_OF,
   * HEADS, HEADED_BY, IMMEDIATELY_HEADS, IMMEDIATELY_HEADED_BY, ONLY_CHILD_OF,
   * HAS_ONLY_CHILD, EQUALS
   *
   * @param s The string representation of the relation
   * @return The singleton static relation of the specified type
   * @throws ParseException If bad relation s
   */

        public static Relation getRelation(string s,
            Func<string, string> basicCatFunction,
            HeadFinder headFinder)
            /*throws ParseException*/
        {
            if (SIMPLE_RELATIONS_MAP.ContainsKey(s))
            {
                return SIMPLE_RELATIONS_MAP[s];
            }

            // these are shorthands for relations with arguments
            if (s.Equals("<,"))
            {
                return getRelation("<", "1", basicCatFunction, headFinder);
            }
            else if (parentOfLastChild.IsMatch(s))
            {
                return getRelation("<", "-1", basicCatFunction, headFinder);
            }
            else if (s.Equals(">,"))
            {
                return getRelation(">", "1", basicCatFunction, headFinder);
            }
            else if (lastChildOfParent.IsMatch(s))
            {
                return getRelation(">", "-1", basicCatFunction, headFinder);
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

        /**
   * Static factory method for relations requiring an argument, including
   * HAS_ITH_CHILD, ITH_CHILD_OF, UNBROKEN_CATEGORY_DOMINATES,
   * UNBROKEN_CATEGORY_DOMINATED_BY.
   *
   * @param s The string representation of the relation
   * @param arg The argument to the relation, as a string; could be a node
   *          description or an integer
   * @return The singleton static relation of the specified type with the
   *         specified argument. Uses Interner to insure singleton-ity
   * @throws ParseException If bad relation s
   */

        public static Relation getRelation(string s, string arg,
            Func<string, string> basicCatFunction,
            HeadFinder headFinder)
            /*throws ParseException*/
        {
            if (arg == null)
            {
                return getRelation(s, basicCatFunction, headFinder);
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

        /**
   * Produce a TregexPattern which represents the given MULTI_RELATION
   * and its children
   */

        public static TregexPattern constructMultiRelation(string s, List<DescriptionPattern> children,
            Func<string, string> basicCatFunction,
            HeadFinder headFinder) /*throws ParseException */
        {
            if (s.Equals("<..."))
            {
                List<TregexPattern> newChildren = new List<TregexPattern>();
                for (int i = 0; i < children.Count; ++i)
                {
                    Relation rel1 = getRelation("<", (i + 1).ToString(), basicCatFunction, headFinder);
                    DescriptionPattern oldChild = children[i];
                    TregexPattern newChild = new DescriptionPattern(rel1, oldChild);
                    newChildren.Add(newChild);
                }
                Relation rel = getRelation("<", (children.Count + 1).ToString(), basicCatFunction, headFinder);
                TregexPattern noExtraChildren = new DescriptionPattern(rel, false, "__", null, false, basicCatFunction,
                    new List<Tuple<int, string>>(), false, null);
                noExtraChildren.negate();
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
            this.symbol = symbol;
        }

        //@Override
        public override string ToString()
        {
            return symbol;
        }

        /**
   * This abstract Iterator implements a NULL iterator, but by subclassing and
   * overriding advance and/or initialize, it is an efficient implementation.
   */
        /*abstract*/ /*static*/

        private class SearchNodeIterator : IEnumerator<Tree>
        {

            /**
     * This is the next tree to be returned by the iterator, or null if there
     * are no more items.
     */
            private Tree pnext; // = null;

            public SearchNodeIterator()
            {
                pnext = null;
            }

            public SearchNodeIterator(Tree tree)
            {
                pnext = tree;
            }

            public void Dispose()
            {
                // do nothing
            }

            public bool MoveNext()
            {
                if (pnext == null)
                {
                    return false;
                }
                else
                {
                    this.Current = pnext;
                    pnext = null;
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

            public override bool satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                return t1 == t2;
            }

            public override IEnumerator<Tree> searchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return new SearchNodeIterator(t);
            }
        }

        public static readonly Relation ROOT = new RootRelation("Root"); /*{  // used in TregexParser

    private static readonly long serialVersionUID = -8311913236233762612L;

    @Override
    bool satisfies(Tree t1, Tree t2, Tree root, readonly TregexMatcher matcher) {
      return t1 == t2;
    }

    @Override
    Iterator<Tree> searchNodeIterator(readonly Tree t,
                                      readonly TregexMatcher matcher) {
      return new SearchNodeIterator() {
        @Override
        void initialize() {
          next = t;
        }
      };
    }
  };*/

        private class EqualsRelation : Relation
        {
            private new static readonly long serialVersionUID = 164629344977943816L;

            //@Override
            public EqualsRelation(string symbol) : base(symbol)
            {
            }

            public override bool satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                return t1 == t2;
            }

            //@Override
            public override IEnumerator<Tree> searchNodeIterator(Tree t,
                TregexMatcher matcher)
            {
                return new List<Tree>() {t}.GetEnumerator();
            }

        }

        private static readonly Relation EQUALS = new EqualsRelation("=="); /*{

    private static readonly long serialVersionUID = 164629344977943816L;

    @Override
    bool satisfies(Tree t1, Tree t2, Tree root, readonly TregexMatcher matcher) {
      return t1 == t2;
    }

    @Override
    Iterator<Tree> searchNodeIterator(readonly Tree t,
                                      readonly TregexMatcher matcher) {
      return Collections.singletonList(t).iterator();
    }

  };*/

        private class PatternSplitterRelation : Relation
        {
            public PatternSplitterRelation(string symbol) : base(symbol)
            {
            }

            public override bool satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                return true;
            }

            public override IEnumerator<Tree> searchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return matcher.getRoot().iterator();
            }
        }

        /* this is a "dummy" relation that allows you to segment patterns. */

        private static readonly Relation PATTERN_SPLITTER = new PatternSplitterRelation(":"); /*{

    private static readonly long serialVersionUID = 3409941930361386114L;

    @Override
    bool satisfies(Tree t1, Tree t2, Tree root, readonly TregexMatcher matcher) {
      return true;
    }

    @Override
    Iterator<Tree> searchNodeIterator(readonly Tree t,
                                      readonly TregexMatcher matcher) {
      return matcher.getRoot().iterator();
    }
  };*/

        private class DominatesRelation : Relation
        {
            public DominatesRelation(string symbol) : base(symbol)
            {
            }

            public override bool satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                return t1 != t2 && t1.dominates(t2);
            }

            public override IEnumerator<Tree> searchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return new StackSearchNodeIterator(t);
            }
        }

        private class StackSearchNodeIterator : IEnumerator<Tree>
        {
            private Stack<Tree> searchStack;

            public StackSearchNodeIterator(Tree t)
            {
                searchStack = new Stack<Tree>();
                for (int i = t.numChildren() - 1; i >= 0; i--)
                {
                    searchStack.Push(t.getChild(i));
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
                    return false;
                }
                else
                {
                    var elt = searchStack.Pop();
                    for (int i = elt.numChildren() - 1; i >= 0; i--)
                    {
                        searchStack.Push(elt.getChild(i));
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

        private static readonly Relation DOMINATES = new DominatesRelation("<<"); /* {

    private static readonly long serialVersionUID = -2580199434621268260L;

    @Override
    bool satisfies(Tree t1, Tree t2, Tree root, readonly TregexMatcher matcher) {
      return t1 != t2 && t1.dominates(t2);
    }

    @Override
    Iterator<Tree> searchNodeIterator(readonly Tree t,
                                      readonly TregexMatcher matcher) {
      return new SearchNodeIterator() {
        Stack<Tree> searchStack;

        @Override
        public void initialize() {
          searchStack = new Stack<Tree>();
          for (int i = t.numChildren() - 1; i >= 0; i--) {
            searchStack.push(t.getChild(i));
          }
          if (!searchStack.isEmpty()) {
            advance();
          }
        }

        @Override
        void advance() {
          if (searchStack.isEmpty()) {
            next = null;
          } else {
            next = searchStack.pop();
            for (int i = next.numChildren() - 1; i >= 0; i--) {
              searchStack.push(next.getChild(i));
            }
          }
        }
      };
    }
  };*/

        private class UpSearchNodeIterator : IEnumerator<Tree>
        {
            private Tree tree;
            private TregexMatcher matcher;

            public UpSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                this.tree = t;
                this.matcher = matcher;
            }

            public void Dispose()
            {
                // do nothing
            }

            public bool MoveNext()
            {
                var parent = this.matcher.getParent(this.tree);
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

            public override bool satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                return DOMINATES.satisfies(t2, t1, root, matcher);
            }

            public override IEnumerator<Tree> searchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return new UpSearchNodeIterator(t, matcher);
            }
        }

        private static readonly Relation DOMINATED_BY = new DominatedByRelation(">>"); /*{

    private static readonly long serialVersionUID = 6140614010121387690L;

    @Override
    bool satisfies(Tree t1, Tree t2, Tree root, /*readonly#1# TregexMatcher matcher) {
      return DOMINATES.satisfies(t2, t1, root, matcher);
    }

    @Override
    Iterator<Tree> searchNodeIterator(/*readonly#1# Tree t,
                                      /*readonly#1# TregexMatcher matcher) {
      return new SearchNodeIterator() {
        @Override
        void initialize() {
          next = matcher.getParent(t);
        }

        @Override
        public void advance() {
          next = matcher.getParent(next);
        }
      };
    }
  };*/

        private class ParentRelation : Relation
        {
            public ParentRelation(string symbol) : base(symbol)
            {
            }

            public override bool satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                Tree[] kids = t1.children();
                for (int i = 0, n = kids.Length; i < n; i++)
                {
                    if (kids[i] == t2)
                    {
                        return true;
                    }
                }
                return false;
            }

            public override IEnumerator<Tree> searchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return t.getChildrenAsList().GetEnumerator();
            }
        }

        private static readonly Relation PARENT_OF = new ParentRelation("<"); /*{

    private static readonly long serialVersionUID = 9140193735607580808L;

    @Override
    bool satisfies(Tree t1, Tree t2, Tree root, readonly TregexMatcher matcher) {
      Tree[] kids = t1.children();
      for (int i = 0, n = kids.Length; i < n; i++) {
        if (kids[i] == t2) {
          return true;
        }
      }
      return false;
    }

    @Override
    Iterator<Tree> searchNodeIterator(readonly Tree t,
                                      readonly TregexMatcher matcher) {
      return new SearchNodeIterator() {
        // subtle bug warning here: if we use 
        //   int nextNum=0;
        // instead, we get the first daughter twice because the
        // assignment occurs after advance() has already been called
        // once by the constructor of SearchNodeIterator.
        int nextNum;

        @Override
        public void advance() {
          if (nextNum < t.numChildren()) {
            next = t.getChild(nextNum);
            nextNum++;
          } else {
            next = null;
          }
        }
      };
    }
  };*/

        private class ChildOfRelation : Relation
        {
            public ChildOfRelation(string symbol) : base(symbol)
            {
            }

            public override bool satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                return PARENT_OF.satisfies(t2, t1, root, matcher);
            }

            public override IEnumerator<Tree> searchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return new List<Tree>() {matcher.getParent(t)}.GetEnumerator();
            }
        }

        private static readonly Relation CHILD_OF = new ChildOfRelation(">"); /*{

    private static readonly long serialVersionUID = 8919710375433372537L;

    @Override
    bool satisfies(Tree t1, Tree t2, Tree root, readonly TregexMatcher matcher) {
      return PARENT_OF.satisfies(t2, t1, root, matcher);
    }

    @Override
    Iterator<Tree> searchNodeIterator(/*readonly#1# Tree t,
                                      /*readonly#1# TregexMatcher matcher) {
      return new SearchNodeIterator() {
        @Override
        void initialize() {
          next = matcher.getParent(t);
        }
      };
    }
  };*/

        private class PrecedeSearchNodeIterator : IEnumerator<Tree>
        {
            private Stack<Tree> searchStack;
            private TregexMatcher matcher;

            public PrecedeSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                searchStack = new Stack<Tree>();
                this.matcher = matcher;

                Tree current = t;
                Tree parent = matcher.getParent(t);
                while (parent != null)
                {
                    for (int i = parent.numChildren() - 1; parent.getChild(i) != current; i--)
                    {
                        searchStack.Push(parent.getChild(i));
                    }
                    current = parent;
                    parent = matcher.getParent(parent);
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
                    return false;
                }
                else
                {
                    this.Current = searchStack.Pop();
                    for (int i = this.Current.numChildren() - 1; i >= 0; i--)
                    {
                        searchStack.Push(this.Current.getChild(i));
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

            public override bool satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                return Trees.rightEdge(t1, root) <= Trees.leftEdge(t2, root);
            }

            public override IEnumerator<Tree> searchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return new PrecedeSearchNodeIterator(t, matcher);
            }
        }

        private static readonly Relation PRECEDES = new PrecedesRelation(".."); /*{

    private static readonly long serialVersionUID = -9065012389549976867L;

    @Override
    bool satisfies(Tree t1, Tree t2, Tree root, /*readonly#1# TregexMatcher matcher) {
      return Trees.rightEdge(t1, root) <= Trees.leftEdge(t2, root);
    }

    @Override
    Iterator<Tree> searchNodeIterator(/*readonly#1# Tree t,
                                      /*readonly#1# TregexMatcher matcher) {
      return new SearchNodeIterator() {
        Stack<Tree> searchStack;

        @Override
        public void initialize() {
          searchStack = new Stack<Tree>();
          Tree current = t;
          Tree parent = matcher.getParent(t);
          while (parent != null) {
            for (int i = parent.numChildren() - 1; parent.getChild(i) != current; i--) {
              searchStack.push(parent.getChild(i));
            }
            current = parent;
            parent = matcher.getParent(parent);
          }
          advance();
        }

        @Override
        void advance() {
          if (searchStack.isEmpty()) {
            next = null;
          } else {
            next = searchStack.pop();
            for (int i = next.numChildren() - 1; i >= 0; i--) {
              searchStack.push(next.getChild(i));
            }
          }
        }
      };
    }
  };*/

        private class ImmediatelyPrecedesSearchNodeIterator : IEnumerator<Tree>
        {
            private Tree firstNode;

            public ImmediatelyPrecedesSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                Tree current;
                Tree parent = t;
                do
                {
                    current = parent;
                    parent = matcher.getParent(parent);
                    if (parent == null)
                    {
                        firstNode = null;
                        return;
                    }
                } while (parent.lastChild() == current);

                for (int i = 1, n = parent.numChildren(); i < n; i++)
                {
                    if (parent.getChild(i - 1) == current)
                    {
                        firstNode = parent.getChild(i);
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
                    this.Current = firstNode;
                    return true;
                }
                else if (this.Current.isLeaf())
                {
                    this.Current = null;
                    return false;
                }
                else
                {
                    this.Current = this.Current.firstChild();
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

            public override bool satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                return Trees.leftEdge(t2, root) == Trees.rightEdge(t1, root);
            }

            public override IEnumerator<Tree> searchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return new ImmediatelyPrecedesSearchNodeIterator(t, matcher);
            }
        }

        private static readonly Relation IMMEDIATELY_PRECEDES = new ImmediatelyPrecedesRelation("."); /*{

    private static readonly long serialVersionUID = 3390147676937292768L;

    @Override
    bool satisfies(Tree t1, Tree t2, Tree root, /*readonly#1# TregexMatcher matcher) {
      return Trees.leftEdge(t2, root) == Trees.rightEdge(t1, root);
    }

    @Override
    Iterator<Tree> searchNodeIterator(/*readonly#1# Tree t,
                                      /*readonly#1# TregexMatcher matcher) {
      return new SearchNodeIterator() {
        @Override
        void initialize() {
          Tree current;
          Tree parent = t;
          do {
            current = parent;
            parent = matcher.getParent(parent);
            if (parent == null) {
              next = null;
              return;
            }
          } while (parent.lastChild() == current);

          for (int i = 1, n = parent.numChildren(); i < n; i++) {
            if (parent.getChild(i - 1) == current) {
              next = parent.getChild(i);
              return;
            }
          }
        }

        @Override
        public void advance() {
          if (next.isLeaf()) {
            next = null;
          } else {
            next = next.firstChild();
          }
        }
      };
    }
  };*/

        private class FollowsSearchNodeIterator : IEnumerator<Tree>
        {
            private Stack<Tree> searchStack;

            public FollowsSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                searchStack = new Stack<Tree>();
                Tree current = t;
                Tree parent = matcher.getParent(t);
                while (parent != null)
                {
                    for (int i = 0; parent.getChild(i) != current; i++)
                    {
                        searchStack.Push(parent.getChild(i));
                    }
                    current = parent;
                    parent = matcher.getParent(parent);
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
                    for (int i = this.Current.numChildren() - 1; i >= 0; i--)
                    {
                        searchStack.Push(this.Current.getChild(i));
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

            public override bool satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                return Trees.rightEdge(t2, root) <= Trees.leftEdge(t1, root);
            }

            public override IEnumerator<Tree> searchNodeIterator(Tree t, TregexMatcher matcher)
            {
                throw new NotImplementedException();
            }
        }

        private static readonly Relation FOLLOWS = new FollowsRelation(",,"); /*{

    private static readonly long serialVersionUID = -5948063114149496983L;

    @Override
    bool satisfies(Tree t1, Tree t2, Tree root, /*readonly#1# TregexMatcher matcher) {
      return Trees.rightEdge(t2, root) <= Trees.leftEdge(t1, root);
    }

    @Override
    Iterator<Tree> searchNodeIterator(/*readonly#1# Tree t,
                                      /*readonly#1# TregexMatcher matcher) {
      return new SearchNodeIterator() {
        Stack<Tree> searchStack;

        @Override
        public void initialize() {
          searchStack = new Stack<Tree>();
          Tree current = t;
          Tree parent = matcher.getParent(t);
          while (parent != null) {
            for (int i = 0; parent.getChild(i) != current; i++) {
              searchStack.push(parent.getChild(i));
            }
            current = parent;
            parent = matcher.getParent(parent);
          }
          advance();
        }

        @Override
        void advance() {
          if (searchStack.isEmpty()) {
            next = null;
          } else {
            next = searchStack.pop();
            for (int i = next.numChildren() - 1; i >= 0; i--) {
              searchStack.push(next.getChild(i));
            }
          }
        }
      };
    }
  };*/

        public class ImmediatelyFollowsSearchNodeIterator : IEnumerator<Tree>
        {
            private Tree firstNode;

            public ImmediatelyFollowsSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                Tree current;
                Tree parent = t;
                do
                {
                    current = parent;
                    parent = matcher.getParent(parent);
                    if (parent == null)
                    {
                        firstNode = null;
                        return;
                    }
                } while (parent.firstChild() == current);

                for (int i = 0, n = parent.numChildren() - 1; i < n; i++)
                {
                    if (parent.getChild(i + 1) == current)
                    {
                        firstNode = parent.getChild(i);
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
                    this.Current = firstNode;
                    return true;
                }
                if (this.Current.isLeaf())
                {
                    this.Current = null;
                    return false;
                }
                else
                {
                    this.Current = this.Current.lastChild();
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

            public override bool satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                return Trees.leftEdge(t1, root) == Trees.rightEdge(t2, root);
            }

            public override IEnumerator<Tree> searchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return new ImmediatelyFollowsSearchNodeIterator(t, matcher);
            }
        }

        private static readonly Relation IMMEDIATELY_FOLLOWS = new ImmediatelyFolllowsRelation(","); /*{

    private static readonly long serialVersionUID = -2895075562891296830L;

    @Override
    bool satisfies(Tree t1, Tree t2, Tree root, /*readonly#1# TregexMatcher matcher) {
      return Trees.leftEdge(t1, root) == Trees.rightEdge(t2, root);
    }

    @Override
    Iterator<Tree> searchNodeIterator(readonly Tree t,
                                      readonly TregexMatcher matcher) {
      return new SearchNodeIterator() {
        @Override
        void initialize() {
          Tree current;
          Tree parent = t;
          do {
            current = parent;
            parent = matcher.getParent(parent);
            if (parent == null) {
              next = null;
              return;
            }
          } while (parent.firstChild() == current);

          for (int i = 0, n = parent.numChildren() - 1; i < n; i++) {
            if (parent.getChild(i + 1) == current) {
              next = parent.getChild(i);
              return;
            }
          }
        }

        @Override
        public void advance() {
          if (next.isLeaf()) {
            next = null;
          } else {
            next = next.lastChild();
          }
        }
      };
    }
  };*/

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
                else if (this.Current.isLeaf())
                {
                    this.Current = null;
                    return false;
                }
                else
                {
                    this.Current = this.Current.firstChild();
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

            public override bool satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                if (t1.isLeaf())
                {
                    return false;
                }
                else
                {
                    return (t1.children()[0] == t2) || satisfies(t1.children()[0], t2, root, matcher);
                }
            }

            public override IEnumerator<Tree> searchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return new HasLeftmostDescendantSearchNodeIterator(t);
            }
        }

        private static readonly Relation HAS_LEFTMOST_DESCENDANT = new HasLeftmostDescendantRelation("<<,"); /*{

    private static readonly long serialVersionUID = -7352081789429366726L;

    @Override
    bool satisfies(Tree t1, Tree t2, Tree root, /*readonly#1# TregexMatcher matcher) {
      if (t1.isLeaf()) {
        return false;
      } else {
        return (t1.children()[0] == t2) || satisfies(t1.children()[0], t2, root, matcher);
      }
    }

    @Override
    Iterator<Tree> searchNodeIterator(/*readonly#1# Tree t,
                                      /*readonly#1# TregexMatcher matcher) {
      return new SearchNodeIterator() {
        @Override
        void initialize() {
          next = t;
          advance();
        }

        @Override
        public void advance() {
          if (next.isLeaf()) {
            next = null;
          } else {
            next = next.firstChild();
          }
        }
      };
    }
  };*/

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
                else if (this.Current.isLeaf())
                {
                    this.Current = null;
                    return false;
                }
                else
                {
                    this.Current = this.Current.lastChild();
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

            public override bool satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                if (t1.isLeaf())
                {
                    return false;
                }
                else
                {
                    Tree lastKid = t1.children()[t1.children().Length - 1];
                    return (lastKid == t2) || satisfies(lastKid, t2, root, matcher);
                }
            }

            public override IEnumerator<Tree> searchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return new HasRighmostDescendantSearchNodeIterator(t);
            }
        }

        private static readonly Relation HAS_RIGHTMOST_DESCENDANT = new HasRightmostDescendantRelation("<<-"); /*{

    private static readonly long serialVersionUID = -1405509785337859888L;

    @Override
    bool satisfies(Tree t1, Tree t2, Tree root, /*readonly#1# TregexMatcher matcher) {
      if (t1.isLeaf()) {
        return false;
      } else {
        Tree lastKid = t1.children()[t1.children().Length - 1];
        return (lastKid == t2) || satisfies(lastKid, t2, root, matcher);
      }
    }

    @Override
    Iterator<Tree> searchNodeIterator(/*readonly#1# Tree t,
                                      /*readonly#1# TregexMatcher matcher) {
      return new SearchNodeIterator() {
        @Override
        void initialize() {
          next = t;
          advance();
        }

        @Override
        public void advance() {
          if (next.isLeaf()) {
            next = null;
          } else {
            next = next.lastChild();
          }
        }
      };
    }
  };*/

        private class LeftmostDescendantOfSearchNodeIterator : IEnumerator<Tree>
        {
            private Tree next;
            private TregexMatcher matcher;

            public LeftmostDescendantOfSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                next = t;
                this.matcher = matcher;
            }

            public void Dispose()
            {
                // do nothing
            }

            public bool MoveNext()
            {
                if (next == null)
                {
                    return false;
                }
                else
                {
                    this.Current = next;
                    next = this.matcher.getParent(next);
                    if (next != null && next.firstChild() != this.Current)
                    {
                        next = null;
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

            public override bool satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                return HAS_LEFTMOST_DESCENDANT.satisfies(t2, t1, root, matcher);
            }

            public override IEnumerator<Tree> searchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return new LeftmostDescendantOfSearchNodeIterator(t, matcher);
            }
        }

        private static readonly Relation LEFTMOST_DESCENDANT_OF = new LeftmostDescendantOfRelation(">>,"); /*{

    private static readonly long serialVersionUID = 3103412865783190437L;

    @Override
    bool satisfies(Tree t1, Tree t2, Tree root, /*readonly#1# TregexMatcher matcher) {
      return HAS_LEFTMOST_DESCENDANT.satisfies(t2, t1, root, matcher);
    }

    @Override
    Iterator<Tree> searchNodeIterator(/*readonly#1# Tree t,
                                      /*readonly#1# TregexMatcher matcher) {
      return new SearchNodeIterator() {
        @Override
        void initialize() {
          next = t;
          advance();
        }

        @Override
        public void advance() {
          Tree last = next;
          next = matcher.getParent(next);
          if (next != null && next.firstChild() != last) {
            next = null;
          }
        }
      };
    }
  };*/

        private class RightmostDescendantOfSearchNodeIterator : IEnumerator<Tree>
        {
            private Tree next;
            private TregexMatcher matcher;

            public RightmostDescendantOfSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                next = t;
                this.matcher = matcher;
            }

            public void Dispose()
            {
                // do nothing
            }

            public bool MoveNext()
            {
                if (next == null)
                {
                    return false;
                }
                else
                {
                    this.Current = next;
                    next = this.matcher.getParent(next);
                    if (next != null && next.lastChild() != this.Current)
                    {
                        next = null;
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

            public override bool satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                return HAS_RIGHTMOST_DESCENDANT.satisfies(t2, t1, root, matcher);
            }

            public override IEnumerator<Tree> searchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return new RightmostDescendantOfSearchNodeIterator(t, matcher);
            }
        }

        private static readonly Relation RIGHTMOST_DESCENDANT_OF = new RightmostDescendantOfRelation(">>-"); /*{

    private static readonly long serialVersionUID = -2000255467314675477L;

    @Override
    bool satisfies(Tree t1, Tree t2, Tree root, readonly TregexMatcher matcher) {
      return HAS_RIGHTMOST_DESCENDANT.satisfies(t2, t1, root, matcher);
    }

    @Override
    Iterator<Tree> searchNodeIterator(/*readonly#1# Tree t,
                                      /*readonly#1# TregexMatcher matcher) {
      return new SearchNodeIterator() {
        @Override
        void initialize() {
          next = t;
          advance();
        }

        @Override
        public void advance() {
          Tree last = next;
          next = matcher.getParent(next);
          if (next != null && next.lastChild() != last) {
            next = null;
          }
        }
      };
    }
  };*/

        private class SisterOfSearchNodeIterator : IEnumerator<Tree>
        {
            private Tree parent;
            private int nextNum;
            private Tree originalNode;

            public SisterOfSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                originalNode = t;
                parent = matcher.getParent(t);
                if (parent != null)
                {
                    nextNum = 0;
                    //advance();
                }
            }

            public void Dispose()
            {
                // do nothing
            }

            public bool MoveNext()
            {
                if (nextNum < parent.numChildren())
                {
                    this.Current = parent.getChild(nextNum++);
                    if (this.Current == originalNode)
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

            public override bool satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                if (t1 == t2 || t1 == root)
                {
                    return false;
                }
                Tree parent = t1.parent(root);
                return PARENT_OF.satisfies(parent, t2, root, matcher);
            }

            public override IEnumerator<Tree> searchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return new SisterOfSearchNodeIterator(t, matcher);
            }
        }

        private static readonly Relation SISTER_OF = new SisterOfRelation("$"); /*{

    private static readonly long serialVersionUID = -3776688096782419004L;

    @Override
    bool satisfies(Tree t1, Tree t2, Tree root, /*readonly#1# TregexMatcher matcher) {
      if (t1 == t2 || t1 == root) {
        return false;
      }
      Tree parent = t1.parent(root);
      return PARENT_OF.satisfies(parent, t2, root, matcher);
    }

    @Override
    Iterator<Tree> searchNodeIterator(/*readonly#1# Tree t,
                                      /*readonly#1# TregexMatcher matcher) {
      return new SearchNodeIterator() {
        Tree parent;

        int nextNum;

        @Override
        void initialize() {
          parent = matcher.getParent(t);
          if (parent != null) {
            nextNum = 0;
            advance();
          }
        }

        @Override
        public void advance() {
          if (nextNum < parent.numChildren()) {
            next = parent.getChild(nextNum++);
            if (next == t) {
              advance();
            }
          } else {
            next = null;
          }
        }
      };
    }
  };*/

        private class LeftSisterOfSearchNodeIterator : IEnumerator<Tree>
        {
            private Tree parent;
            private int nextNum;
            private Tree originalNode;

            public LeftSisterOfSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                originalNode = t;
                parent = matcher.getParent(t);
                if (parent != null)
                {
                    nextNum = parent.numChildren() - 1;
                }
            }

            public void Dispose()
            {
                // do nothing
            }

            public bool MoveNext()
            {
                this.Current = parent.getChild(nextNum--);
                if (this.Current == originalNode)
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

            public override bool satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                if (t1 == t2 || t1 == root)
                {
                    return false;
                }
                Tree parent = t1.parent(root);
                Tree[] kids = parent.children();
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

            public override IEnumerator<Tree> searchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return new LeftSisterOfSearchNodeIterator(t, matcher);
            }
        }

        private static readonly Relation LEFT_SISTER_OF = new LeftSisterOfRelation("$++"); /*{

    private static readonly long serialVersionUID = -4516161080140406862L;

    @Override
    bool satisfies(Tree t1, Tree t2, Tree root, /*readonly#1# TregexMatcher matcher) {
      if (t1 == t2 || t1 == root) {
        return false;
      }
      Tree parent = t1.parent(root);
      Tree[] kids = parent.children();
      for (int i = kids.Length - 1; i > 0; i--) {
        if (kids[i] == t1) {
          return false;
        }
        if (kids[i] == t2) {
          return true;
        }
      }
      return false;
    }

    @Override
    Iterator<Tree> searchNodeIterator(/*readonly#1# Tree t,
                                      /*readonly#1# TregexMatcher matcher) {
      return new SearchNodeIterator() {
        Tree parent;

        int nextNum;

        @Override
        void initialize() {
          parent = matcher.getParent(t);
          if (parent != null) {
            nextNum = parent.numChildren() - 1;
            advance();
          }
        }

        @Override
        public void advance() {
          next = parent.getChild(nextNum--);
          if (next == t) {
            next = null;
          }
        }
      };
    }
  };*/

        private class RightSisterOfSearchNodeIterator : IEnumerator<Tree>
        {
            private Tree parent;
            private Tree originalNode;
            private int nextNum;

            public RightSisterOfSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                originalNode = t;
                parent = matcher.getParent(t);
                if (parent != null)
                {
                    nextNum = 0;
                }
            }

            public void Dispose()
            {
                // do nothing
            }

            public bool MoveNext()
            {
                this.Current = parent.getChild(nextNum++);
                if (this.Current == originalNode)
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

            public override bool satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                return LEFT_SISTER_OF.satisfies(t2, t1, root, matcher);
            }

            public override IEnumerator<Tree> searchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return new RightSisterOfSearchNodeIterator(t, matcher);
            }
        }

        private static readonly Relation RIGHT_SISTER_OF = new RightSisterOfRelation("$--"); /*{

    private static readonly long serialVersionUID = -5880626025192328694L;

    @Override
    bool satisfies(Tree t1, Tree t2, Tree root, /*readonly#1# TregexMatcher matcher) {
      return LEFT_SISTER_OF.satisfies(t2, t1, root, matcher);
    }

    @Override
    Iterator<Tree> searchNodeIterator(/*readonly#1# Tree t,
                                      /*readonly#1# TregexMatcher matcher) {
      return new SearchNodeIterator() {
        Tree parent;

        int nextNum;

        @Override
        void initialize() {
          parent = matcher.getParent(t);
          if (parent != null) {
            nextNum = 0;
            advance();
          }
        }

        @Override
        public void advance() {
          next = parent.getChild(nextNum++);
          if (next == t) {
            next = null;
          }
        }
      };
    }
  };*/

        private class ImmediateLeftSisterOfRelation : Relation
        {
            public ImmediateLeftSisterOfRelation(string symbol) : base(symbol)
            {
            }

            public override bool satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                if (t1 == t2 || t1 == root)
                {
                    return false;
                }
                Tree[] sisters = t1.parent(root).children();
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

            public override IEnumerator<Tree> searchNodeIterator(Tree t, TregexMatcher matcher)
            {
                if (t != matcher.getRoot())
                {
                    Tree parent = matcher.getParent(t);
                    int i = 0;
                    while (parent.getChild(i) != t)
                    {
                        i++;
                    }
                    if (i + 1 < parent.numChildren())
                    {
                        var node = parent.getChild(i + 1);
                        return new List<Tree>() {node}.GetEnumerator();
                    }
                }
                return new List<Tree>().GetEnumerator();
            }
        }

        private static readonly Relation IMMEDIATE_LEFT_SISTER_OF = new ImmediateLeftSisterOfRelation("$+"); /*{

    private static readonly long serialVersionUID = 7745237994722126917L;

    @Override
    bool satisfies(Tree t1, Tree t2, Tree root, /*readonly#1# TregexMatcher matcher) {
      if (t1 == t2 || t1 == root) {
        return false;
      }
      Tree[] sisters = t1.parent(root).children();
      for (int i = sisters.Length - 1; i > 0; i--) {
        if (sisters[i] == t1) {
          return false;
        }
        if (sisters[i] == t2) {
          return sisters[i - 1] == t1;
        }
      }
      return false;
    }

    @Override
    Iterator<Tree> searchNodeIterator(/*readonly#1# Tree t,
                                      /*readonly#1# TregexMatcher matcher) {
      return new SearchNodeIterator() {
        @Override
        void initialize() {
          if (t != matcher.getRoot()) {
            Tree parent = matcher.getParent(t);
            int i = 0;
            while (parent.getChild(i) != t) {
              i++;
            }
            if (i + 1 < parent.numChildren()) {
              next = parent.getChild(i + 1);
            }
          }
        }
      };
    }
  };*/

        private class ImmediateRightSisterOfRelation : Relation
        {
            public ImmediateRightSisterOfRelation(string symbol) : base(symbol)
            {
            }

            public override bool satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                return IMMEDIATE_LEFT_SISTER_OF.satisfies(t2, t1, root, matcher);
            }

            public override IEnumerator<Tree> searchNodeIterator(Tree t, TregexMatcher matcher)
            {
                if (t != matcher.getRoot())
                {
                    Tree parent = matcher.getParent(t);
                    int i = 0;
                    while (parent.getChild(i) != t)
                    {
                        i++;
                    }
                    if (i > 0)
                    {
                        var node = parent.getChild(i - 1);
                        return new List<Tree>() {node}.GetEnumerator();
                    }
                }

                return new List<Tree>().GetEnumerator();
            }
        }

        private static readonly Relation IMMEDIATE_RIGHT_SISTER_OF = new ImmediateRightSisterOfRelation("$-"); /*{

    private static readonly long serialVersionUID = -6555264189937531019L;

    @Override
    bool satisfies(Tree t1, Tree t2, Tree root, /*readonly#1# TregexMatcher matcher) {
      return IMMEDIATE_LEFT_SISTER_OF.satisfies(t2, t1, root, matcher);
    }

    @Override
    Iterator<Tree> searchNodeIterator(/*readonly#1# Tree t,
                                      /*readonly#1# TregexMatcher matcher) {
      return new SearchNodeIterator() {
        @Override
        void initialize() {
          if (t != matcher.getRoot()) {
            Tree parent = matcher.getParent(t);
            int i = 0;
            while (parent.getChild(i) != t) {
              i++;
            }
            if (i > 0) {
              next = parent.getChild(i - 1);
            }
          }
        }
      };
    }
  };*/

        private class OnlyChildOfRelation : Relation
        {
            public OnlyChildOfRelation(string symbol) : base(symbol)
            {
            }

            public override bool satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                return t2.children().Length == 1 && t2.firstChild() == t1;
            }

            public override IEnumerator<Tree> searchNodeIterator(Tree t, TregexMatcher matcher)
            {
                if (t != matcher.getRoot())
                {
                    var next = matcher.getParent(t);
                    if (next.numChildren() == 1)
                    {
                        return new List<Tree>() {next}.GetEnumerator();
                    }
                }
                return new List<Tree>().GetEnumerator();
            }
        }

        private static readonly Relation ONLY_CHILD_OF = new OnlyChildOfRelation(">:"); /*{

    private static readonly long serialVersionUID = 1719812660770087879L;

    @Override
    bool satisfies(Tree t1, Tree t2, Tree root, /*readonly#1# TregexMatcher matcher) {
      return t2.children().Length == 1 && t2.firstChild() == t1;
    }

    @Override
    Iterator<Tree> searchNodeIterator(/*readonly#1# Tree t,
                                      /*readonly#1# TregexMatcher matcher) {
      return new SearchNodeIterator() {
        @Override
        void initialize() {
          if (t != matcher.getRoot()) {
            next = matcher.getParent(t);
            if (next.numChildren() != 1) {
              next = null;
            }
          }
        }
      };
    }
  };*/

        private class HasOnlyChildRelation : Relation
        {
            public HasOnlyChildRelation(string symbol) : base(symbol)
            {
            }

            public override bool satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                return t1.children().Length == 1 && t1.firstChild() == t2;
            }

            public override IEnumerator<Tree> searchNodeIterator(Tree t, TregexMatcher matcher)
            {
                if (!t.isLeaf() && t.numChildren() == 1)
                {
                    var next = t.firstChild();
                    return new List<Tree>() {next}.GetEnumerator();
                }
                else
                {
                    return new List<Tree>().GetEnumerator();
                }
            }
        }

        private static readonly Relation HAS_ONLY_CHILD = new HasOnlyChildRelation("<:"); /*{

    private static readonly long serialVersionUID = -8776487500849294279L;

    @Override
    bool satisfies(Tree t1, Tree t2, Tree root, /*readonly#1# TregexMatcher matcher) {
      return t1.children().Length == 1 && t1.firstChild() == t2;
    }

    @Override
    Iterator<Tree> searchNodeIterator(/*readonly#1# Tree t,
                                      /*readonly#1# TregexMatcher matcher) {
      return new SearchNodeIterator() {
        @Override
        void initialize() {
          if (!t.isLeaf() && t.numChildren() == 1) {
            next = t.firstChild();
          }
        }
      };
    }
  };*/

        private class UnaryPathAncestorSeachNodeIterator : IEnumerator<Tree>
        {
            private Stack<Tree> searchStack;

            public UnaryPathAncestorSeachNodeIterator(Tree t)
            {
                searchStack = new Stack<Tree>();
                if (!t.isLeaf() && t.children().Length == 1)
                    searchStack.Push(t.getChild(0));
                /*if (searchStack.isEmpty()) {
            advance();
          }*/
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
                    if (!this.Current.isLeaf() && this.Current.children().Length == 1)
                    {
                        searchStack.Push(this.Current.getChild(0));
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

            public override bool satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                if (t1.isLeaf() || t1.children().Length > 1)
                    return false;
                Tree onlyDtr = t1.children()[0];
                if (onlyDtr == t2)
                    return true;
                else
                    return satisfies(onlyDtr, t2, root, matcher);
            }

            public override IEnumerator<Tree> searchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return new UnaryPathAncestorSeachNodeIterator(t);
            }
        }

        private static readonly Relation UNARY_PATH_ANCESTOR_OF = new UnaryPathAncestorOfRelation("<<:"); /* {

    private static readonly long serialVersionUID = -742912038636163403L;

    @Override
    bool satisfies(Tree t1, Tree t2, Tree root, /*readonly#1# TregexMatcher matcher) {
      if (t1.isLeaf() || t1.children().Length > 1)
        return false;
      Tree onlyDtr = t1.children()[0];
      if (onlyDtr == t2)
        return true;
      else
        return satisfies(onlyDtr, t2, root, matcher);
    }

    @Override
    Iterator<Tree> searchNodeIterator(/*readonly#1# Tree t,
                                      /*readonly#1# TregexMatcher matcher) {
      return new SearchNodeIterator() {
        Stack<Tree> searchStack;

        @Override
        public void initialize() {
          searchStack = new Stack<Tree>();
          if (!t.isLeaf() && t.children().Length == 1)
            searchStack.push(t.getChild(0));
          if (!searchStack.isEmpty()) {
            advance();
          }
        }

        @Override
        void advance() {
          if (searchStack.isEmpty()) {
            next = null;
          } else {
            next = searchStack.pop();
            if (!next.isLeaf() && next.children().Length == 1)
              searchStack.push(next.getChild(0));
          }
        }
      };
    }
  };*/

        private class UnaryPathDescendantSearchNodeIterator : IEnumerator<Tree>
        {
            private Stack<Tree> searchStack;
            private TregexMatcher matcher;

            public UnaryPathDescendantSearchNodeIterator(Tree t, TregexMatcher matcher)
            {
                this.matcher = matcher;
                searchStack = new Stack<Tree>();
                Tree parent = matcher.getParent(t);
                if (parent != null && !parent.isLeaf() && parent.children().Length == 1)
                {
                    searchStack.Push(parent);
                }
                /*if (!searchStack.isEmpty()) {
                    advance();
                  }*/
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
                    Tree parent = matcher.getParent(this.Current);
                    if (parent != null && !parent.isLeaf() && parent.children().Length == 1)
                    {
                        searchStack.Push(parent);
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

            public override bool satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                if (t2.isLeaf() || t2.children().Length > 1)
                {
                    return false;
                }
                Tree onlyDtr = t2.children()[0];
                if (onlyDtr == t1)
                {
                    return true;
                }
                else
                {
                    return satisfies(t1, onlyDtr, root, matcher);
                }
            }

            public override IEnumerator<Tree> searchNodeIterator(Tree t, TregexMatcher matcher)
            {
                return new UnaryPathDescendantSearchNodeIterator(t, matcher);
            }
        }

        private static readonly Relation UNARY_PATH_DESCENDANT_OF = new UnaryPathDescendantOfRelation(">>:"); /*{

    private static readonly long serialVersionUID = 4364021807752979404L;

    @Override
    bool satisfies(Tree t1, Tree t2, Tree root, /*readonly#1# TregexMatcher matcher) {
      if (t2.isLeaf() || t2.children().Length > 1)
        return false;
      Tree onlyDtr = t2.children()[0];
      if (onlyDtr == t1)
        return true;
      else
        return satisfies(t1, onlyDtr, root, matcher);
    }

    @Override
    Iterator<Tree> searchNodeIterator(/*readonly#1# Tree t,
                                      /*readonly#1# TregexMatcher matcher) {
      return new SearchNodeIterator() {
        Stack<Tree> searchStack;

        @Override
        public void initialize() {
          searchStack = new Stack<Tree>();
          Tree parent = matcher.getParent(t);
          if (parent != null && !parent.isLeaf() &&
              parent.children().Length == 1)
            searchStack.push(parent);
          if (!searchStack.isEmpty()) {
            advance();
          }
        }

        @Override
        void advance() {
          if (searchStack.isEmpty()) {
            next = null;
          } else {
            next = searchStack.pop();
            Tree parent = matcher.getParent(next);
            if (parent != null && !parent.isLeaf() &&
                parent.children().Length == 1)
              searchStack.push(parent);
          }
        }
      };
    }
  };
*/

        private class ParentEqualsRelation : Relation
        {
            public ParentEqualsRelation(string symbol) : base(symbol)
            {
            }

            public override bool satisfies(Tree t1, Tree t2, Tree root, TregexMatcher matcher)
            {
                if (t1 == t2)
                {
                    return true;
                }
                return PARENT_OF.satisfies(t1, t2, root, matcher);
            }

            public override IEnumerator<Tree> searchNodeIterator(Tree t, TregexMatcher matcher)
            {
                // TODO: check this implementation, sounds strange to me
                return t.getChildrenAsList().GetEnumerator();
            }
        }

        private static readonly Relation PARENT_EQUALS = new ParentEqualsRelation("<="); /*{
    private static readonly long serialVersionUID = 98745298745198245L;

    @Override
    bool satisfies(Tree t1, Tree t2, Tree root, /*readonly#1# TregexMatcher matcher) {
      if (t1 == t2) {
        return true;
      }
      return PARENT_OF.satisfies(t1, t2, root, matcher);
    }

    @Override
    Iterator<Tree> searchNodeIterator(/*readonly#1# Tree t,
                                      /*readonly#1# TregexMatcher matcher) {
      return new SearchNodeIterator() {
        int nextNum;
        bool usedParent;

        @Override
        public void advance() {
          if (!usedParent) {
            next = t;
            usedParent = true;
          } else {
            if (nextNum < t.numChildren()) {
              next = t.getChild(nextNum);
              nextNum++;
            } else {
              next = null;
            }
          }
        }
      };
    }
  };*/

        private static readonly Relation[] SIMPLE_RELATIONS =
        {
            DOMINATES, DOMINATED_BY, PARENT_OF, CHILD_OF, PRECEDES,
            IMMEDIATELY_PRECEDES, FOLLOWS, IMMEDIATELY_FOLLOWS,
            HAS_LEFTMOST_DESCENDANT, HAS_RIGHTMOST_DESCENDANT,
            LEFTMOST_DESCENDANT_OF, RIGHTMOST_DESCENDANT_OF, SISTER_OF,
            LEFT_SISTER_OF, RIGHT_SISTER_OF, IMMEDIATE_LEFT_SISTER_OF,
            IMMEDIATE_RIGHT_SISTER_OF, ONLY_CHILD_OF, HAS_ONLY_CHILD, EQUALS,
            PATTERN_SPLITTER, UNARY_PATH_ANCESTOR_OF, UNARY_PATH_DESCENDANT_OF,
            PARENT_EQUALS
        };

        private static readonly Dictionary<string, Relation> ADDITIONAL_RELATION_MAP = new Dictionary<string, Relation>()
        {
            {"<<`", HAS_RIGHTMOST_DESCENDANT},
            {"<<,", HAS_LEFTMOST_DESCENDANT},
            {">>`", RIGHTMOST_DESCENDANT_OF},
            {">>,", LEFTMOST_DESCENDANT_OF},
            {"$..", LEFT_SISTER_OF},
            {"$,,", RIGHT_SISTER_OF},
            {"$.", IMMEDIATE_LEFT_SISTER_OF},
            {"$,", IMMEDIATE_RIGHT_SISTER_OF}
        };

        private static readonly Dictionary<string, Relation> SIMPLE_RELATIONS_MAP = SIMPLE_RELATIONS
            .ToDictionary(r => r.symbol, r => r)
            .Union(ADDITIONAL_RELATION_MAP)
            .ToDictionary(ent => ent.Key, ent => ent.Value);

        /*static {
    for (Relation r : SIMPLE_RELATIONS) {
      SIMPLE_RELATIONS_MAP.put(r.symbol, r);
    }
    SIMPLE_RELATIONS_MAP.put("<<`", HAS_RIGHTMOST_DESCENDANT);
    SIMPLE_RELATIONS_MAP.put("<<,", HAS_LEFTMOST_DESCENDANT);
    SIMPLE_RELATIONS_MAP.put(">>`", RIGHTMOST_DESCENDANT_OF);
    SIMPLE_RELATIONS_MAP.put(">>,", LEFTMOST_DESCENDANT_OF);
    SIMPLE_RELATIONS_MAP.put("$..", LEFT_SISTER_OF);
    SIMPLE_RELATIONS_MAP.put("$,,", RIGHT_SISTER_OF);
    SIMPLE_RELATIONS_MAP.put("$.", IMMEDIATE_LEFT_SISTER_OF);
    SIMPLE_RELATIONS_MAP.put("$,", IMMEDIATE_RIGHT_SISTER_OF);
  }*/

        //@Override
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

            /*readonly*/
            Relation relation = (Relation) o;

            return symbol.Equals(relation.symbol);
        }

        /*@Override*/

        public override int GetHashCode()
        {
            return symbol.GetHashCode();
        }


        private /*static*/ class Heads : Relation
        {

            private new static readonly long serialVersionUID = 4681433462932265831L;

            public readonly HeadFinder hf;

            public Heads(HeadFinder hf) : base(">>#")
            {
                this.hf = hf;
            }

            //@Override
            public override bool satisfies(Tree t1, Tree t2, Tree root, /*readonly*/ TregexMatcher matcher)
            {
                if (t2.isLeaf())
                {
                    return false;
                }
                else if (t2.isPreTerminal())
                {
                    return (t2.firstChild() == t1);
                }
                else
                {
                    HeadFinder headFinder = matcher.getHeadFinder();
                    if (headFinder == null) headFinder = this.hf;
                    Tree head = headFinder.determineHead(t2);
                    if (head == t1)
                    {
                        return true;
                    }
                    else
                    {
                        return satisfies(t1, head, root, matcher);
                    }
                }
            }

            /*@Override*/

            public override IEnumerator<Tree> searchNodeIterator( /*readonly*/ Tree t,
                /*readonly */TregexMatcher matcher)
            {
                var next = t;

                HeadFinder headFinder = matcher.getHeadFinder();
                if (headFinder == null)
                {
                    headFinder = this.hf;
                }

                Tree last = next;
                next = matcher.getParent(next);
                if (next != null && headFinder.determineHead(next) != last)
                {
                    next = null;
                    return new List<Tree>().GetEnumerator();
                }
                else
                {
                    return new List<Tree>() {next}.GetEnumerator();
                }
                /*return new SearchNodeIterator() {
        @Override
        void initialize() {
          next = t;
          advance();
        }

        @Override
        public void advance() {
          HeadFinder headFinder = matcher.getHeadFinder();
          if (headFinder == null) headFinder = hf;

          Tree last = next;
          next = matcher.getParent(next);
          if (next != null && headFinder.determineHead(next) != last) {
            next = null;
          }
        }
      };*/
            }

            //@Override
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

                /*readonly*/
                Heads heads = (Heads) o;

                if (hf != null ? !hf.Equals(heads.hf) : heads.hf != null)
                {
                    return false;
                }

                return true;
            }

            /*@Override*/

            public override int GetHashCode()
            {
                int result = base.GetHashCode();
                result = 29*result + (hf != null ? hf.GetHashCode() : 0);
                return result;
            }
        }

        private class HeadedByIterator : IEnumerator<Tree>
        {
            private Tree initialNode;
            private HeadFinder hf;
            private bool initialized;

            public HeadedByIterator(Tree t, TregexMatcher matcher, HeadFinder hf)
            {
                this.initialNode = t;
                this.hf = hf;
                this.initialized = false;
            }

            public void Dispose()
            {
                // do nothing
            }

            public bool MoveNext()
            {
                if (!initialized)
                {
                    this.initialized = true;
                    if (this.initialNode.isLeaf())
                    {
                        this.Current = null;
                        return false;
                    }
                    else
                    {
                        this.Current = this.hf.determineHead(this.initialNode);
                        return true;
                    }
                }
                else
                {
                    if (this.Current.isLeaf())
                    {
                        this.Current = null;
                        return false;
                    }
                    else
                    {
                        this.Current = this.hf.determineHead(this.Current);
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

        private /*static*/ class HeadedBy : Relation
        {

            private new static readonly long serialVersionUID = 2825997185749055693L;

            private readonly Heads heads;

            public HeadedBy(HeadFinder hf) : base("<<#")
            {
                this.heads = Interner<Heads>.GlobalIntern(new Heads(hf));
            }

            /*@Override*/

            public override bool satisfies(Tree t1, Tree t2, Tree root, /*readonly*/ TregexMatcher matcher)
            {
                return heads.satisfies(t2, t1, root, matcher);
            }

            /*@Override*/

            public override IEnumerator<Tree> searchNodeIterator( /*readonly*/ Tree t,
                /*readonly */TregexMatcher matcher)
            {
                var finder = matcher.getHeadFinder() != null ? matcher.getHeadFinder() : this.heads.hf;
                return new HeadedByIterator(t, matcher, finder);
                /*return new SearchNodeIterator() {
        @Override
        void initialize() {
          next = t;
          advance();
        }

        @Override
        public void advance() {
          if (next.isLeaf()) {
            next = null;
          } else {
            if (matcher.getHeadFinder() != null) {
              next = matcher.getHeadFinder().determineHead(next);
            } else {
              next = heads.hf.determineHead(next);
            }
          }
        }
      };*/
            }

            /*@Override*/

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

                /*readonly */
                HeadedBy headedBy = (HeadedBy) o;

                if (heads != null
                    ? !heads.Equals(headedBy.heads)
                    : headedBy.heads != null)
                {
                    return false;
                }

                return true;
            }

            /*@Override*/

            public override int GetHashCode()
            {
                int result = base.GetHashCode();
                result = 29*result + (heads != null ? heads.GetHashCode() : 0);
                return result;
            }
        }


        private /*static*/ class ImmediatelyHeads : Relation
        {


            private new static readonly long serialVersionUID = 2085410152913894987L;

            public readonly HeadFinder hf;

            public ImmediatelyHeads(HeadFinder hf) : base(">#")
            {
                this.hf = hf;
            }

            /*@Override*/

            public override bool satisfies(Tree t1, Tree t2, Tree root, /*readonly */TregexMatcher matcher)
            {
                if (matcher.getHeadFinder() != null)
                {
                    return matcher.getHeadFinder().determineHead(t2) == t1;
                }
                else
                {
                    return hf.determineHead(t2) == t1;
                }
            }

            /*@Override*/

            public override IEnumerator<Tree> searchNodeIterator( /*readonly*/ Tree t,
                /*readonly*/ TregexMatcher matcher)
            {
                if (t != matcher.getRoot())
                {
                    var next = matcher.getParent(t);
                    HeadFinder headFinder = matcher.getHeadFinder() == null ? hf : matcher.getHeadFinder();
                    if (headFinder.determineHead(next) == t)
                    {
                        return new List<Tree>() {next}.GetEnumerator();
                    }
                }
                return new List<Tree>().GetEnumerator();
                /*return new SearchNodeIterator() {
        @Override
        void initialize() {
          if (t != matcher.getRoot()) {
            next = matcher.getParent(t);
            HeadFinder headFinder = matcher.getHeadFinder() == null ? hf : matcher.getHeadFinder();
            if (headFinder.determineHead(next) != t) {
              next = null;
            }
          }
        }
      };*/
            }

            /*@Override*/

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

                /*readonly*/
                ImmediatelyHeads immediatelyHeads = (ImmediatelyHeads) o;

                if (hf != null ? !hf.Equals(immediatelyHeads.hf) : immediatelyHeads.hf != null)
                {
                    return false;
                }

                return true;
            }

            /*@Override*/

            public override int GetHashCode()
            {
                int result = base.GetHashCode();
                result = 29*result + (hf != null ? hf.GetHashCode() : 0);
                return result;
            }
        }


        private /*static*/ class ImmediatelyHeadedBy : Relation
        {

            private new static readonly long serialVersionUID = 5910075663419780905L;

            private readonly ImmediatelyHeads immediatelyHeads;

            public ImmediatelyHeadedBy(HeadFinder hf) : base("<#")
            {
                this.immediatelyHeads = Interner<ImmediatelyHeads>
                    .GlobalIntern(new ImmediatelyHeads(hf));
            }

            /*@Override*/

            public override bool satisfies(Tree t1, Tree t2, Tree root, /*readonly */TregexMatcher matcher)
            {
                return immediatelyHeads.satisfies(t2, t1, root, matcher);
            }

            /*@Override*/

            public override IEnumerator<Tree> searchNodeIterator( /*readonly*/ Tree t,
                /*readonly */TregexMatcher matcher)
            {
                if (!t.isLeaf())
                {
                    var headFinder = matcher.getHeadFinder() != null ? matcher.getHeadFinder() : immediatelyHeads.hf;
                    var next = headFinder.determineHead(t);
                    return new List<Tree>() {next}.GetEnumerator();
                }
                return new List<Tree>().GetEnumerator();
                /*return new SearchNodeIterator() {
        @Override
        void initialize() {
          if (!t.isLeaf()) {
            if (matcher.getHeadFinder() != null) {
              next = matcher.getHeadFinder().determineHead(t);
            } else {
              next = immediatelyHeads.hf.determineHead(t);
            }
          }
        }
      };*/
            }

            /*@Override*/

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

                /*readonly */
                ImmediatelyHeadedBy immediatelyHeadedBy = (ImmediatelyHeadedBy) o;

                if (immediatelyHeads != null
                    ? !immediatelyHeads
                        .Equals(immediatelyHeadedBy.immediatelyHeads)
                    : immediatelyHeadedBy.immediatelyHeads != null)
                {
                    return false;
                }

                return true;
            }

            /*@Override*/

            public override int GetHashCode()
            {
                int result = base.GetHashCode();
                result = 29*result
                         + (immediatelyHeads != null ? immediatelyHeads.GetHashCode() : 0);
                return result;
            }
        }


        private /*static */ class IthChildOf : Relation
        {

            private new static readonly long serialVersionUID = -1463126827537879633L;

            public readonly int childNum;

            public IthChildOf(int i) : base(">" + i)
            {
                if (i == 0)
                {
                    throw new ArgumentException(
                        "Error -- no such thing as zeroth child!");
                }
                else
                {
                    childNum = i;
                }
            }

            /*@Override*/

            public override bool satisfies(Tree t1, Tree t2, Tree root, /*readonly */TregexMatcher matcher)
            {
                Tree[] kids = t2.children();
                if (kids.Length < Math.Abs(childNum))
                {
                    return false;
                }
                if (childNum > 0 && kids[childNum - 1] == t1)
                {
                    return true;
                }
                if (childNum < 0 && kids[kids.Length + childNum] == t1)
                {
                    return true;
                }
                return false;
            }

            /*@Override*/

            public override IEnumerator<Tree> searchNodeIterator( /*readonly */ Tree t,
                /*readonly */TregexMatcher matcher)
            {
                if (t != matcher.getRoot())
                {
                    var next = matcher.getParent(t);
                    if (childNum > 0
                        && (next.numChildren() < childNum || next
                            .getChild(childNum - 1) != t)
                        || childNum < 0
                        && (next.numChildren() < -childNum || next.getChild(next
                            .numChildren()
                                                                            + childNum) != t))
                    {
                        next = null;
                    }
                    if (next != null)
                    {
                        return new List<Tree>() {next}.GetEnumerator();
                    }
                }
                return new List<Tree>().GetEnumerator();
                /*return new SearchNodeIterator() {
        @Override
        void initialize() {
          if (t != matcher.getRoot()) {
            next = matcher.getParent(t);
            if (childNum > 0
                && (next.numChildren() < childNum || next
                    .getChild(childNum - 1) != t)
                || childNum < 0
                && (next.numChildren() < -childNum || next.getChild(next
                    .numChildren()
                    + childNum) != t)) {
              next = null;
            }
          }
        }
      };*/
            }

            /*@Override*/

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

                /*readonly */
                IthChildOf ithChildOf = (IthChildOf) o;

                if (childNum != ithChildOf.childNum)
                {
                    return false;
                }

                return true;
            }

            /*@Override*/

            public override int GetHashCode()
            {
                return childNum;
            }

        }


        private /*static */ class HasIthChild : Relation
        {

            private new static readonly long serialVersionUID = 3546853729291582806L;

            private readonly IthChildOf ithChildOf;

            public HasIthChild(int i) : base("<" + i)
            {
                ithChildOf = Interner<IthChildOf>.GlobalIntern(new IthChildOf(i));
            }

            /*@Override*/

            public override bool satisfies(Tree t1, Tree t2, Tree root, /*readonly */TregexMatcher matcher)
            {
                return ithChildOf.satisfies(t2, t1, root, matcher);
            }

            /*@Override*/

            public override IEnumerator<Tree> searchNodeIterator( /*readonly */ Tree t,
                /*readonly */TregexMatcher matcher)
            {
                int childNum = ithChildOf.childNum;
                if (t.numChildren() >= Math.Abs(childNum))
                {
                    Tree next;
                    if (childNum > 0)
                    {
                        next = t.getChild(childNum - 1);
                    }
                    else
                    {
                        next = t.getChild(t.numChildren() + childNum);
                    }
                    return new List<Tree>() {next}.GetEnumerator();
                }
                return new List<Tree>().GetEnumerator();
                /*return new SearchNodeIterator() {
        @Override
        void initialize() {
          int childNum = ithChildOf.childNum;
          if (t.numChildren() >= Math.abs(childNum)) {
            if (childNum > 0) {
              next = t.getChild(childNum - 1);
            } else {
              next = t.getChild(t.numChildren() + childNum);
            }
          }
        }
      };*/
            }

            /*@Override*/

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

                /*readonly */
                HasIthChild hasIthChild = (HasIthChild) o;

                if (ithChildOf != null
                    ? !ithChildOf.Equals(hasIthChild.ithChildOf)
                    : hasIthChild.ithChildOf != null)
                {
                    return false;
                }

                return true;
            }

            /*@Override*/

            public override int GetHashCode()
            {
                int result = base.GetHashCode();
                result = 29*result + (ithChildOf != null ? ithChildOf.GetHashCode() : 0);
                return result;
            }
        }

        private class UnbrokenCategoryDominatesIterator : IEnumerator<Tree>
        {
            private Stack<Tree> searchStack;
            private Func<Tree, bool> patchMatchesNode;

            public UnbrokenCategoryDominatesIterator(Tree t, Func<Tree, bool> patchMatchesNode)
            {
                searchStack = new Stack<Tree>();
                for (int i = t.numChildren() - 1; i >= 0; i--)
                {
                    searchStack.Push(t.getChild(i));
                }
                /*if (!searchStack.isEmpty()) {
            advance();
          }*/
                this.patchMatchesNode = patchMatchesNode;
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
                    if (this.patchMatchesNode(this.Current))
                    {
                        for (int i = this.Current.numChildren() - 1; i >= 0; i--)
                        {
                            searchStack.Push(this.Current.getChild(i));
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


        private /*static */ class UnbrokenCategoryDominates : Relation
        {

            private new static readonly long serialVersionUID = -4174923168221859262L;

            private readonly Regex pattern;
            private readonly bool negatedPattern;
            private readonly bool basicCat;
            private Func<string, string> basicCatFunction;


            /**
     *
     * @param arg This may have a ! and then maybe a @ and then either an
     *            identifier or regex
     */

            public UnbrokenCategoryDominates(string arg,
                Func<string, string> basicCatFunction) : base("<+(" + arg + ')')
            {
                if (arg.StartsWith("!"))
                {
                    negatedPattern = true;
                    arg = arg.Substring(1);
                }
                else
                {
                    negatedPattern = false;
                }
                if (arg.StartsWith("@"))
                {
                    basicCat = true;
                    this.basicCatFunction = basicCatFunction;
                    arg = arg.Substring(1);
                }
                else
                {
                    basicCat = false;
                }
                if (Regex.IsMatch(arg, "/.*/"))
                {
                    //pattern = new Regex(arg.Substring(1, arg.Length - 1));
                    pattern = new Regex(arg.Substring(1, arg.Length - 2));
                }
                else if (Regex.IsMatch(arg, "__"))
                {
                    pattern = new Regex("^.*$");
                }
                else
                {
                    pattern = new Regex("^(?:" + arg + ")$");
                }
            }

            /** {@inheritDoc} */
            /*@Override*/

            public override bool satisfies(Tree t1, Tree t2, Tree root, /*readonly */TregexMatcher matcher)
            {
                foreach (Tree kid in t1.children())
                {
                    if (kid == t2)
                    {
                        return true;
                    }
                    else
                    {
                        if (pathMatchesNode(kid) && satisfies(kid, t2, root, matcher))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            public bool pathMatchesNode(Tree node)
            {
                string lab = node.value();
                // added this code to not crash if null node, even though there probably should be null nodes in the tree
                if (lab == null)
                {
                    // Say that a null label matches no positive pattern, but any negated patern
                    return negatedPattern;
                }
                else
                {
                    if (basicCat)
                    {
                        lab = basicCatFunction(lab);
                    }
                    var m = pattern.Match(lab);
                    return m.Success != negatedPattern;
                }
            }

            /** {@inheritDoc} */
            /*@Override*/

            public override IEnumerator<Tree> searchNodeIterator( /*readonly */ Tree t,
                /*readonly */TregexMatcher matcher)
            {
                return new UnbrokenCategoryDominatesIterator(t, this.pathMatchesNode);
                /*return new SearchNodeIterator() {
        Stack<Tree> searchStack;

        @Override
        public void initialize() {
          searchStack = new Stack<Tree>();
          for (int i = t.numChildren() - 1; i >= 0; i--) {
            searchStack.push(t.getChild(i));
          }
          if (!searchStack.isEmpty()) {
            advance();
          }
        }

        @Override
        void advance() {
          if (searchStack.isEmpty()) {
            next = null;
          } else {
            next = searchStack.pop();
            if (pathMatchesNode(next)) {
              for (int i = next.numChildren() - 1; i >= 0; i--) {
                searchStack.push(next.getChild(i));
              }
            }
          }
        }
      };*/
            }

            /*@Override*/

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

                /*readonly*/
                UnbrokenCategoryDominates unbrokenCategoryDominates = (UnbrokenCategoryDominates) o;

                if (negatedPattern != unbrokenCategoryDominates.negatedPattern)
                {
                    return false;
                }
                if (!pattern.Equals(unbrokenCategoryDominates.pattern))
                {
                    return false;
                }

                return true;
            }

            /*@Override*/

            public override int GetHashCode()
            {
                int result;
                result = pattern.GetHashCode();
                result = 29*result + (negatedPattern ? 1 : 0);
                return result;
            }

        } // end class UnbrokenCategoryDominates


        private /*static*/ class UnbrokenCategoryIsDominatedBy : Relation
        {

            private new static readonly long serialVersionUID = 2867922828235355129L;

            private readonly UnbrokenCategoryDominates unbrokenCategoryDominates;

            public UnbrokenCategoryIsDominatedBy(string arg,
                Func<string, string> basicCatFunction) : base(">+(" + arg + ')')
            {
                unbrokenCategoryDominates = Interner<UnbrokenCategoryDominates>
                    .GlobalIntern((new UnbrokenCategoryDominates(arg, basicCatFunction)));
            }

            /** {@inheritDoc} */
            /*@Override*/

            public override bool satisfies(Tree t1, Tree t2, Tree root, /*readonly */TregexMatcher matcher)
            {
                return unbrokenCategoryDominates.satisfies(t2, t1, root, matcher);
            }

            /** {@inheritDoc} */
            /*@Override*/

            public override IEnumerator<Tree> searchNodeIterator( /*readonly */ Tree t,
                /*readonly */TregexMatcher matcher)
            {
                var nodes = new List<Tree>();
                var node = t;
                while (unbrokenCategoryDominates.pathMatchesNode(node))
                {
                    node = matcher.getParent(node);
                    nodes.Add(node);
                }
                return nodes.GetEnumerator();
                /*return new SearchNodeIterator() {
        @Override
        void initialize() {
          next = matcher.getParent(t);
        }

        @Override
        public void advance() {
          if (unbrokenCategoryDominates.pathMatchesNode(next)) {
            next = matcher.getParent(next);
          } else {
            next = null;
          }
        }
      };*/
            }

            /*@Override*/

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

                /*readonly */
                UnbrokenCategoryIsDominatedBy unbrokenCategoryIsDominatedBy = (UnbrokenCategoryIsDominatedBy) o;

                return unbrokenCategoryDominates.Equals(unbrokenCategoryIsDominatedBy.unbrokenCategoryDominates);
            }

            /*@Override*/

            public override int GetHashCode()
            {
                int result = base.GetHashCode();
                result = 29*result + unbrokenCategoryDominates.GetHashCode();
                return result;
            }
        }


        /**
   * Note that this only works properly for context-free trees.
   * Also, the use of initialize and advance is not very efficient just yet.  Finally, each node in the tree
   * is added only once, even if there is more than one unbroken-category precedence path to it.
   *
   */

        private /*static*/ class UnbrokenCategoryPrecedes : Relation
        {

            private new static readonly long serialVersionUID = 6866888667804306111L;

            private readonly Regex pattern;
            private readonly bool negatedPattern;
            private readonly bool basicCat;
            private Func<string, string> basicCatFunction;

            /**
     * @param arg The pattern to match, perhaps preceded by ! and/or @
     */

            public UnbrokenCategoryPrecedes(string arg,
                Func<string, string> basicCatFunction) : base(".+(" + arg + ')')
            {
                if (arg.StartsWith("!"))
                {
                    negatedPattern = true;
                    arg = arg.Substring(1);
                }
                else
                {
                    negatedPattern = false;
                }
                if (arg.StartsWith("@"))
                {
                    basicCat = true;
                    this.basicCatFunction = basicCatFunction;
                        // todo -- this was missing a this. which must be testable in a unit test!!! Make one
                    arg = arg.Substring(1);
                }
                else
                {
                    basicCat = false;
                }
                if (Regex.IsMatch(arg, "/.*/"))
                {
                    //pattern = new Regex(arg.Substring(1, arg.Length - 1));
                    pattern = new Regex(arg.Substring(1, arg.Length - 2));
                }
                else if (Regex.IsMatch(arg, "__"))
                {
                    pattern = new Regex("^.*$");
                }
                else
                {
                    pattern = new Regex("^(?:" + arg + ")$");
                }
            }

            /** {@inheritDoc} */
            //@Override
            public override bool satisfies(Tree t1, Tree t2, Tree root, /*readonly */TregexMatcher matcher)
            {
                return true; // shouldn't have to do anything here.
            }

            private bool pathMatchesNode(Tree node)
            {
                string lab = node.value();
                // added this code to not crash if null node, even though there probably should be null nodes in the tree
                if (lab == null)
                {
                    // Say that a null label matches no positive pattern, but any negated pattern
                    return negatedPattern;
                }
                else
                {
                    if (basicCat)
                    {
                        lab = basicCatFunction(lab);
                    }
                    var m = pattern.Match(lab);
                    return m.Success != negatedPattern;
                }
            }

            private void initializeHelper(Stack<Tree> stack, Tree node, Tree root, TregexMatcher matcher,
                IdentityHashSet<Tree> nodesToSearch)
            {
                if (node == root)
                {
                    return;
                }
                Tree parent = matcher.getParent(node);
                int i = parent.objectIndexOf(node);
                while (i == parent.children().Length - 1 && parent != root)
                {
                    node = parent;
                    parent = matcher.getParent(parent);
                    i = parent.objectIndexOf(node);
                }
                Tree followingNode;
                if (i + 1 < parent.children().Length)
                {
                    followingNode = parent.children()[i + 1];
                }
                else
                {
                    followingNode = null;
                }
                while (followingNode != null)
                {
                    //System.err.println("adding to stack node " + followingNode.ToString());
                    if (! nodesToSearch.Contains(followingNode))
                    {
                        stack.Push(followingNode);
                        nodesToSearch.Add(followingNode);
                    }
                    if (pathMatchesNode(followingNode))
                    {
                        initializeHelper(stack, followingNode, root, matcher, nodesToSearch);
                    }
                    if (! followingNode.isLeaf())
                    {
                        followingNode = followingNode.children()[0];
                    }
                    else
                    {
                        followingNode = null;
                    }
                }
            }

            /** {@inheritDoc} */
            //@Override
            public override IEnumerator<Tree> searchNodeIterator( /*readonly */ Tree t,
                /*readonly */TregexMatcher matcher)
            {
                var stack = new Stack<Tree>();
                initializeHelper(stack, t, matcher.getRoot(), matcher, new IdentityHashSet<Tree>());

                return stack.GetEnumerator();
                /*return new SearchNodeIterator() {
        private IdentityHashSet<Tree> nodesToSearch;
        private Stack<Tree> searchStack;

        @Override
        public void initialize() {
          nodesToSearch = new IdentityHashSet<Tree>();
          searchStack = new Stack<Tree>();
          initializeHelper(searchStack, t, matcher.getRoot());
          advance();
        }

        private void initializeHelper(Stack<Tree> stack, Tree node, Tree root) {
          if (node==root) {
            return;
          }
          Tree parent = matcher.getParent(node);
          int i = parent.objectIndexOf(node);
          while (i == parent.children().Length-1 && parent != root) {
            node = parent;
            parent = matcher.getParent(parent);
            i = parent.objectIndexOf(node);
          }
          Tree followingNode;
          if (i+1 < parent.children().Length) {
            followingNode = parent.children()[i+1];
          } else {
            followingNode = null;
          }
          while (followingNode != null) {
            //System.err.println("adding to stack node " + followingNode.ToString());
            if (! nodesToSearch.contains(followingNode)) {
              stack.Add(followingNode);
              nodesToSearch.Add(followingNode);
            }
            if (pathMatchesNode(followingNode)) {
              initializeHelper(stack, followingNode, root);
            }
            if (! followingNode.isLeaf()) {
              followingNode = followingNode.children()[0];
            } else {
              followingNode = null;
            }
          }
        }

        @Override
        void advance() {
          if (searchStack.isEmpty()) {
            next = null;
          } else {
            next = searchStack.pop();
          }
        }
      };*/
            }
        }


        /**
   * Note that this only works properly for context-free trees.
   * Also, the use of initialize and advance is not very efficient just yet.  Finally, each node in the tree
   * is added only once, even if there is more than one unbroken-category precedence path to it.
   */

        private /*static */ class UnbrokenCategoryFollows : Relation
        {

            private new static readonly long serialVersionUID = -7890430001297866437L;

            private readonly Regex pattern;
            private readonly bool negatedPattern;
            private readonly bool basicCat;
            private Func<string, string> basicCatFunction;

            /**
     * @param arg The pattern to match, perhaps preceded by ! and/or @
     */

            public UnbrokenCategoryFollows(string arg,
                Func<string, string> basicCatFunction) : base(",+(" + arg + ')')
            {
                if (arg.StartsWith("!"))
                {
                    negatedPattern = true;
                    arg = arg.Substring(1);
                }
                else
                {
                    negatedPattern = false;
                }
                if (arg.StartsWith("@"))
                {
                    basicCat = true;
                    this.basicCatFunction = basicCatFunction;
                    arg = arg.Substring(1);
                }
                else
                {
                    basicCat = false;
                }
                if (Regex.IsMatch(arg, "/.*/"))
                {
                    //pattern = new Regex(arg.Substring(1, arg.Length - 1));
                    pattern = new Regex(arg.Substring(1, arg.Length - 2));
                }
                else if (Regex.IsMatch(arg, "__"))
                {
                    pattern = new Regex("^.*$");
                }
                else
                {
                    pattern = new Regex("^(?:" + arg + ")$");
                }
            }

            /** {@inheritDoc} */
            //@Override
            public override bool satisfies(Tree t1, Tree t2, Tree root, /*readonly */TregexMatcher matcher)
            {
                return true; // shouldn't have to do anything here.
            }

            private bool pathMatchesNode(Tree node)
            {
                string lab = node.value();
                // added this code to not crash if null node, even though there probably should be null nodes in the tree
                if (lab == null)
                {
                    // Say that a null label matches no positive pattern, but any negated pattern
                    return negatedPattern;
                }
                else
                {
                    if (basicCat)
                    {
                        lab = basicCatFunction(lab);
                    }
                    var m = pattern.Match(lab);
                    return m.Success != negatedPattern;
                }
            }

            private void initializeHelper(Stack<Tree> stack, Tree node, Tree root, TregexMatcher matcher,
                IdentityHashSet<Tree> nodesToSearch)
            {
                if (node == root)
                {
                    return;
                }
                Tree parent = matcher.getParent(node);
                int i = parent.objectIndexOf(node);
                while (i == 0 && parent != root)
                {
                    node = parent;
                    parent = matcher.getParent(parent);
                    i = parent.objectIndexOf(node);
                }
                Tree precedingNode;
                if (i > 0)
                {
                    precedingNode = parent.children()[i - 1];
                }
                else
                {
                    precedingNode = null;
                }
                while (precedingNode != null)
                {
                    //System.err.println("adding to stack node " + precedingNode.ToString());
                    if (! nodesToSearch.Contains(precedingNode))
                    {
                        stack.Push(precedingNode);
                        nodesToSearch.Add(precedingNode);
                    }
                    if (pathMatchesNode(precedingNode))
                    {
                        initializeHelper(stack, precedingNode, root, matcher, nodesToSearch);
                    }
                    if (! precedingNode.isLeaf())
                    {
                        precedingNode = precedingNode.children()[0];
                    }
                    else
                    {
                        precedingNode = null;
                    }
                }
            }

            /** {@inheritDoc} */
            //@Override
            public override IEnumerator<Tree> searchNodeIterator( /*readonly */ Tree t,
                /*readonly */TregexMatcher matcher)
            {
                var searchStack = new Stack<Tree>();
                initializeHelper(searchStack, t, matcher.getRoot(), matcher, new IdentityHashSet<Tree>());

                return searchStack.GetEnumerator();
                /*return new SearchNodeIterator() {
        IdentityHashSet<Tree> nodesToSearch;
        Stack<Tree> searchStack;

        @Override
        public void initialize() {
          nodesToSearch = new IdentityHashSet<Tree>();
          searchStack = new Stack<Tree>();
          initializeHelper(searchStack, t, matcher.getRoot());
          advance();
        }

        private void initializeHelper(Stack<Tree> stack, Tree node, Tree root) {
          if (node==root) {
            return;
          }
          Tree parent = matcher.getParent(node);
          int i = parent.objectIndexOf(node);
          while (i == 0 && parent != root) {
            node = parent;
            parent = matcher.getParent(parent);
            i = parent.objectIndexOf(node);
          }
          Tree precedingNode;
          if (i > 0) {
            precedingNode = parent.children()[i-1];
          } else {
            precedingNode = null;
          }
          while (precedingNode != null) {
            //System.err.println("adding to stack node " + precedingNode.ToString());
            if ( ! nodesToSearch.contains(precedingNode)) {
              stack.Add(precedingNode);
              nodesToSearch.Add(precedingNode);
            }
            if (pathMatchesNode(precedingNode)) {
              initializeHelper(stack, precedingNode, root);
            }
            if (! precedingNode.isLeaf()) {
              precedingNode = precedingNode.children()[0];
            } else {
              precedingNode = null;
            }
          }
        }

        @Override
        void advance() {
          if (searchStack.isEmpty()) {
            next = null;
          } else {
            next = searchStack.pop();
          }
        }
      };*/
            }
        }
    }
}