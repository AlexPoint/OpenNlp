using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Graphs;
using OpenNLP.Tools.Util.Ling;

namespace OpenNLP.Tools.Util.Trees
{
    /**
 * A {@code GrammaticalStructure} stores dependency relations between
 * nodes in a tree.  A new <code>GrammaticalStructure</code> is constructed
 * from an existing parse tree with the help of {@link
 * GrammaticalRelation <code>GrammaticalRelation</code>}, which
 * defines a hierarchy of grammatical relations, along with
 * patterns for identifying them in parse trees.  The constructor for
 * <code>GrammaticalStructure</code> uses these definitions to
 * populate the new <code>GrammaticalStructure</code> with as many
 * labeled grammatical relations as it can.  Once constructed, the new
 * <code>GrammaticalStructure</code> can be printed in various
 * formats, or interrogated using the interface methods in this
 * class. Internally, this uses a representation via a {@code TreeGraphNode},
 * that is, a tree with additional labeled
 * arcs between nodes, for representing the grammatical relations in a
 * parse tree.
 * <p/>
 * <b>Caveat emptor!</b> This is a work in progress.
 * Nothing in here should be relied upon to function perfectly.
 * Feedback welcome.
 *
 * @author Bill MacCartney
 * @author Galen Andrew (refactoring English-specific stuff)
 * @author Ilya Sherman (dependencies)
 * @author Daniel Cer
 * @see EnglishGrammaticalRelations
 * @see GrammaticalRelation
 * @see EnglishGrammaticalStructure
 */

    [Serializable]
    public /*abstract*/ class GrammaticalStructure
    {

        //private static readonly bool PRINT_DEBUGGING = System.getProperty("GrammaticalStructure", null) != null;

        protected readonly List<TypedDependency> typedDependencies;
        protected readonly List<TypedDependency> allTypedDependencies;

        protected readonly Predicate<string> puncFilter;

        /**
   * The root Tree node for this GrammaticalStructure.
   */
        protected readonly TreeGraphNode root;

        /**
   * A map from arbitrary integer indices to nodes.
   */
        private readonly Dictionary<int, TreeGraphNode> indexMap = new Dictionary<int, TreeGraphNode>();

        /**
   * Create a new GrammaticalStructure, analyzing the parse tree and
   * populate the GrammaticalStructure with as many labeled
   * grammatical relation arcs as possible.
   *
   * @param t             A Tree to analyze
   * @param relations     A set of GrammaticalRelations to consider
   * @param relationsLock Something needed to make this thread-safe
   * @param hf            A HeadFinder for analysis
   * @param puncFilter    A Filter to reject punctuation. To delete punctuation
   *                      dependencies, this filter should return false on
   *                      punctuation word strings, and true otherwise.
   *                      If punctuation dependencies should be kept, you
   *                      should pass in a Filters.&lt;String&gt;acceptFilter().
   */

        public GrammaticalStructure(Tree t, ICollection<GrammaticalRelation> relations,
            Object relationsLock, HeadFinder hf, Predicate<string> puncFilter)
        {
            this.root = new TreeGraphNode(t, this);
            indexNodes(this.root);
            // add head word and tag to phrase nodes
            if (hf == null)
            {
                throw new InvalidDataException("Cannot use null HeadFinder");
            }
            root.percolateHeads(hf);
            if (root.value() == null)
            {
                root.setValue("ROOT"); // todo: cdm: it doesn't seem like this line should be here
            }
            // add dependencies, using heads
            this.puncFilter = puncFilter;
            NoPunctFilter puncDepFilter = new NoPunctFilter(puncFilter);
            NoPunctTypedDependencyFilter puncTypedDepFilter = new NoPunctTypedDependencyFilter(puncFilter);

            DirectedMultiGraph<TreeGraphNode, GrammaticalRelation> basicGraph =
                new DirectedMultiGraph<TreeGraphNode, GrammaticalRelation>();
            DirectedMultiGraph<TreeGraphNode, GrammaticalRelation> completeGraph =
                new DirectedMultiGraph<TreeGraphNode, GrammaticalRelation>();

            // analyze the root (and its descendants, recursively)
            /*if (relationsLock != null) {
      relationsLock.lock();
    }
    try {
      analyzeNode(root, root, relations, hf, puncFilter, basicGraph, completeGraph);
    }
    finally {
      if (relationsLock != null) {
        relationsLock.unlock();
      }
    }*/
            lock (relationsLock)
            {
                analyzeNode(root, root, relations, hf, puncFilter, basicGraph, completeGraph);
            }

            attachStrandedNodes(root, root, false, puncFilter, basicGraph);

            // add typed dependencies
            typedDependencies = getDeps(puncTypedDepFilter.test, basicGraph);
            allTypedDependencies = new List<TypedDependency>(typedDependencies);
            getExtraDeps(allTypedDependencies, puncTypedDepFilter.test, completeGraph);
        }


        /**
   * Assign sequential integer indices (starting with 1) to all
   * nodes of the subtree rooted at this
   * <code>Tree</code>.  The leaves are indexed first,
   * from left to right.  Then the internal nodes are indexed,
   * using a pre-order tree traversal.
   */

        private void indexNodes(TreeGraphNode tree)
        {
            indexNodes(tree, indexLeaves(tree, 1));
        }

        /**
   * Assign sequential integer indices to the leaves of the subtree
   * rooted at this <code>TreeGraphNode</code>, beginning with
   * <code>startIndex</code>, and traversing the leaves from left
   * to right. If node is already indexed, then it uses the existing index.
   *
   * @param startIndex index for this node
   * @return the next index still unassigned
   */

        private int indexLeaves(TreeGraphNode tree, int startIndex)
        {
            if (tree.isLeaf())
            {
                int oldIndex = tree.index();
                if (oldIndex >= 0)
                {
                    startIndex = oldIndex;
                }
                else
                {
                    tree.setIndex(startIndex);
                }
                addNodeToIndexMap(startIndex, tree);
                startIndex++;
            }
            else
            {
                foreach (TreeGraphNode child in tree.children())
                {
                    startIndex = indexLeaves(child, startIndex);
                }
            }
            return startIndex;
        }

        /**
   * Assign sequential integer indices to all nodes of the subtree
   * rooted at this <code>TreeGraphNode</code>, beginning with
   * <code>startIndex</code>, and doing a pre-order tree traversal.
   * Any node which already has an index will not be re-indexed
   * &mdash; this is so that we can index the leaves first, and
   * then index the rest.
   *
   * @param startIndex index for this node
   * @return the next index still unassigned
   */

        private int indexNodes(TreeGraphNode tree, int startIndex)
        {
            if (tree.index() < 0)
            {
                // if this node has no index
                addNodeToIndexMap(startIndex, tree);
                tree.setIndex(startIndex++);
            }
            if (!tree.isLeaf())
            {
                foreach (TreeGraphNode child in tree.children())
                {
                    startIndex = indexNodes(child, startIndex);
                }
            }
            return startIndex;
        }

        /**
   * Store a mapping from an arbitrary integer index to a node in
   * this treegraph.  Normally a client shouldn't need to use this,
   * as the nodes are automatically indexed by the
   * <code>TreeGraph</code> constructor.
   *
   * @param index the arbitrary integer index
   * @param node  the <code>TreeGraphNode</code> to be indexed
   */

        private void addNodeToIndexMap(int index, TreeGraphNode node)
        {
            indexMap.Add(index, node);
        }


        /**
   * Return the node in the this treegraph corresponding to the
   * specified integer index.
   *
   * @param index the integer index of the node you want
   * @return the <code>TreeGraphNode</code> having the specified
   *         index (or <code>null</code> if such does not exist)
   */

        private TreeGraphNode getNodeByIndex(int index)
        {
            return indexMap[index];
        }

        /**
   * Return the root Tree of this GrammaticalStructure.
   *
   * @return the root Tree of this GrammaticalStructure
   */

        public TreeGraphNode mroot()
        {
            return root;
        }

        private static void throwDepFormatException(string dep)
        {
            throw new SystemException(
                string.Format("Dependencies should be for the format 'type(arg-idx, arg-idx)'. Could not parse '{0}'",
                    dep));
        }

        /**
   * Create a grammatical structure from its string representation.
   *
   * Like buildCoNLLXGrammaticalStructure,
   * this method fakes up the parts of the tree structure that are not
   * used by the grammatical relation transformation operations.
   *
   * <i>Note:</i> Added by daniel cer
   *
   * @param tokens
   * @param posTags
   * @param deps
   */

        public static GrammaticalStructure fromStringReps(List<string> tokens, List<string> posTags, List<string> deps)
        {
            if (tokens.Count != posTags.Count)
            {
                throw new SystemException(String.Format(
                    "tokens.Count: {0} != pos.Count: {1}\n", tokens.Count, posTags.Count));
            }

            List<TreeGraphNode> tgWordNodes = new List<TreeGraphNode>(tokens.Count);
            List<TreeGraphNode> tgPOSNodes = new List<TreeGraphNode>(tokens.Count);

            CoreLabel rootLabel = new CoreLabel();
            rootLabel.setValue("ROOT");
            List<IndexedWord> nodeWords = new List<IndexedWord>(tgPOSNodes.Count + 1);
            nodeWords.Add(new IndexedWord(rootLabel));

            SemanticHeadFinder headFinder = new SemanticHeadFinder();

            IEnumerator<string> posIter = posTags.GetEnumerator();
            foreach (string wordString in tokens)
            {
                string posString = posIter.Current;
                posIter.MoveNext();
                CoreLabel wordLabel = new CoreLabel();
                wordLabel.setWord(wordString);
                wordLabel.setValue(wordString);
                wordLabel.setTag(posString);
                TreeGraphNode word = new TreeGraphNode(wordLabel);
                CoreLabel tagLabel = new CoreLabel();
                tagLabel.setValue(posString);
                tagLabel.setWord(posString);
                TreeGraphNode pos = new TreeGraphNode(tagLabel);
                tgWordNodes.Add(word);
                tgPOSNodes.Add(pos);
                TreeGraphNode[] childArr = {word};
                pos.setChildren(childArr);
                word.setParent(pos);
                pos.percolateHeads(headFinder);
                nodeWords.Add(new IndexedWord(wordLabel));
            }

            TreeGraphNode root = new TreeGraphNode(rootLabel);

            root.setChildren(tgPOSNodes.ToArray());

            root.setIndex(0);

            // Build list of TypedDependencies
            List<TypedDependency> tdeps = new List<TypedDependency>(deps.Count);

            foreach (string depString in deps)
            {
                int firstBracket = depString.IndexOf('(');
                if (firstBracket == -1) throwDepFormatException(depString);


                string type = depString.Substring(0, firstBracket);

                if (depString[depString.Length - 1] != ')') throwDepFormatException(depString);

                string args = depString.Substring(firstBracket + 1, depString.Length - 1);

                int argSep = args.IndexOf(", ");
                if (argSep == -1) throwDepFormatException(depString);

                string parentArg = args.Substring(0, argSep);
                string childArg = args.Substring(argSep + 2);
                int parentDash = parentArg.LastIndexOf('-');
                if (parentDash == -1) throwDepFormatException(depString);
                int childDash = childArg.LastIndexOf('-');
                if (childDash == -1) throwDepFormatException(depString);
                //System.err.printf("parentArg: %s\n", parentArg);
                int parentIdx = int.Parse(parentArg.Substring(parentDash + 1).Replace("'", ""));

                int childIdx = int.Parse(childArg.Substring(childDash + 1).Replace("'", ""));

                GrammaticalRelation grel = new GrammaticalRelation(GrammaticalRelation.Language.Any, type, null,
                    GrammaticalRelation.DEPENDENT);

                TypedDependency tdep = new TypedDependency(grel, nodeWords[parentIdx], nodeWords[childIdx]);
                tdeps.Add(tdep);
            }

            // TODO add some elegant way to construct language
            // appropriate GrammaticalStructures (e.g., English, Chinese, etc.)
            return new GrammaticalStructure(tdeps, root)
            {
                //private static readonly long serialVersionUID = 1L;
            };
        }

        public GrammaticalStructure(List<TypedDependency> projectiveDependencies, TreeGraphNode root)
        {
            this.root = root;
            indexNodes(this.root);
            this.puncFilter = Filters.AcceptFilter<string>();
            allTypedDependencies = typedDependencies = new List<TypedDependency>(projectiveDependencies);
        }

        public GrammaticalStructure(Tree t, ICollection<GrammaticalRelation> relations,
            HeadFinder hf, Predicate<string> puncFilter) :
                this(t, relations, null, hf, puncFilter)
        {
        }

        //@Override
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(root.toPrettyString(0).Substring(1));
            sb.Append("Typed Dependencies:\n");
            sb.Append(typedDependencies);
            return sb.ToString();
        }

