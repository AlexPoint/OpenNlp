using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees
{
    [Serializable]
    public abstract class GrammaticalStructure
    {

        /**
   * Returns the typed dependencies of this grammatical structure.  These
   * are the basic word-level typed dependencies, where each word is dependent
   * on one other thing, either a word or the starting ROOT, and the
   * dependencies have a tree structure.  This corresponds to the
   * command-line option "basicDependencies".
   *
   * @return The typed dependencies of this grammatical structure
   */
        public List<TypedDependency> typedDependencies()
        {
            return typedDependencies(false);
        }


        /**
         * Returns all the typed dependencies of this grammatical structure.
         * These are like the basic (uncollapsed) dependencies, but may include
         * extra arcs for control relationships, etc. This corresponds to the
         * "nonCollapsed" option.
         */
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
         */
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
              deps = new List<TypedDependency>(allTypedDependencies.size());
              for (TypedDependency dep : allTypedDependencies) {
                deps.add(new TypedDependency(dep));
              }
            } else {
              deps = new List<TypedDependency>(typedDependencies.size());
              for (TypedDependency dep : typedDependencies) {
                deps.add(new TypedDependency(dep));
              }
            }*/
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
         */
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
         */
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
         */
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
        protected void correctDependencies(List<TypedDependency> list)
        {
            // do nothing as default operation
        }

    }
}
