using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenNLP.Tools.Parser;
using OpenNLP.Tools.Util;

namespace OpenNLP.Tools.Trees.TRegex
{
    /// <summary>
    /// A TregexPattern is a <code>tgrep</code>-type pattern for matching tree
    /// node configurations.  Unlike <code>tgrep</code> or <code>tgrep2</code>but like Unix
    /// <code>grep</code>, there is no pre-indexing of the data to be searched.
    /// Rather there is a linear scan through the trees where matches are sought.
    /// As a result, matching is slower, but a TregexPattern can be applied
    /// to an arbitrary set of trees at runtime in a processing pipeline.
    ///
    /// TregexPattern instances can be matched against instances of the {@link Tree} class.
    /// The {@link #main} method can be used to find matching nodes of a treebank from the command line.
    ///
    /// Currently supported node-node relations and their symbols:
    /// 
    /// <table border = "1">
    /// <tr><th>Symbol</th>th>Meaning</tr>
    /// <tr><td>A &lt;&lt; B </td>td>A dominates B</tr>
    /// <tr><td>A &gt;&gt; B </td>td>A is dominated by B</tr>
    /// <tr><td>A &lt; B </td>td>A immediately dominates B</tr>
    /// <tr><td>A &gt; B </td>A is immediately dominated by B</tr>
    /// <tr><td>A &#36; B </td>A is a sister of B (and not equal to B)</tr>
    /// <tr><td>A .. B </td>A precedes B</tr>
    /// <tr><td>A . B </td>A immediately precedes B</tr>
    /// <tr><td>A ,, B </td>A follows B</tr>
    /// <tr><td>A , B </td>A immediately follows B</tr>
    /// <tr><td>A &lt;&lt;, B </td>B is a leftmost descendant of A</tr>
    /// <tr><td>A &lt;&lt;- B </td>B is a rightmost descendant of A</tr>
    /// <tr><td>A &gt;&gt;, B </td>A is a leftmost descendant of B</tr>
    /// <tr><td>A &gt;&gt;- B </td>A is a rightmost descendant of B</tr>
    /// <tr><td>A &lt;, B </td>B is the first child of A</tr>
    /// <tr><td>A &gt;, B </td>A is the first child of B</tr>
    /// <tr><td>A &lt;- B </td>B is the last child of A</tr>
    /// <tr><td>A &gt;- B </td>A is the last child of B</tr>
    /// <tr><td>A &lt;` B </td>B is the last child of A</tr>
    /// <tr><td>A &gt;` B </td>A is the last child of B</tr>
    /// <tr><td>A &lt;i B </td>B is the ith child of A (i > 0)</tr>
    /// <tr><td>A &gt;i B </td>A is the ith child of B (i > 0)</tr>
    /// <tr><td>A &lt;-i B </td>B is the ith-to-last child of A (i > 0)</tr>
    /// <tr><td>A &gt;-i B </td>A is the ith-to-last child of B (i > 0)</tr>
    /// <tr><td>A &lt;: B </td>B is the only child of A</tr>
    /// <tr><td>A &gt;: B </td>A is the only child of B</tr>
    /// <tr><td>A &lt;&lt;: B </td>A dominates B via an unbroken chain (length > 0) of unary local trees.</tr>
    /// <tr><td>A &gt;&gt;: B </td>A is dominated by B via an unbroken chain (length > 0) of unary local trees.</tr>
    /// <tr><td>A &#36;++ B </td>A is a left sister of B (same as &#36;.. for context-free trees)</tr>
    /// <tr><td>A &#36;-- B </td>A is a right sister of B (same as &#36;,, for context-free trees)</tr>
    /// <tr><td>A &#36;+ B </td>A is the immediate left sister of B (same as &#36;. for context-free trees)</tr>
    /// <tr><td>A &#36;- B </td>A is the immediate right sister of B (same as &#36;, for context-free trees)</tr>
    /// <tr><td>A &#36;.. B </td>A is a sister of B and precedes B</tr>
    /// <tr><td>A &#36;,, B </td>A is a sister of B and follows B</tr>
    /// <tr><td>A &#36;. B </td>A is a sister of B and immediately precedes B</tr>
    /// <tr><td>A &#36;, B </td>A is a sister of B and immediately follows B</tr>
    /// <tr><td>A &lt;+(C) B </td>A dominates B via an unbroken chain of (zero or more) nodes matching description C</tr>
    /// <tr><td>A &gt;+(C) B </td>A is dominated by B via an unbroken chain of (zero or more) nodes matching description C</tr>
    /// <tr><td>A .+(C) B </td>A precedes B via an unbroken chain of (zero or more) nodes matching description C</tr>
    /// <tr><td>A ,+(C) B </td>A follows B via an unbroken chain of (zero or more) nodes matching description C</tr>
    /// <tr><td>A &lt;&lt;&#35; B </td>B is a head of phrase A</tr>
    /// <tr><td>A &gt;&gt;&#35; B </td>A is a head of phrase B</tr>
    /// <tr><td>A &lt;&#35; B </td>B is the immediate head of phrase A</tr>
    /// <tr><td>A &gt;&#35; B </td>A is the immediate head of phrase B</tr>
    /// <tr><td>A == B </td>A and B are the same node</tr>
    /// <tr><td>A &lt;= B </td>A and B are the same node or A is the parent of B</tr>
    /// <tr><td>A : B</td>[this is a pattern-segmenting operator that places no constraints on the relationship between A and B]</tr>
    /// <tr><td>A &lt;... { B ; C ; ... }</td>A has exactly B, C, etc as its subtree, with no other children.</tr>
    /// </table>
    /// Label descriptions can be literal strings, which much match labels
    /// exactly, or regular expressions in regular expression bars: /regex/.
    /// Literal string matching proceeds as String equality.
    /// In order to prevent ambiguity with other Tregex symbols, ASCII symbols are
    /// not allowed in literal strings, and they cannot begin with ASCII digits.
    /// (That is literals can be standard "identifiers" matching
    /// [a-zA-Z]([a-zA-Z0-9_-])/// but also may include letters from other alphabets.)
    /// If you want to use other symbols, you can do so by using a regular
    /// expression instead of a literal string.
    /// A disjunctive list of literal strings can be given separated by '|'.
    /// The special string '__' (two underscores) can be used to match any
    /// node.  (WARNING!!  Use of the '__' node description may seriously
    /// slow down search.)  If a label description is preceded by '@', the
    /// label will match any node whose <em>basicCategory</em> matches the
    /// description.  <emph>NB: A single '@' thus scopes over a disjunction
    /// specified by '|': @NP|VP means things with basic category NP or VP.
    /// </emph> The basicCategory is defined according to a Function
    /// mapping Strings to Strings, as provided by
    /// {@link edu.stanford.nlp.trees.AbstractTreebankLanguagePack#getBasicCategoryFunction()}.
    /// Label description regular expressions are matched as <code>find()</code>,
    /// as in Perl/tgrep;
    /// you need to use <code>^</code> or <code>$</code> to constrain matches to
    /// the ends of strings.
    /// <p/>
    /// In a chain of relations, all relations are relative to the first node in
    /// the chain. For example, <code> (S &lt; VP &lt; NP) </code> means
    /// "an S over a VP and also over an NP".
    /// If instead what you want is an S above a VP above an NP, you should write
    /// "<code>S &lt; (VP &lt; NP)</code>".
    /// Nodes can be grouped using parentheses '(' and ')'
    /// as in <code> S &lt; (NP $++ VP) </code> to match an S
    /// over an NP, where the NP has a VP as a right sister.
    ///
    /// <h3>Notes on relations</h3>
    ///
    /// 
    /// Node <code>B</code> "follows" node <code>A</code> if <code>B</code>
    /// or one of its ancestors is a right sibling of <code>A</code> or one
    /// of its ancestors.  Node <code>B</code> "immediately follows" node
    /// <code>A</code> if <code>B</code> follows <code>A</code> and there
    /// is no node <code>C</code> such that <code>B</code> follows
    /// <code>C</code> and <code>C</code> follows <code>A</code>.
    ///
    /// 
    /// Node <code>A</code> dominates <code>B</code> through an unbroken
    /// chain of unary local trees only if <code>A</code> is also
    /// unary. <code>(A (B))</code> is a valid example that matches <code>A
    /// &lt;&lt;: B</code>
    ///
    /// 
    /// When specifying that nodes are dominated via an unbroken chain of
    /// nodes matching a description <code>C</code>, the description
    /// <code>C</code> cannot be a full Tregex expression, but only an
    /// expression specifying the name of the node.  Negation of this
    /// description is allowed.
    ///
    /// 
    /// == has the same precedence as the other relations, so the expression
    /// <code>A &lt;&lt; B == A &lt;&lt; C</code> associates as
    /// <code>(((A &lt;&lt; B) == A) &lt;&lt; C)</code>, not as
    /// <code>((A &lt;&lt; B) == (A &lt;&lt; C))</code>.  (Both expressions are
    /// equivalent, of course, but this is just an example.)
    ///
    /// <h3>Boolean relational operators</h3>
    ///
    /// Relations can be combined using the '&' and '|' operators,
    /// negated with the '!' operator, and made optional with the '?' operator.
    /// Thus <code> (NP &lt; NN | &lt; NNS) </code> will match an NP node dominating either
    /// an NN or an NNS.  <code> (NP > S & $++ VP) </code> matches an NP that
    /// is both under an S and has a VP as a right sister.
    /// 
    /// Expressions stop evaluating as soon as the result is known.  For
    /// example, if the pattern is <code>NP=a | NNP=b</code> and the NP
    /// matches, then variable <code>b</code> will not be assigned even if
    /// there is an NNP in the tree.
    ///
    /// Relations can be grouped using brackets '[' and ']'.  So the
    /// expression
    ///
    /// <blockquote>
    /// <code> NP [&lt; NN | &lt; NNS] & > S </code>
    /// </blockquote>
    ///
    ///  matches an NP that (1) dominates either an NN or an NNS, and (2) is under an S.  Without
    /// brackets, &amp; takes precedence over |, and equivalent operators are
    /// left-associative.  Also note that &amp; is the default combining operator if the
    /// operator is omitted in a chain of relations, so that the two patterns are equivalent:
    ///
    /// <blockquote>
    /// <code> (S &lt; VP &lt; NP) </code>
    /// <code> (S &lt; VP & &lt; NP) </code>
    /// </blockquote>
    ///
    /// As another example, <code> (VP &lt; VV | &lt; NP % NP)</code> 
    /// can be written explicitly as <code> (VP [&lt; VV | [&lt; NP & % NP] ] )</code>
    ///
    /// Relations can be negated with the '!' operator, in which case the
    /// expression will match only if there is no node satisfying the relation.
    /// For example <code> (NP !&lt; NNP) </code> matches only NPs not dominating
    /// an NNP.  Label descriptions can also be negated with '!': (NP &lt; !NNP|NNS) matches
    /// NPs dominating some node that is not an NNP or an NNS.