        private static void attachStrandedNodes(TreeGraphNode t, TreeGraphNode root, bool attach,
            Predicate<string> puncFilter, DirectedMultiGraph<TreeGraphNode, GrammaticalRelation> basicGraph)
        {
            if (t.isLeaf())
            {
                return;
            }
            if (attach && puncFilter(t.headWordNode().label().value()))
            {
                // make faster by first looking for links from parent
                // it is necessary to look for paths using all directions
                // because sometimes there are edges created from lower nodes to
                // nodes higher up
                var parentAsTreeGraphNode = t.parent() as TreeGraphNode;
                if (parentAsTreeGraphNode != null)
                {
                    TreeGraphNode parent = parentAsTreeGraphNode.highestNodeWithSameHead();
                    if (!basicGraph.isEdge(parent, t) && basicGraph.getShortestPath(root, t, false) == null)
                    {
                        basicGraph.add(parent, t, GrammaticalRelation.DEPENDENT);
                    }
                }
                else
                {
                    throw new SystemException("Should never be here.");
                }
            }
            foreach (TreeGraphNode kid in t.children())
            {
                attachStrandedNodes(kid, root, (kid.headWordNode() != t.headWordNode()), puncFilter, basicGraph);
            }
        }

        // cdm dec 2009: I changed this to automatically fail on preterminal nodes, since they shouldn't match for GR parent patterns.  Should speed it up.
        private static void analyzeNode(TreeGraphNode t, TreeGraphNode root, ICollection<GrammaticalRelation> relations,
            HeadFinder hf, Predicate<string> puncFilter,
            DirectedMultiGraph<TreeGraphNode, GrammaticalRelation> basicGraph,
            DirectedMultiGraph<TreeGraphNode, GrammaticalRelation> completeGraph)
        {
            //Console.WriteLine("analyzeNode()");
            if (t.isPhrasal())
            {
                // don't do leaves or preterminals!
                //Console.WriteLine("isPhrasal = true");
                TreeGraphNode tHigh = t.highestNodeWithSameHead();
                foreach (GrammaticalRelation egr in relations)
                {
                    if (egr.isApplicable(t))
                    {
                        //Console.WriteLine("isApplicable = true");
                        foreach (TreeGraphNode u in egr.getRelatedNodes(t, root, hf))
                        {
                            TreeGraphNode uHigh = u.highestNodeWithSameHead();
                            if (uHigh == tHigh)
                            {
                                continue;
                            }
                            if (!puncFilter(uHigh.headWordNode().label().value()))
                            {
                                continue;
                            }
                            completeGraph.add(tHigh, uHigh, egr);
                            // If there are two patterns that add dependencies, X --> Z and Y --> Z, and X dominates Y, then the dependency Y --> Z is not added to the basic graph to prevent unwanted duplication.
                            // Similarly, if there is already a path from X --> Y, and an expression would trigger Y --> X somehow, we ignore that
                            ReadOnlyCollection<TreeGraphNode> parents = basicGraph.getParents(uHigh);
                            if ((parents == null || parents.Count == 0 || parents.Contains(tHigh)) &&
                                basicGraph.getShortestPath(uHigh, tHigh, true) == null)
                            {
                                // System.err.println("Adding " + egr.getShortName() + " from " + t + " to " + u + " tHigh=" + tHigh + "(" + tHigh.headWordNode() + ") uHigh=" + uHigh + "(" + uHigh.headWordNode() + ")");
                                basicGraph.add(tHigh, uHigh, egr);
                            }
                        }
                    }
                }
                // now recurse into children
                foreach (TreeGraphNode kid in t.children())
                {
                    analyzeNode(kid, root, relations, hf, puncFilter, basicGraph, completeGraph);
                }
            }
        }

        private void getExtraDeps(List<TypedDependency> deps, Predicate<TypedDependency> puncTypedDepFilter,
            DirectedMultiGraph<TreeGraphNode, GrammaticalRelation> completeGraph)
        {
            getExtras(deps);
            // adds stuff to basicDep based on the tregex patterns over the tree
            getTreeDeps(deps, completeGraph, puncTypedDepFilter, extraTreeDepFilter());
            deps.Sort();
        }


        /**
   * Helps the constructor build a list of typed dependencies using
   * information from a {@code GrammaticalStructure}.
   */

        private List<TypedDependency> getDeps(Predicate<TypedDependency> puncTypedDepFilter,
            DirectedMultiGraph<TreeGraphNode, GrammaticalRelation> basicGraph)
        {
            List<TypedDependency> basicDep = new List<TypedDependency>();

            foreach (TreeGraphNode gov in basicGraph.getAllVertices())
            {
                foreach (TreeGraphNode dep in basicGraph.getChildren(gov))
                {
                    var govCLabel = gov.label() as CoreLabel;
                    var depCLabel = dep.label() as CoreLabel;
                    if (govCLabel != null && depCLabel != null)
                    {
                        GrammaticalRelation reln = getGrammaticalRelationCommonAncestor(govCLabel, depCLabel,
                            basicGraph.getEdges(gov, dep).ToList());
                        // System.err.println("  Gov: " + gov + " Dep: " + dep + " Reln: " + reln);
                        basicDep.Add(new TypedDependency(reln, new IndexedWord(gov.headWordNode().label()),
                            new IndexedWord(dep.headWordNode().label())));
                    }
                    else
                    {
                        throw new SystemException("Should never be here");
                    }
                }
            }

            // add the root
            TreeGraphNode dependencyRoot = new TreeGraphNode(new Word("ROOT"));
            dependencyRoot.setIndex(0);
            TreeGraphNode rootDep = mroot().headWordNode();
            if (rootDep == null)
            {
                List<Tree> leaves = Trees.leaves(mroot());
                if (leaves.Count > 0)
                {
                    Tree leaf = leaves[0];
                    if (!(leaf is TreeGraphNode))
                    {
                        throw new InvalidDataException("Leaves should be TreeGraphNodes");
                    }
                    rootDep = (TreeGraphNode) leaf;
                    if (rootDep.headWordNode() != null)
                    {
                        rootDep = rootDep.headWordNode();
                    }
                }
            }
            if (rootDep != null)
            {
                TypedDependency rootTypedDep = new TypedDependency(GrammaticalRelation.ROOT,
                    new IndexedWord(dependencyRoot.label()), new IndexedWord(rootDep.label()));
                if (puncTypedDepFilter(rootTypedDep))
                {
                    basicDep.Add(rootTypedDep);
                }
            }

            postProcessDependencies(basicDep);

            basicDep.Sort();

            return basicDep;
        }

        /**
   * Returns a Filter which checks dependencies for usefulness as
   * extra tree-based dependencies.  By default, everything is
   * accepted.  One example of how this can be useful is in the
   * English dependencies, where the REL dependency is used as an
   * intermediate and we do not want this to be added when we make a
   * second pass over the trees for missing dependencies.
   */

        protected Predicate<TypedDependency> extraTreeDepFilter()
        {
            return Filters.AcceptFilter<TypedDependency>();
        }

        /**
   * Post process the dependencies in whatever way this language
   * requires.  For example, English might replace "rel" dependencies
   * with either dobj or pobj depending on the surrounding
   * dependencies.
   */

        protected void postProcessDependencies(List<TypedDependency> basicDep)
        {
            // no post processing by default
        }

        /**
   * Get extra dependencies that do not depend on the tree structure,
   * but rather only depend on the existing dependency structure.
   * For example, the English xsubj dependency can be extracted that way.
   */

        protected void getExtras(List<TypedDependency> basicDep)
        {
            // no extra dependencies by default
        }


        /** Look through the tree t and adds to the List basicDep
   *  additional dependencies which aren't
   *  in the List but which satisfy the filter puncTypedDepFilter.
   *
   * @param deps The list of dependencies which may be augmented
   * @param completeGraph a graph of all the tree dependencies found earlier
   * @param puncTypedDepFilter The filter that may skip punctuation dependencies
   * @param extraTreeDepFilter Additional dependencies are added only if they pass this filter
   */

        private static void getTreeDeps(List<TypedDependency> deps,
            DirectedMultiGraph<TreeGraphNode, GrammaticalRelation> completeGraph,
            Predicate<TypedDependency> puncTypedDepFilter,
            Predicate<TypedDependency> extraTreeDepFilter)
        {
            foreach (TreeGraphNode gov in completeGraph.getAllVertices())
            {
                foreach (TreeGraphNode dep in completeGraph.getChildren(gov))
                {
                    foreach (
                        GrammaticalRelation rel in
                            removeGrammaticalRelationAncestors(completeGraph.getEdges(gov, dep).ToList()))
                    {
                        TypedDependency newDep = new TypedDependency(rel, new IndexedWord(gov.headWordNode().label()),
                            new IndexedWord(dep.headWordNode().label()));
                        if (!deps.Contains(newDep) && puncTypedDepFilter(newDep) && extraTreeDepFilter(newDep))
                        {
                            newDep.setExtra();
                            deps.Add(newDep);
                        }
                    }
                }
            }
        }

        private /*static*/ class NoPunctFilter : IPredicate<Dependency<Label, Label, Object>> /*, Serializable*/
        {
            private Predicate<string> npf;

            public NoPunctFilter(Predicate<string> f)
            {
                this.npf = f;
            }

            //@Override
            public bool test(Dependency<Label, Label, Object> d)
            {
                if (d == null)
                {
                    return false;
                }
                Label lab = d.dependent();
                if (lab == null)
                {
                    return false;
                }
                return npf(lab.value());
            }

            // Automatically generated by Eclipse
            private static readonly long serialVersionUID = -2319891944796663180L;
        } // end static class NoPunctFilter

        public interface IPredicate<T>
        {
            bool test(T elt);
        }

        private /*static*/ class NoPunctTypedDependencyFilter : IPredicate<TypedDependency> /*, Serializable*/
        {
            private Predicate<string> npf;

            public NoPunctTypedDependencyFilter(Predicate<string> f)
            {
                this.npf = f;
            }

            //@Override
            public bool test(TypedDependency d)
            {
                if (d == null) return false;

                IndexedWord l = d.dep();
                if (l == null) return false;

                return npf(l.value());
            }

            // Automatically generated by Eclipse
            private static readonly long serialVersionUID = -2872766864289207468L;
        } // end static class NoPunctTypedDependencyFilter


        /**
   * Get GrammaticalRelation between gov and dep, and null if gov  is not the
   * governor of dep
   */

        public GrammaticalRelation getGrammaticalRelation(int govIndex, int depIndex)
        {
            TreeGraphNode gov = getNodeByIndex(govIndex);
            TreeGraphNode dep = getNodeByIndex(depIndex);
            // TODO: this is pretty ugly
            return getGrammaticalRelation(new IndexedWord(gov.label()), new IndexedWord(dep.label()));
        }

        /**
   * Get GrammaticalRelation between gov and dep, and null if gov is not the
   * governor of dep
   */