    /// Relations can be made optional with the '?' operator.  This way the
    /// expression will match even if the optional relation is not satisfied.  This is useful when used together
    ///  with node naming (see below).
    ///
    /// <h3>Basic Categories</h3>
    ///
    /// In order to consider only the "basic category" of a tree label,
    /// i.e. to ignore functional tags or other annotations on the label,
    /// prefix that node's description with the &#64; symbol.  For example
    /// <code> (@NP &lt; @/NN.?/) </code>  This can only be used for individual nodes;
    /// if you want all nodes to use the basic category, it would be more efficient
    /// to use a {@link edu.stanford.nlp.trees.TreeNormalizer} to remove functional
    /// tags before passing the tree to the TregexPattern.
    ///
    /// <h3>Segmenting patterns</h3>
    ///
    /// The ":" operator allows you to segment a pattern into two pieces.  This can simplify your pattern writing.  For example,
    /// the pattern
    ///
    /// <blockquote>
    ///   S : NP
    /// </blockquote>
    ///
    /// matches only those S nodes in trees that also have an NP node.
    ///
    /// <h3>Naming nodes</h3>
    ///
    /// Nodes can be given names (a.k.a. handles) using '='.  A named node will be stored in a
    /// map that maps names to nodes so that if a match is found, the node
    /// corresponding to the named node can be extracted from the map.  For
    /// example <code> (NP &lt; NNP=name) </code> will match an NP dominating an NNP
    /// and after a match is found, the map can be queried with the
    /// name to retreived the matched node using {@link TregexMatcher#getNode(String o)}
    /// with (String) argument "name" (<it>not</it> "=name").
    /// Note that you are not allowed to name a node that is under the scope of a negation operator (the semantics would
    /// be unclear, since you can't store a node that never gets matched to).
    /// Trying to do so will cause a {@link TregexParseException} to be thrown. Named nodes <it>can be put within the scope of an optionality operator</it>.
    ///
    /// Named nodes that refer back to previous named nodes need not have a node
    /// description -- this is known as "backreferencing".  In this case, the expression
    /// will match only when all instances of the same name get matched to the same tree node.
    /// For example: the pattern
    ///
    /// <blockquote>
    /// <code> (@NP &lt;, (@NP $+ (/,/ $+ (@NP $+ /,/=comma))) &lt;- =comma) </code>
    /// </blockquote>
    ///
    /// matches only an NP dominating exactly the four node sequence
    /// <code>NP , NP ,</code> -- the mother NP cannot have any other
    /// daughters. Multiple backreferences are allowed.  If the node w/ no
    /// node description does not refer to a previously named node, there
    /// will be no error, the expression simply will not match anything.
    ///
    /// Another way to refer to previously named nodes is with the "link" symbol: '~'.
    /// A link is like a backreference, except that instead of having to be <i>equal to</i> the
    /// referred node, the current node only has to match the label of the referred to node.
    /// A link cannot have a node description, i.e. the '~' symbol must immediately follow a
    /// relation symbol.
    ///
    /// <h3>Customizing headship and basic categories</h3>
    ///
    /// The HeadFinder used to determine heads for the head relations <code>&lt;#</code>, <code>&gt;#</code>, <code>&lt;&lt;#</code>, and <code>&gt;&gt;#</code>, and also
    /// the Function mapping from labels to Basic Category tags can be
    /// chosen by using a {@link TregexPatternCompiler}.
    ///
    /// <h3>Variable Groups</h3>
    ///
    /// If you write a node description using a regular expression, you can assign its matching groups to variable names.
    /// If more than one node has a group assigned to the same variable name, then matching will only occur when all such groups
    /// capture the same string.  This is useful for enforcing coindexation constraints.  The syntax is
    ///
    /// <blockquote>
    /// <code> / &lt;regex-stuff&gt; /#&lt;group-number&gt;%&lt;variable-name&gt;</code>
    /// </blockquote>
    ///
    /// For example, the pattern (designed for Penn Treebank trees)
    ///
    /// <blockquote>
    /// <code> @SBAR &lt; /^WH.///-([0-9]+)$/#1%index &lt;&lt; (__=empty &lt; (/^-NONE-/ &lt; /^\///T\///-([0-9]+)$/#1%index)) </code>
    /// </blockquote>
    ///
    /// will match only such that the WH- node under the SBAR is coindexed with the trace node that gets the name <code>empty</code>.
    ///
    /// <h3>Getting Started</h3>
    ///
    /// Suppose we want to find all examples of subtrees where the label of
    /// the root of the subtree starts with MW.  For example, we want any
    /// subtree whose root is labeled MWV, MWN, etc.
    /// 
    /// The first thing to do is figure out what pattern to use.  Since we
    /// want to match anything starting with MW, we use the pattern
    /// <code>/^MW/</code>.
    /// 
    /// We then create a pattern, find matches in a given tree, and process
    /// those matches as follows:
    /// <blockquote>
    /// <code>
    ///   // Create a reusable pattern object 
    ///   TregexPattern patternMW = TregexPattern.compile("/^MW/"); 
    ///   // Run the pattern on one particular tree 
    ///   TregexMatcher matcher = patternMW.matcher(tree); 
    ///   // Iterate over all of the subtrees that matched 
    ///   while (matcher.findNextMatchingNode()) { 
    ///     Tree match = matcher.getMatch(); 
    ///     // do what we want to with the subtree 
    ///   }
    /// </code>
    /// </blockquote>
    ///
    /// <h3>Current known bugs/shortcomings:</h3>
    ///
    /// <ul>
    /// <li> Tregex does not support disjunctions at the root level.  For
    /// example, the pattern <code>A | B</code> will not work.</li>
    /// <li> Using multiple variable strings in one regex may not
    /// necessarily work.  For example, suppose the first two regex
    /// patterns are <code>/(.///)/#1%foo</code> and
    /// <code>/(.///)/#1%bar</code>.  You might then want to write a pattern
    /// that matches the concatenation of these patterns,
    /// <code>/(.///)(.///)/#1%foo#2%bar</code>, but that will not work.</li>
    /// </ul>
    ///
    /// @author Galen Andrew
    /// @author Roger Levy (rog@csli.stanford.edu)
    /// @author Anna Rafferty (filter mode)
    /// @author John Bauer (extensively tested and bugfixed)
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public abstract class TregexPattern
    {
        private bool neg = false;
        private bool opt = false;
        private string patternString;

        public void Negate()
        {
            neg = true;
            if (opt)
            {
                throw new SystemException("Node cannot be both negated and optional.");
            }
        }

        public void MakeOptional()
        {
            opt = true;
            if (neg)
            {
                throw new SystemException("Node cannot be both negated and optional.");
            }
        }

        /*private void prettyPrint(PrintWriter pw, int indent) {
            for (int i = 0; i < indent; i++) {
                pw.print("   ");
            }
            if (neg) {
                pw.print('!');
            }
            if (opt) {
                pw.print('?');
            }
            pw.println(localString());
            for (TregexPattern child : getChildren()) {
                child.prettyPrint(pw, indent + 1);
            }
            }*/
        
        public abstract List<TregexPattern> GetChildren();

        public abstract string LocalString();

        public bool IsNegated()
        {
            return neg;
        }

        public bool IsOptional()
        {
            return opt;
        }

        public abstract TregexMatcher Matcher(Tree root, Tree tree,
            IdentityDictionary<Tree, Tree> nodesToParents,
            Dictionary<string, Tree> namesToNodes,
            VariableStrings variableStrings,
            IHeadFinder headFinder);

        /// <summary>
        /// Get a {@link TregexMatcher} for this pattern on this tree.
        /// </summary>
        /// <param name="t">a tree to match on</param>
        /// <returns>a TregexMatcher</returns>
        public TregexMatcher Matcher(Tree t)
        {
            // In the assumption that there will usually be very few names in
            // the pattern, we use an ArrayMap instead of a hash map
            // TODO: it would be even more efficient if we set this to be
            // exactly the right size
            return Matcher(t, t, null, new Dictionary<string, Tree>(), new VariableStrings(), null);
        }

        /// <summary>
        /// Get a {@link TregexMatcher} for this pattern on this tree.
        /// Any Relations which use heads of trees should use the provided HeadFinder.
        /// </summary>
        /// <param name="t">a tree to match on</param>
        /// <param name="headFinder">a HeadFinder to use when matching</param>
        /// <returns>a TregexMatcher</returns>
        public TregexMatcher Matcher(Tree t, IHeadFinder headFinder)
        {
            return Matcher(t, t, null, new Dictionary<string, Tree>(), new VariableStrings(), headFinder);
        }

        /// <summary>
        /// Creates a pattern from the given string using the default HeadFinder and
        /// BasicCategoryFunction.  If you want to use a different HeadFinder or
        /// BasicCategoryFunction, use a {@link TregexPatternCompiler} object.
        /// </summary>
        /// <param name="tregex">the pattern string</param>
        /// <returns>a TregexPattern for the string.</returns>
        public static TregexPattern Compile(string tregex)
        {
            return TregexPatternCompiler.defaultCompiler.Compile(tregex);
        }

        /// <summary>
        /// Creates a pattern from the given string using the default HeadFinder and
        /// BasicCategoryFunction.  If you want to use a different HeadFinder or
        /// BasicCategoryFunction, use a {@link TregexPatternCompiler} object.
        /// Rather than throwing an exception when the string does not parse,
        /// simply returns null.
        /// </summary>
        /// <param name="tregex">the pattern string</param>
        /// <param name="verbose">whether to log errors when the string doesn't parse</param>
        /// <returns>a TregexPattern for the string, or null if the string does not parse.</returns>
        public static TregexPattern SafeCompile(string tregex, bool verbose)
        {
            TregexPattern result = null;
            try
            {
                result = TregexPatternCompiler.defaultCompiler.Compile(tregex);
            }
            catch (TregexParseException ex)
            {
                if (verbose)
                {
                    //ex.printStackTrace();
                }
            }
            return result;
        }

        public string Pattern()
        {
            return patternString;
        }

        /// <summary>
        /// Only used by the TregexPatternCompiler to set the pattern. Pseudo-final
        /// </summary>
        public void SetPatternString(string patternString)
        {
            this.patternString = patternString;
        }

        /**
        * @return A single-line string representation of the pattern
        */
            /*@Override
        public abstract string ToString();*/

            /*/**
        * Print a multi-line representation
        * of the pattern illustrating it's syntax.
        #1#
        public void prettyPrint(PrintWriter pw) {
        prettyPrint(pw, 0);
        }

        /**
        * Print a multi-line representation
        * of the pattern illustrating it's syntax.
        #1#
        public void prettyPrint(PrintStream ps) {
        prettyPrint(new PrintWriter(new OutputStreamWriter(ps), true));
        }*/


        private static readonly Regex CodePattern = new Regex("([0-9]+):([0-9]+)", RegexOptions.Compiled);

        /*private static void extractSubtrees(List<string> codeStrings, string treeFile) {
            List<Tuple<int,int>> codes = new List<Tuple<int,int>>();
            foreach(string s in codeStrings) {
                //Matcher m = codePattern.matcher(s);
                var match = codePattern.Match(s);
                if (match.Success)
                    codes.Add(new Tuple<int, int>(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value)));
                else
                    throw new SystemException("Error: illegal node code " + s);
            }
            TreeReaderFactory trf = new TRegexTreeReaderFactory();
            MemoryTreebank treebank = new MemoryTreebank(trf);
            treebank.loadPath(treeFile,null, true);
            foreach (Tuple<int,int> code in codes) {
                Tree t = treebank.get(code.Item1 - 1);
                t.getNodeNumber(code.Item2).pennPrint();
            }
            }*/

        /*private static TreeReaderFactory getTreeReaderFactory(string treeReaderFactoryClassName) {
        TreeReaderFactory trf = new TRegexTreeReaderFactory();
        if (treeReaderFactoryClassName != null) {
            try {
            trf = (TreeReaderFactory) Class.forName(treeReaderFactoryClassName).newInstance();
            } catch(Exception e) {
            throw new SystemException("Error occurred while constructing TreeReaderFactory: " + e);
            }
        }
        return trf;
        }

        private static Treebank treebank; // used by main method, must be accessible*/

            // not thread-safe, but only used by TregexPattern's main method
            /*private static class TRegexTreeVisitor: TreeVisitor {

        private static bool printNumMatchesToStdOut = false;
        static bool printNonMatchingTrees = false;
        static bool printSubtreeCode = false;
        static bool printTree = false;
        static bool printWholeTree = false;
        static bool printMatches = true;
        static bool printFilename = false;
        static bool oneMatchPerRootNode = false;
        static bool reportTreeNumbers = false;

        //static TreePrint tp;
        //private PrintWriter pw;

        int treeNumber = 0;

        private readonly TregexPattern p;
        string[] handles;
        int numMatches;
        
        // todo: add an option to only print each tree once, regardless.  Most useful in conjunction with -w
        public void visitTree(Tree t) {
            treeNumber++;
            if (printTree) {
            pw.print(treeNumber+":");
            pw.println("Next tree read:");
            tp.printTree(t,pw);
            }
            TregexMatcher match = p.matcher(t);
            if(printNonMatchingTrees) {
            if(match.find())
                numMatches++;
            else
                tp.printTree(t,pw);
            return;
            }
            Tree lastMatchingRootNode = null;
            while (match.find()) {
            if(oneMatchPerRootNode) {
                if(lastMatchingRootNode == match.getMatch())
                continue;
                else
                lastMatchingRootNode = match.getMatch();
            }
            numMatches++;
            if (printFilename && treebank instanceof DiskTreebank) {
                DiskTreebank dtb = (DiskTreebank) treebank;
                pw.print("# ");
                pw.println(dtb.getCurrentFilename());
            }
            if(printSubtreeCode) {
                pw.print(treeNumber);
                pw.print(':');
                pw.println(match.getMatch().nodeNumber(t));
            }
            if (printMatches) {
                if(reportTreeNumbers) {
                pw.print(treeNumber);
                pw.print(": ");
                }
                if (printTree) {
                pw.println("Found a full match:");
                }
                if (printWholeTree) {
                tp.printTree(t,pw);
                } else if (handles != null) {
                if (printTree) {
                    pw.println("Here's the node you were interested in:");
                }
                for (string handle : handles) {
                    Tree labeledNode = match.getNode(handle);
                    if (labeledNode != null) {
                    tp.printTree(labeledNode,pw);
                    }
                }
                } else {
                tp.printTree(match.getMatch(),pw);
                }
                // pw.println();  // TreePrint already puts a blank line in
            } // end if (printMatches)
            } // end while match.find()
        } // end visitTree

        public int numMatches() {
            return numMatches;
        }

        } // end class TRegexTreeVisitor*/
        

        /*public static class TRegexTreeReaderFactory: TreeReaderFactory {

            private readonly TreeNormalizer tn;

            public TRegexTreeReaderFactory() {
                this(new TreeNormalizer() {
                /**
                    *
                    #1#

                @Override
                public string normalizeNonterminal(string str) {
                    if (str == null) {
                    return "";
                    } else {
                    return str;
                    }
                }
                });
            }

            public TRegexTreeReaderFactory(TreeNormalizer tn) {
                this.tn = tn;
            }

            public TreeReader newTreeReader(Reader input) {
                return new PennTreeReader(new BufferedReader(input), new LabeledScoredTreeFactory(), tn);
            }

            } // end class TRegexTreeReaderFactory*/
    }
}