        public GrammaticalRelation getGrammaticalRelation(IndexedWord gov, IndexedWord dep)
        {
            List<GrammaticalRelation> labels = new List<GrammaticalRelation>();
            foreach (TypedDependency dependency in mtypedDependencies(true))
            {
                if (dependency.gov().Equals(gov) && dependency.dep().Equals(dep))
                {
                    labels.Add(dependency.reln());
                }
            }

            return getGrammaticalRelationCommonAncestor(gov, dep, labels);
        }

        /**
   * Returns the GrammaticalRelation which is the highest common
   * ancestor of the list of relations passed in.  The IndexedWords
   * are passed in only for debugging reasons.
   */

        private static GrammaticalRelation getGrammaticalRelationCommonAncestor(AbstractCoreLabel govH,
            AbstractCoreLabel depH, List<GrammaticalRelation> labels)
        {
            GrammaticalRelation reln = GrammaticalRelation.DEPENDENT;

            List<GrammaticalRelation> sortedLabels;
            if (labels.Count <= 1)
            {
                sortedLabels = labels;
            }
            else
            {
                sortedLabels = labels.ToList();
                sortedLabels.Sort(new NameComparator<GrammaticalRelation>());
            }
            // System.err.println(" gov " + govH + " dep " + depH + " arc labels: " + sortedLabels);

            foreach (GrammaticalRelation reln2 in sortedLabels)
            {
                if (reln.isAncestor(reln2))
                {
                    reln = reln2;
                } /* else if (PRINT_DEBUGGING && ! reln2.isAncestor(reln)) {
        System.err.println("@@@\t" + reln + "\t" + reln2 + "\t" +
                           govH[CoreAnnotations.ValueAnnotation.class) + "\t" + depH[CoreAnnotations.ValueAnnotation.class));
      }*/
            }
            /*if (PRINT_DEBUGGING && reln.Equals(GrammaticalRelation.DEPENDENT)) {
      string topCat = govH[CoreAnnotations.ValueAnnotation.class);
      string topTag = govH[TreeCoreAnnotations.HeadTagAnnotation.class).value();
      string topWord = govH[TreeCoreAnnotations.HeadWordAnnotation.class).value();
      string botCat = depH[CoreAnnotations.ValueAnnotation.class);
      string botTag = depH[TreeCoreAnnotations.HeadTagAnnotation.class).value();
      string botWord = depH[TreeCoreAnnotations.HeadWordAnnotation.class).value();
      System.err.println("### dep\t" + topCat + "\t" + topTag + "\t" + topWord +
                         "\t" + botCat + "\t" + botTag + "\t" + botWord + "\t");
    }*/
            return reln;
        }

        private static List<GrammaticalRelation> removeGrammaticalRelationAncestors(List<GrammaticalRelation> original)
        {
            List<GrammaticalRelation> filtered = new List<GrammaticalRelation>();
            foreach (GrammaticalRelation reln in original)
            {
                bool descendantFound = false;
                for (int index = 0; index < filtered.Count; ++index)
                {
                    GrammaticalRelation gr = filtered[index];
                    //if the element in the list is an ancestor of the current
                    //relation, remove it (we will replace it later)
                    if (gr.isAncestor(reln))
                    {
                        filtered.RemoveAt(index);
                        --index;
                    }
                    else if (reln.isAncestor(gr))
                    {
                        //if the relation is not an ancestor of an element in the
                        //list, we add the relation
                        descendantFound = true;
                    }
                }
                if (!descendantFound)
                {
                    filtered.Add(reln);
                }
            }
            return filtered;
        }


        /**
   * Returns the typed dependencies of this grammatical structure.  These
   * are the basic word-level typed dependencies, where each word is dependent
   * on one other thing, either a word or the starting ROOT, and the
   * dependencies have a tree structure.  This corresponds to the
   * command-line option "basicDependencies".
   *
   * @return The typed dependencies of this grammatical structure
   */

        public ICollection<TypedDependency> mtypedDependencies()
        {
            return mtypedDependencies(false);
        }


        /**
   * Returns all the typed dependencies of this grammatical structure.
   * These are like the basic (uncollapsed) dependencies, but may include
   * extra arcs for control relationships, etc. This corresponds to the
   * "nonCollapsed" option.
   */

        public ICollection<TypedDependency> mallTypedDependencies()
        {
            return mtypedDependencies(true);
        }


        /**
   * Returns the typed dependencies of this grammatical structure. These
   * are non-collapsed dependencies (basic or nonCollapsed).
   *
   * @param includeExtras If true, the list of typed dependencies
   * returned may include "extras", and does not follow a tree structure.
   * @return The typed dependencies of this grammatical structure
   */

        public List<TypedDependency> mtypedDependencies(bool includeExtras)
        {
            List<TypedDependency> deps;
            // This copy has to be done because of the broken way
            // TypedDependency objects can be mutated by downstream methods
            // such as collapseDependencies.  Without the copy here it is
            // possible for two consecutive calls to
            // typedDependenciesCollapsed to get different results.  For
            // example, the English dependencies rename existing objects KILL
            // to note that they should be removed.
            if (includeExtras)
            {
                deps = new List<TypedDependency>(allTypedDependencies.Count);
                foreach (TypedDependency dep in allTypedDependencies)
                {
                    deps.Add(new TypedDependency(dep));
                }
            }
            else
            {
                deps = new List<TypedDependency>(typedDependencies.Count);
                foreach (TypedDependency dep in typedDependencies)
                {
                    deps.Add(new TypedDependency(dep));
                }
            }
            correctDependencies(deps);
            return deps;
        }

        /**
   * Get the typed dependencies after collapsing them.
   * Collapsing dependencies refers to turning certain function words
   * such as prepositions and conjunctions into arcs, so they disappear from
   * the set of nodes.
   * There is no guarantee that the dependencies are a tree. While the
   * dependencies are normally tree-like, the collapsing may introduce
   * not only re-entrancies but even small cycles.
   *
   * @return A set of collapsed dependencies
   */

        public ICollection<TypedDependency> typedDependenciesCollapsed()
        {
            return typedDependenciesCollapsed(false);
        }

        // todo [cdm 2012]: The semantics of this method is the opposite of the others.
        // The other no argument methods correspond to includeExtras being
        // true, but for this one it is false.  This should probably be made uniform.
        /**
   * Get the typed dependencies after mostly collapsing them, but keep a tree
   * structure.  In order to do this, the code does:
   * <ol>
   * <li> no relative clause processing
   * <li> no xsubj relations
   * <li> no propagation of conjuncts
   * </ol>
   * This corresponds to the "tree" option.
   *
   * @return collapsed dependencies keeping a tree structure
   */

        public ICollection<TypedDependency> typedDependenciesCollapsedTree()
        {
            List<TypedDependency> tdl = mtypedDependencies(false);
            collapseDependenciesTree(tdl);
            return tdl;
        }

        /**
   * Get the typed dependencies after collapsing them.
   * The "collapsed" option corresponds to calling this method with argument
   * {@code true}.
   *
   * @param includeExtras If true, the list of typed dependencies
   * returned may include "extras", like controlling subjects
   * @return collapsed dependencies
   */

        public List<TypedDependency> typedDependenciesCollapsed(bool includeExtras)
        {
            List<TypedDependency> tdl = mtypedDependencies(includeExtras);
            collapseDependencies(tdl, false, includeExtras);
            return tdl;
        }


        /**
   * Get the typed dependencies after collapsing them and processing eventual
   * CC complements.  The effect of this part is to distributed conjoined
   * arguments across relations or conjoined predicates across their arguments.
   * This is generally useful, and we generally recommend using the output of
   * this method with the second argument being {@code true}.
   * The "CCPropagated" option corresponds to calling this method with an
   * argument of {@code true}.
   *
   * @param includeExtras If true, the list of typed dependencies
   * returned may include "extras", such as controlled subject links.
   * @return collapsed dependencies with CC processed
   */

        public List<TypedDependency> typedDependenciesCCprocessed(bool includeExtras)
        {
            List<TypedDependency> tdl = mtypedDependencies(includeExtras);
            collapseDependencies(tdl, true, includeExtras);
            return tdl;
        }


        /**
   * Get a list of the typed dependencies, including extras like control
   * dependencies, collapsing them and distributing relations across
   * coordination.  This method is generally recommended for best
   * representing the semantic and syntactic relations of a sentence. In
   * general it returns a directed graph (i.e., the output may not be a tree
   * and it may contain (small) cycles).
   * The "CCPropagated" option corresponds to calling this method.
   *
   * @return collapsed dependencies with CC processed
   */

        public List<TypedDependency> typedDependenciesCCprocessed()
        {
            return typedDependenciesCCprocessed(true);
        }


        /**
   * Destructively modify the <code>Collection&lt;TypedDependency&gt;</code> to collapse
   * language-dependent transitive dependencies.
   * <p/>
   * Default is no-op; to be over-ridden in subclasses.
   *
   * @param list A list of dependencies to process for possible collapsing
   * @param CCprocess apply CC process?
   */

        protected virtual void collapseDependencies(List<TypedDependency> list, bool CCprocess, bool includeExtras)
        {
            // do nothing as default operation
        }

        /**
   * Destructively modify the <code>Collection&lt;TypedDependency&gt;</code> to collapse
   * language-dependent transitive dependencies but keeping a tree structure.
   * <p/>
   * Default is no-op; to be over-ridden in subclasses.
   *
   * @param list A list of dependencies to process for possible collapsing
   *
   */

        protected virtual void collapseDependenciesTree(List<TypedDependency> list)
        {
            // do nothing as default operation
        }


        /**
   * Destructively modify the <code>TypedDependencyGraph</code> to correct
   * language-dependent dependencies. (e.g., nsubjpass in a relative clause)
   * <p/>
   * Default is no-op; to be over-ridden in subclasses.
   *
   */

        protected void correctDependencies(ICollection<TypedDependency> list)
        {
            // do nothing as default operation
        }


        /**
   * Checks if all the typeDependencies are connected
   * @param list a list of typedDependencies
   * @return true if the list represents a connected graph, false otherwise
   */

        public static bool isConnected(ICollection<TypedDependency> list)
        {
            return getRoots(list).Count <= 1; // there should be no more than one root to have a connected graph
            // there might be no root in the way we look when you have a relative clause
            // ex.: Apple is a society that sells computers
            // (the root "society" will also be the nsubj of "sells")
        }

        /**
   * Return a list of TypedDependencies which are not dependent on any node from the list.
   *
   * @param list The list of TypedDependencies to check
   * @return A list of TypedDependencies which are not dependent on any node from the list
   */

        public static ICollection<TypedDependency> getRoots(ICollection<TypedDependency> list)
        {

            ICollection<TypedDependency> roots = new List<TypedDependency>();

            // need to see if more than one governor is not listed somewhere as a dependent
            // first take all the deps
            ICollection<IndexedWord> deps = new HashSet<IndexedWord>();
            foreach (TypedDependency typedDep in list)
            {
                deps.Add(typedDep.dep());
            }

            // go through the list and add typedDependency for which the gov is not a dep
            ICollection<IndexedWord> govs = new HashSet<IndexedWord>();
            foreach (TypedDependency typedDep in list)
            {
                IndexedWord gov = typedDep.gov();
                if (!deps.Contains(gov) && !govs.Contains(gov))
                {
                    roots.Add(typedDep);
                }
                govs.Add(gov);
            }
            return roots;
        }

        private static readonly long serialVersionUID = 2286294455343892678L;

        private /*static*/ class NameComparator<X> : IComparer<X>
        {
            //@Override
            public int Compare(X o1, X o2)
            {
                string n1 = o1.ToString();
                string n2 = o2.ToString();
                return n1.CompareTo(n2);
            }
        }


        public static readonly string DEFAULT_PARSER_FILE = "/u/nlp/data/lexparser/englishPCFG.ser.gz";

        /**
   * Print typed dependencies in either the Stanford dependency representation
   * or in the conllx format.
   *
   * @param deps
   *          Typed dependencies to print
   * @param tree
   *          Tree corresponding to typed dependencies (only necessary if conllx
   *          == true)
   * @param conllx
   *          If true use conllx format, otherwise use Stanford representation
   * @param extraSep
   *          If true, in the Stanford representation, the extra dependencies
   *          (which do not preserve the tree structure) are printed after the
   *          basic dependencies
   */
        /*public static void printDependencies(GrammaticalStructure gs, Collection<TypedDependency> deps, Tree tree, bool conllx, bool extraSep) {
    System.out.println(dependenciesToString(gs, deps, tree, conllx, extraSep));
  }

  public static string dependenciesToString(GrammaticalStructure gs, Collection<TypedDependency> deps, Tree tree, bool conllx, bool extraSep) {
    StringBuilder bf = new StringBuilder();

    Map<Integer, Integer> indexToPos = Generics.newHashMap();
    indexToPos.put(0,0); // to deal with the special node "ROOT"
    List<Tree> gsLeaves = gs.root.getLeaves();
    for (int i = 0; i < gsLeaves.Count; i++) {
      TreeGraphNode leaf = (TreeGraphNode) gsLeaves[i);
      indexToPos.put(leaf.label.index(), i + 1);
    }

    if (conllx) {
      List<Tree> leaves = tree.getLeaves();
      string[] words = new string[leaves.Count];
      string[] pos = new string[leaves.Count];
      string[] relns = new string[leaves.Count];
      int[] govs = new int[leaves.Count];

      int index = 0;
      for (Tree leaf : leaves) {
        index++;
        if (!indexToPos.containsKey(index)) {
          continue;
        }
        int depPos = indexToPos[index) - 1;
        words[depPos] = leaf.value();
        pos[depPos] = leaf.parent(tree).value(); // use slow, but safe, parent look up
      }

      for (TypedDependency dep : deps) {
        int depPos = indexToPos[dep.dep().index()) - 1;
        govs[depPos] = indexToPos[dep.gov().index());
        relns[depPos] = dep.reln().ToString();
      }

      for (int i = 0; i < relns.Length; i++) {
        if (words[i] == null) {
          continue;
        }
        string out = string.format("%d\t%s\t_\t%s\t%s\t_\t%d\t%s\t_\t_\n", i + 1, words[i], pos[i], pos[i], govs[i], (relns[i] != null ? relns[i] : "erased"));
        bf.Append(out);
      }

    } else {
      if (extraSep) {
        List<TypedDependency> extraDeps = new List<TypedDependency>();
        for (TypedDependency dep : deps) {
          if (dep.extra()) {
            extraDeps.Add(dep);
          } else {
            bf.Append(toStringIndex(dep, indexToPos));
            bf.Append("\n");
          }
        }
        // now we print the separator for extra dependencies, and print these if
        // there are some
        if (!extraDeps.isEmpty()) {
          bf.Append("======\n");
          for (TypedDependency dep : extraDeps) {
            bf.Append(toStringIndex(dep, indexToPos));
            bf.Append("\n");
          }
        }
      } else {
        for (TypedDependency dep : deps) {
          bf.Append(toStringIndex(dep, indexToPos));
          bf.Append("\n");
        }
      }
    }

    return bf.ToString();
  }

  private static string toStringIndex(TypedDependency td, Map<Integer, Integer> indexToPos) {
    IndexedWord gov = td.gov();
    IndexedWord dep = td.dep();
    return td.reln() + "(" + gov.value() + "-" + indexToPos[gov.index()) + gov.toPrimes() + ", " + dep.value() + "-" + indexToPos[dep.index()) + dep.toPrimes() + ")";
  }*/


        // Note that these field constants are 0-based whereas much documentation is 1-based

        public static readonly int CoNLLX_WordField = 1;
        public static readonly int CoNLLX_POSField = 3;
        public static readonly int CoNLLX_GovField = 6;
        public static readonly int CoNLLX_RelnField = 7;

        public static readonly int CoNLLX_FieldCount = 10;

        /**
   * Read in a file containing a CoNLL-X dependency treebank and return a
   * corresponding list of GrammaticalStructures.
   *
   * @throws IOException
   */
        /*public static List<GrammaticalStructure> readCoNLLXGrammaticalStructureCollection(string fileName, Dictionary<string, GrammaticalRelation> shortNameToGRel, GrammaticalStructureFromDependenciesFactory factory) /*throws IOException#1# {
    LineNumberReader reader = new LineNumberReader(new FileReader(fileName));
    List<GrammaticalStructure> gsList = new List<GrammaticalStructure>();

    List<List<string>> tokenFields = new List<List<string>>();

    for (string inline = reader.readLine(); inline != null;
         inline = reader.readLine()) {
      if (!"".Equals(inline)) {
        // read in a single sentence token by token
        List<string> fields = Arrays.asList(inline.split("\t"));
        if (fields.Count != CoNLLX_FieldCount) {
          throw new SystemException(String.format("Error (line %d): 10 fields expected but %d are present", reader.getLineNumber(), fields.Count));
        }
        tokenFields.Add(fields);
      } else {
        if (tokenFields.isEmpty())
          continue; // skip excess empty lines

        gsList.Add(buildCoNLLXGrammaticalStructure(tokenFields, shortNameToGRel, factory));
        tokenFields = new List<List<string>>();
      }
    }

    return gsList;
  }

  public static GrammaticalStructure buildCoNLLXGrammaticalStructure(List<List<string>> tokenFields,
                                Dictionary<string, GrammaticalRelation> shortNameToGRel,
                                GrammaticalStructureFromDependenciesFactory factory) {
    List<IndexedWord> tgWords = new List<IndexedWord>(tokenFields.Count);
    List<TreeGraphNode> tgPOSNodes = new List<TreeGraphNode>(tokenFields.Count);

    SemanticHeadFinder headFinder = new SemanticHeadFinder();

    // Construct TreeGraphNodes for words and POS tags
    foreach (List<string> fields in tokenFields) {
      CoreLabel word = new CoreLabel();
      word.setValue(fields[CoNLLX_WordField]);
      word.setWord(fields[CoNLLX_WordField]);
      word.setTag(fields[CoNLLX_POSField]);
      word.setIndex(tgWords.Count + 1);
      CoreLabel pos = new CoreLabel();
      pos.setTag(fields[CoNLLX_POSField]);
      pos.setValue(fields[CoNLLX_POSField]);
      TreeGraphNode wordNode = new TreeGraphNode(word);
      TreeGraphNode posNode =new TreeGraphNode(pos);
      tgWords.Add(new IndexedWord(word));
      tgPOSNodes.Add(posNode);
      TreeGraphNode[] childArr = { wordNode };
      posNode.setChildren(childArr);
      wordNode.setParent(posNode);
      posNode.percolateHeads(headFinder);
    }

    // We fake up the parts of the tree structure that are not
    // actually used by the grammatical relation transformation
    // operations.
    //
    // That is, the constructed TreeGraphs consist of a flat tree,
    // without any phrase bracketing, but that does preserve the
    // parent child relationship between words and their POS tags.
    //
    // e.g. (ROOT (PRP I) (VBD hit) (DT the) (NN ball) (. .))

    TreeGraphNode root =
      new TreeGraphNode(new Word("ROOT-" + (tgPOSNodes.Count + 1)));
    root.setChildren(tgPOSNodes.ToArray());

    // Build list of TypedDependencies
    List<TypedDependency> tdeps = new List<TypedDependency>(tgWords.Count);

    // Create a node outside the tree useful for root dependencies;
    // we want to keep those if they were stored in the conll file
    
    CoreLabel rootLabel = new CoreLabel();
    rootLabel.setValue("ROOT");
    rootLabel.setWord("ROOT");
    rootLabel.setIndex(0);
    IndexedWord dependencyRoot = new IndexedWord(rootLabel);
    for (int i = 0; i < tgWords.Count; i++) {
      string parentIdStr = tokenFields[i][CoNLLX_GovField];
      if (parentIdStr == null || parentIdStr.Equals(""))
        continue;
      int parentId = int.Parse(parentIdStr) - 1;
      string grelString = tokenFields[i][CoNLLX_RelnField];
      if (grelString.Equals("null") || grelString.Equals("erased"))
        continue;
      GrammaticalRelation grel = shortNameToGRel[grelString.ToLower()];
      TypedDependency tdep;
      if (grel == null) {
        if (grelString.ToLower().Equals("root")) {
          tdep = new TypedDependency(GrammaticalRelation.ROOT, dependencyRoot, tgWords[i]);
        } else {
          throw new SystemException("Unknown grammatical relation '" +
                                     grelString + "' fields: " +
                                     tokenFields[i] + "\nNode: " +
                                     tgWords[i] + "\n" +
                                     "Known Grammatical relations: ["+shortNameToGRel.Keys+"]" );
        }
      } else {
        if (parentId >= tgWords.Count) {
          /*System.err.printf("Warning: Invalid Parent Id %d Sentence Length: %d%n", parentId+1, tgWords.Count);
          System.err.printf("         Assigning to root (0)%n");#1#
          parentId = -1;
        }
        tdep = new TypedDependency(grel, (parentId == -1 ? dependencyRoot : tgWords[parentId)),
                                   tgWords[i));
      }
      tdeps.Add(tdep);
    }
    return factory.build(tdeps, root);
  }


  private static string[] parseClassConstructArgs(string namePlusArgs) {
    string[] args = StringUtils.EMPTY_STRING_ARRAY;
    string name = namePlusArgs;
    if (namePlusArgs.matches(".*\\([^)]*\\)$")) {
      string argStr = namePlusArgs.replaceFirst("^.*\\(([^)]*)\\)$", "$1");
      args = argStr.split(",");
      name = namePlusArgs.replaceFirst("\\([^)]*\\)$", "");
    }
    string[] tokens = new string[1 + args.Length];
    tokens[0] = name;
    Array.Copy(args, 0, tokens, 1, args.Length);
    return tokens;
  }*/


        /*private static DependencyReader loadAlternateDependencyReader(string altDepReaderName) {
    DependencyReader altDepReaderClass = null;
    string[] toks = parseClassConstructArgs(altDepReaderName);
    altDepReaderName = toks[0];
    string[] depReaderArgs = new string[toks.Length - 1];
    Array.Copy(toks, 1, depReaderArgs, 0, toks.Length - 1);

    try {
      Class<?> cl = Class.forName(altDepReaderName);
      altDepReaderClass = cl.asSubclass(DependencyReader.class);
    } catch (ClassNotFoundException e) {
      // have a second go below
    }
    if (altDepReaderClass == null) {
      try {
        Class<?> cl = Class.forName("edu.stanford.nlp.trees." + altDepReaderName);
        altDepReaderClass = cl.asSubclass(DependencyReader.class);
      } catch (ClassNotFoundException e) {
          //
      }
    }
    if (altDepReaderClass == null) {
      //System.err.println("Can't load dependency reader " + altDepReaderName + " or edu.stanford.nlp.trees." + altDepReaderName);
      return null;
    }

    DependencyReader altDepReader; // initialized below
    if (depReaderArgs.Length == 0) {
      try {
        altDepReader = altDepReaderClass.newInstance();
      } catch (InstantiationException e) {
        throw new SystemException(e);
      } catch (IllegalAccessException e) {
        System.err.println("No argument constructor to " + altDepReaderName + " is not public");
        return null;
      }
    } else {
      try {
        altDepReader = altDepReaderClass.getConstructor(string[].class).newInstance((Object) depReaderArgs);
      } catch (IllegalArgumentException e) {
        throw new SystemException(e);
      } catch (SecurityException e) {
        throw new SystemException(e);
      } catch (InstantiationException e) {
        e.printStackTrace();
        return null;
      } catch (IllegalAccessException e) {
        System.err.println(depReaderArgs.Length + " argument constructor to " + altDepReaderName + " is not public.");
        return null;
      } catch (InvocationTargetException e) {
        throw new SystemException(e);
      } catch (NoSuchMethodException e) {
        System.err.println("String arguments constructor to " + altDepReaderName + " does not exist.");
        return null;
      }
    }
    return altDepReader;
  }


  private static DependencyPrinter loadAlternateDependencyPrinter(string altDepPrinterName) {
    Class<? extends DependencyPrinter> altDepPrinterClass = null;
    string[] toks = parseClassConstructArgs(altDepPrinterName);
    altDepPrinterName = toks[0];
    string[] depPrintArgs = new string[toks.Length - 1];
    Array.Copy(toks, 1, depPrintArgs, 0, toks.Length - 1);

    try {
      Class<?> cl = Class.forName(altDepPrinterName);
      altDepPrinterClass = cl.asSubclass(DependencyPrinter.class);
    } catch (ClassNotFoundException e) {
      //
    }
    if (altDepPrinterClass == null) {
      try {
        Class<?> cl = Class.forName("edu.stanford.nlp.trees." + altDepPrinterName);
        altDepPrinterClass = cl.asSubclass(DependencyPrinter.class);
      } catch (ClassNotFoundException e) {
        //
      }
    }
    if (altDepPrinterClass == null) {
      System.err.printf("Unable to load alternative printer %s or %s. Is your classpath set correctly?\n", altDepPrinterName, "edu.stanford.nlp.trees." + altDepPrinterName);
      return null;
    }
    try {
      DependencyPrinter depPrinter;
      if (depPrintArgs.Length == 0) {
        depPrinter = altDepPrinterClass.newInstance();
      } else {
        depPrinter = altDepPrinterClass.getConstructor(string[].class).newInstance((Object) depPrintArgs);
      }
      return depPrinter;
    } catch (IllegalArgumentException e) {
      e.printStackTrace();
      return null;
    } catch (SecurityException e) {
      e.printStackTrace();
      return null;
    } catch (InstantiationException e) {
      e.printStackTrace();
      return null;
    } catch (IllegalAccessException e) {
      e.printStackTrace();
      return null;
    } catch (InvocationTargetException e) {
      e.printStackTrace();
      return null;
    } catch (NoSuchMethodException e) {
      if (depPrintArgs == null) {
        System.err.printf("Can't find no-argument constructor %s().\n", altDepPrinterName);
      } else {
        System.err.printf("Can't find constructor %s(%s).\n", altDepPrinterName, Arrays.ToString(depPrintArgs));
      }
      return null;
    }
  }

  private static Function<List<? extends HasWord>, Tree> loadParser(string parserFile, string parserOptions, bool makeCopulaHead) {
    if (parserFile == null || "".Equals(parserFile)) {
      parserFile = DEFAULT_PARSER_FILE;
      if (parserOptions == null) {
        parserOptions = "-retainTmpSubcategories";
      }
    }
    if (parserOptions == null) {
      parserOptions = "";
    }
    if (makeCopulaHead) {
      parserOptions = "-makeCopulaHead " + parserOptions;
    }
    parserOptions = parserOptions.trim();
    // Load parser by reflection, so that this class doesn't require parser
    // for runtime use
    // LexicalizedParser lp = LexicalizedParser.loadModel(parserFile);
    // For example, the tregex package uses TreePrint, which uses
    // GrammaticalStructure, which would then import the
    // LexicalizedParser.  The tagger can read trees, which means it
    // would depend on tregex and therefore depend on the parser.
    Function<List<? extends HasWord>, Tree> lp;
    try {
      Class<?>[] classes = new Class<?>[] { string.class, string[].class };
      Method method = Class.forName("edu.stanford.nlp.parser.lexparser.LexicalizedParser").getMethod("loadModel", classes);
      string[] opts = {};
      if (parserOptions.Length > 0) {
        opts = parserOptions.split(" +");
      }
      lp = (Function<List<? extends HasWord>,Tree>) method.invoke(null, parserFile, opts);
    } catch (Exception cnfe) {
      throw new SystemException(cnfe);
    }
    return lp;
  }*/

        /**
   * Allow a collection of trees, that is a Treebank, appear to be a collection
   * of GrammaticalStructures.
   *
   * @author danielcer
   *
   */
        /*private static class TreeBankGrammaticalStructureWrapper: IEnumerable<GrammaticalStructure> {

    private readonly Iterable<Tree> trees;
    private readonly bool keepPunct;
    private readonly TreebankLangParserParams params;

    //private readonly Dictionary<GrammaticalStructure, Tree> origTrees = new WeakHashMap<GrammaticalStructure, Tree>();
    private readonly Dictionary<GrammaticalStructure, Tree> origTrees = new Dictionary<GrammaticalStructure, Tree>();

    public TreeBankGrammaticalStructureWrapper(Iterable<Tree> wrappedTrees, bool keepPunct, TreebankLangParserParams params) {
      trees = wrappedTrees;
      this.keepPunct = keepPunct;
      this.params = params;
    }

    //@Override
    public Iterator<GrammaticalStructure> iterator() {
      return new GsIterator();
    }

    public Tree getOriginalTree(GrammaticalStructure gs) {
      return origTrees[gs);
    }


    private class GsIterator : IEnumerator<GrammaticalStructure> {

      private readonly Iterator<Tree> tbIterator = trees.iterator();
      private readonly Predicate<string> puncFilter;
      private readonly HeadFinder hf;
      private GrammaticalStructure next;

      public GsIterator() {
        // TODO: this is very english specific
        if (keepPunct) {
          puncFilter = Filters.acceptFilter();
        } else {
          puncFilter = new PennTreebankLanguagePack().punctuationWordRejectFilter();
        }
        hf = params.typedDependencyHeadFinder();
        primeGs();
      }

      private void primeGs() {
        GrammaticalStructure gs = null;
        while (gs == null && tbIterator.hasNext()) {
          Tree t = tbIterator.next();
          // System.err.println("GsIterator: Next tree is");
          // System.err.println(t);
          if (t == null) {
            continue;
          }
          try {
            gs = params.getGrammaticalStructure(t, puncFilter, hf);
            origTrees.put(gs, t);
            next = gs;
            // System.err.println("GsIterator: Next tree is");
            // System.err.println(t);
            return;
          } catch (NullPointerException npe) {
            System.err.println("Bung tree caused below dump. Continuing....");
            System.err.println(t);
            npe.printStackTrace();
          }
        }
        next = null;
      }

      @Override
      public bool hasNext() {
        return next != null;
      }

      @Override
      public GrammaticalStructure next() {
        GrammaticalStructure ret = next;
        if (ret == null) {
          throw new NoSuchElementException();
        }
        primeGs();
        return ret;
      }

      @Override
      public void remove() {
        throw new UnsupportedOperationException();
      }

    }
  } // end static class TreebankGrammaticalStructureWrapper*/


        // todo [cdm 2013]: Take this out and make it a trees class: TreeIterableByParsing
        /*/*static#1# class LazyLoadTreesByParsing : IEnumerable<Tree> {
    readonly Reader reader;
    readonly string filename;
    readonly bool tokenized;
    readonly string encoding;
    readonly Function<List<? extends HasWord>, Tree> lp;

    public LazyLoadTreesByParsing(string filename, string encoding, bool tokenized, Function<List<? extends HasWord>, Tree> lp) {
      this.filename = filename;
      this.encoding = encoding;
      this.reader = null;
      this.tokenized = tokenized;
      this.lp = lp;
    }
    public LazyLoadTreesByParsing(Reader reader, bool tokenized, Function<List<? extends HasWord>, Tree> lp) {
      this.filename = null;
      this.encoding = null;
      this.reader = reader;
      this.tokenized = tokenized;
      this.lp = lp;
    }

    @Override
    public Iterator<Tree> iterator() {
      readonly BufferedReader iReader;
      if (reader != null) {
        iReader = new BufferedReader(reader);
      } else {
        try {
          iReader = new BufferedReader(new InputStreamReader(new FileInputStream(filename), encoding));
        } catch (IOException e) {
          throw new SystemException(e);
        }
      }

      return new Iterator<Tree>() {

        string line = null;

        @Override
        public bool hasNext() {
          if (line != null) {
            return true;
          } else {
            try {
              line = iReader.readLine();
            } catch (IOException e) {
              throw new SystemException(e);
            }
            if (line == null) {
              try {
                if (reader == null) iReader.close();
              } catch (Exception e) {
                throw new SystemException(e);
              }
              return false;
            }
            return true;
          }
        }

        @Override
        public Tree next() {
          if (line == null) {
            throw new NoSuchElementException();
          }
          Reader lineReader = new StringReader(line);
          line = null;
          List<Word> words;
          if (tokenized) {
            words = WhitespaceTokenizer.newWordWhitespaceTokenizer(lineReader).tokenize();
          } else {
            words = PTBTokenizer.newPTBTokenizer(lineReader).tokenize();
          }
          if (!words.isEmpty()) {
            // the parser throws an exception if told to parse an empty sentence.
            Tree parseTree = lp.apply(words);
            return parseTree;
          } else {
            return new SimpleTree();
          }
        }

        @Override
        public void remove() {
          throw new UnsupportedOperationException();
        }

      };
    }

  }*/


        // implementation after retrieving everytinh from java code ------------

        /*/**
   * Returns the typed dependencies of this grammatical structure.  These
   * are the basic word-level typed dependencies, where each word is dependent
   * on one other thing, either a word or the starting ROOT, and the
   * dependencies have a tree structure.  This corresponds to the
   * command-line option "basicDependencies".
   *
   * @return The typed dependencies of this grammatical structure
   #1#
        public List<TypedDependency> typedDependencies()
        {
            return typedDependencies(false);
        }


        /**
         * Returns all the typed dependencies of this grammatical structure.
         * These are like the basic (uncollapsed) dependencies, but may include
         * extra arcs for control relationships, etc. This corresponds to the
         * "nonCollapsed" option.
         #1#
        public List<TypedDependency> allTypedDependencies()
        {
            return typedDependencies(true);
        }


        /**
         * Returns the typed dependencies of this grammatical structure. These
         * are non-collapsed dependencies (basic or nonCollapsed).
         *
         * @param includeExtras If true, the list of typed dependencies
         * returned may include "extras", and does not follow a tree structure.
         * @return The typed dependencies of this grammatical structure
         #1#
        public List<TypedDependency> typedDependencies(bool includeExtras)
        {
            // This copy has to be done because of the broken way
            // TypedDependency objects can be mutated by downstream methods
            // such as collapseDependencies.  Without the copy here it is
            // possible for two consecutive calls to
            // typedDependenciesCollapsed to get different results.  For
            // example, the English dependencies rename existing objects KILL
            // to note that they should be removed.
            List<TypedDependency> deps = includeExtras ? allTypedDependencies().ToList() : typedDependencies().ToList();
            /*if (includeExtras) {
              deps = new List<TypedDependency>(allTypedDependencies.Count);
              for (TypedDependency dep : allTypedDependencies) {
                deps.Add(new TypedDependency(dep));
              }
            } else {
              deps = new List<TypedDependency>(typedDependencies.Count);
              for (TypedDependency dep : typedDependencies) {
                deps.Add(new TypedDependency(dep));
              }
            }#1#
            correctDependencies(deps);
            return deps;
          }

        /**
         * Get the typed dependencies after collapsing them.
         * Collapsing dependencies refers to turning certain function words
         * such as prepositions and conjunctions into arcs, so they disappear from
         * the set of nodes.
         * There is no guarantee that the dependencies are a tree. While the
         * dependencies are normally tree-like, the collapsing may introduce
         * not only re-entrancies but even small cycles.
         *
         * @return A set of collapsed dependencies
         #1#
        public List<TypedDependency> typedDependenciesCollapsed()
        {
            return typedDependenciesCollapsed(false);
        }

        // todo [cdm 2012]: The semantics of this method is the opposite of the others.
        // The other no argument methods correspond to includeExtras being
        // true, but for this one it is false.  This should probably be made uniform.
        /**
         * Get the typed dependencies after mostly collapsing them, but keep a tree
         * structure.  In order to do this, the code does:
         * <ol>
         * <li> no relative clause processing
         * <li> no xsubj relations
         * <li> no propagation of conjuncts
         * </ol>
         * This corresponds to the "tree" option.
         *
         * @return collapsed dependencies keeping a tree structure
         #1#
        public List<TypedDependency> typedDependenciesCollapsedTree()
        {
            List<TypedDependency> tdl = typedDependencies(false);
            collapseDependenciesTree(tdl);
            return tdl;
        }

        /**
         * Get the typed dependencies after collapsing them.
         * The "collapsed" option corresponds to calling this method with argument
         * {@code true}.
         *
         * @param includeExtras If true, the list of typed dependencies
         * returned may include "extras", like controlling subjects
         * @return collapsed dependencies
         #1#
        public List<TypedDependency> typedDependenciesCollapsed(bool includeExtras)
        {
            List<TypedDependency> tdl = typedDependencies(includeExtras);
            collapseDependencies(tdl, false, includeExtras);
            return tdl;
        }


        /**
         * Get the typed dependencies after collapsing them and processing eventual
         * CC complements.  The effect of this part is to distributed conjoined
         * arguments across relations or conjoined predicates across their arguments.
         * This is generally useful, and we generally recommend using the output of
         * this method with the second argument being {@code true}.
         * The "CCPropagated" option corresponds to calling this method with an
         * argument of {@code true}.
         *
         * @param includeExtras If true, the list of typed dependencies
         * returned may include "extras", such as controlled subject links.
         * @return collapsed dependencies with CC processed
         #1#
        public List<TypedDependency> typedDependenciesCCprocessed(bool includeExtras)
        {
            List<TypedDependency> tdl = typedDependencies(includeExtras);
            collapseDependencies(tdl, true, includeExtras);
            return tdl;
        }


        /**
         * Get a list of the typed dependencies, including extras like control
         * dependencies, collapsing them and distributing relations across
         * coordination.  This method is generally recommended for best
         * representing the semantic and syntactic relations of a sentence. In
         * general it returns a directed graph (i.e., the output may not be a tree
         * and it may contain (small) cycles).
         * The "CCPropagated" option corresponds to calling this method.
         *
         * @return collapsed dependencies with CC processed
         #1#
        public List<TypedDependency> typedDependenciesCCprocessed()
        {
            return typedDependenciesCCprocessed(true);
        }

        /**
  * Destructively modify the <code>Collection&lt;TypedDependency&gt;</code> to collapse
  * language-dependent transitive dependencies.
  * <p/>
  * Default is no-op; to be over-ridden in subclasses.
  *
  * @param list A list of dependencies to process for possible collapsing
  * @param CCprocess apply CC process?
  #1#
        protected virtual void collapseDependencies(List<TypedDependency> list, bool CCprocess, bool includeExtras)
        {
            // do nothing as default operation
        }

        /**
         * Destructively modify the <code>Collection&lt;TypedDependency&gt;</code> to collapse
         * language-dependent transitive dependencies but keeping a tree structure.
         * <p/>
         * Default is no-op; to be over-ridden in subclasses.
         *
         * @param list A list of dependencies to process for possible collapsing
         *
         #1#
        protected virtual void collapseDependenciesTree(List<TypedDependency> list)
        {
            // do nothing as default operation
        }


        /**
         * Destructively modify the <code>TypedDependencyGraph</code> to correct
         * language-dependent dependencies. (e.g., nsubjpass in a relative clause)
         * <p/>
         * Default is no-op; to be over-ridden in subclasses.
         *
         #1#
        protected void correctDependencies(List<TypedDependency> list)
        {
            // do nothing as default operation
        }*/

    }
}