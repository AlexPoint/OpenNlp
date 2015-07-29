using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Ling;
using OpenNLP.Tools.Util;

namespace OpenNLP.Tools.Trees
{
    /// <summary>
    /// A GrammaticalStructure for English.
    /// 
    /// The Stanford parser should be run with the "-retainNPTmpSubcategories"
    /// option! <b>Caveat emptor!</b> This is a work in progress. Suggestions welcome.
    /// 
    /// @author Bill MacCartney
    /// @author Marie-Catherine de Marneffe
    /// @author Christopher Manning
    /// @author Daniel Cer (CoNLLX format and alternative user selected dependency printer/reader interface)
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public class EnglishGrammaticalStructure : GrammaticalStructure
    {
        /// <summary>
        /// Construct a new <code>GrammaticalStructure</code> from an existing parse tree.
        /// The new <code>GrammaticalStructure</code> has the same tree structure
        /// and label values as the given tree (but no shared storage). As part of
        /// construction, the parse tree is analyzed using definitions from
        /// {@link GrammaticalRelation <code>GrammaticalRelation</code>} to populate
        /// the new <code>GrammaticalStructure</code> with as many labeled grammatical relations as it can.
        /// </summary>
        /// <param name="t">Parse tree to make grammatical structure from</param>
        public EnglishGrammaticalStructure(Tree t) :
            this(t, new PennTreebankLanguagePack().PunctuationWordRejectFilter())
        {
        }

        /// <summary>
        /// This gets used by GrammaticalStructureFactory (by reflection). DON'T DELETE.
        /// </summary>
        /// <param name="t">Parse tree to make grammatical structure from</param>
        /// <param name="puncFilter">Filter to remove punctuation dependencies</param>
        public EnglishGrammaticalStructure(Tree t, Predicate<string> puncFilter) :
            this(t, puncFilter, new SemanticHeadFinder(true), true)
        {
        }

        /// <summary>
        /// This gets used by GrammaticalStructureFactory (by reflection). DON'T DELETE.
        /// </summary>
        /// <param name="t">Parse tree to make grammatical structure from</param>
        /// <param name="puncFilter">Filter to remove punctuation dependencies</param>
        /// <param name="hf">HeadFinder to use when building it</param>
        public EnglishGrammaticalStructure(Tree t, Predicate<string> puncFilter, IHeadFinder hf) :
            this(t, puncFilter, hf, true)
        {
        }

        /// <summary>
        /// Construct a new <code>GrammaticalStructure</code> from an existing parse tree.
        /// The new <code>GrammaticalStructure</code> has the same tree structure
        /// and label values as the given tree (but no shared storage). As part of
        /// construction, the parse tree is analyzed using definitions from
        /// {@link GrammaticalRelation <code>GrammaticalRelation</code>} to populate
        /// the new <code>GrammaticalStructure</code> with as many labeled grammatical
        /// relations as it can.
        /// </summary>
        /// <param name="t">Parse tree to make grammatical structure from</param>
        /// <param name="puncFilter">Filter for punctuation words</param>
        /// <param name="hf">HeadFinder to use when building it</param>
        /// <param name="threadSafe">Whether or not to support simultaneous instances among multiple threads</param>
        public EnglishGrammaticalStructure(Tree t, Predicate<string> puncFilter, IHeadFinder hf, bool threadSafe) :
            base((new CoordinationTransformer(hf)).TransformTree(t.DeepCopy()),
                EnglishGrammaticalRelations.Values(threadSafe),
                threadSafe ? EnglishGrammaticalRelations.valuesLock : null,
                hf, puncFilter)
        {
            // the tree is normalized (for index and functional tag stripping) inside CoordinationTransformer
        }

        /** Used for postprocessing CoNLL X dependencies */
        /*public EnglishGrammaticalStructure(List<TypedDependency> projectiveDependencies, TreeGraphNode root)
        {
            super(projectiveDependencies, root);
        }*/

        /// <summary>
        /// Destructively modifies this <code>Collection{TypedDependency}</code>
        /// by collapsing several types of transitive pairs of dependencies.
        /// If called with a tree of dependencies and both CCprocess and
        /// includeExtras set to false, then the tree structure is preserved.
        /// <dl>
        /// <dt>prepositional object dependencies: pobj</dt>
        /// <dd><code>prep(cat, in)</code> and <code>pobj(in, hat)</code> are collapsed to
        /// <code>prep_in(cat, hat)</code></dd>
        /// <dt>prepositional complement dependencies: pcomp</dt>
        /// <dd>
        /// <code>prep(heard, of)</code> and <code>pcomp(of, attacking)</code> are
        /// collapsed to <code>prepc_of(heard, attacking)</code></dd>
        /// <dt>conjunct dependencies</dt>
        /// <dd>
        /// <code>cc(investors, and)</code> and
        /// <code>conj(investors, regulators)</code> are collapsed to
        /// <code>conj_and(investors,regulators)</code></dd>
        /// <dt>possessive dependencies: possessive</dt>
        /// <dd>
        /// <code>possessive(Montezuma, 's)</code> will be erased. This is like a collapsing, but
        /// due to the flatness of NPs, two dependencies are not actually composed.</dd>
        /// <dt>For relative clauses, it will collapse referent</dt>
        /// <dd>
        /// <code>ref(man, that)</code> and <code>dobj(love, that)</code> are collapsed
        /// to <code>dobj(love, man)</code></dd>
        /// </dl>
        /// </summary>
        protected override void CollapseDependencies(List<TypedDependency> list, bool cCprocess, bool includeExtras)
        {
            CorrectDependencies(list);

            EraseMultiConj(list);

            Collapse2Wp(list);

            CollapseFlatMwp(list);

            Collapse2WpBis(list);

            Collapse3Wp(list);

            CollapsePrepAndPoss(list);

            CollapseConj(list);

            if (includeExtras)
            {
                AddRef(list);

                CollapseReferent(list);
            }

            if (cCprocess)
            {
                TreatCc(list);
            }

            if (includeExtras)
            {
                AddExtraNSubj(list);

                CorrectSubjPass(list);
            }

            RemoveDep(list);
            list.Sort();
        }

        /*
        *
        * While upgrading, here are some lists of common multiword prepositions which
        * we might try to cover better. (Also do corpus counts for same?)
        *
        * (Prague Dependency Treebank) as per CRIT except for RESTR but for RESTR
        * apart from RESTR away from RESTR aside from RESTR as from TSIN ahead of
        * TWHEN back of LOC, DIR3 exclusive of* RESTR instead of SUBS outside of LOC,
        * DIR3 off of DIR1 upwards of LOC, DIR3 as of TSIN because of CAUS inside of
        * LOC, DIR3 irrespective of REG out of LOC, DIR1 regardless of REG according
        * to CRIT due to CAUS next to LOC, RESTR owing to* CAUS preparatory to* TWHEN
        * prior to* TWHEN subsequent to* TWHEN as to/for REG contrary to* CPR close
        * to* LOC, EXT (except the case named in the next table) near to LOC, DIR3
        * nearer to LOC, DIR3 preliminary to* TWHEN previous to* TWHEN pursuant to*
        * CRIT thanks to CAUS along with ACMP together with ACMP devoid of* ACMP void
        * of* ACMP
        *
        * http://www.keepandshare.com/doc/view.php?u=13166
        *
        * according to ahead of as far as as well as by means of due to far from in
        * addition to in case of in front of in place of in spite of inside of
        * instead of in to (into) near to next to on account of on behalf of on top
        * of on to (onto) out of outside of owing to prior to with regards to
        *
        * www.eslmonkeys.com/book/learner/prepositions.pdf According to Ahead of
        * Along with Apart from As for As to Aside from Because of But for Contrary
        * to Except for Instead of Next to Out of Prior to Thanks to
        */
        /// <summary>
        /// Collapse multi-words preposition of the following format, which comes from
        /// flat annotation. This handles e.g., "because of" (PP (IN because) (IN of)
        /// ...), "such as" (PP (JJ such) (IN as) ...)
        /// prep(gov, mwp[1]) dep(mpw[1], mwp[0]) pobj(mwp[1], compl) ->
        /// prep_mwp[0]_mwp[1](gov, compl)
        /// </summary>
        /// <param name="list">List of typedDependencies to work on</param>
        private static void CollapseFlatMwp(List<TypedDependency> list)
        {
            var newTypedDeps = new List<TypedDependency>();

            foreach (string[] mwp in MultiwordPreps)
            {
                newTypedDeps.Clear();

                IndexedWord mwp1 = null;
                IndexedWord governor = null;

                TypedDependency prep = null;
                TypedDependency dep = null;
                TypedDependency pobj = null;

                // first find the multi_preposition: dep(mpw[1], mwp[0])
                foreach (TypedDependency td in list)
                {
                    if (Math.Abs(td.Gov.Index() - td.Dep.Index()) == 1 &&
                        td.Gov.Value().Equals(mwp[1], StringComparison.InvariantCultureIgnoreCase)
                        && td.Dep.Value().Equals(mwp[0], StringComparison.InvariantCultureIgnoreCase))
                    {
                        mwp1 = td.Gov;
                        dep = td;
                    }
                }

                if (mwp1 == null)
                {
                    continue;
                }

                // now search for prep(gov, mwp1)
                foreach (TypedDependency td1 in list)
                {
                    if (td1.Dep.Equals(mwp1) && td1.Reln == EnglishGrammaticalRelations.PrepositionalModifier)
                    {
                        // we found prep(gov, mwp1)
                        prep = td1;
                        governor = prep.Gov;
                    }
                }

                if (prep == null)
                {
                    continue;
                }

                // search for the complement: pobj|pcomp(mwp1,X)
                foreach (TypedDependency td2 in list)
                {
                    if (td2.Gov.Equals(mwp1) && td2.Reln == EnglishGrammaticalRelations.PrepositionalObject)
                    {
                        pobj = td2;
                        // create the new gr relation
                        GrammaticalRelation gr = EnglishGrammaticalRelations.GetPrep(mwp[0] + '_' + mwp[1]);
                        newTypedDeps.Add(new TypedDependency(gr, governor, pobj.Dep));
                    }
                    if (td2.Gov.Equals(mwp1) && td2.Reln == EnglishGrammaticalRelations.PrepositionalComplement)
                    {
                        pobj = td2;
                        // create the new gr relation
                        GrammaticalRelation gr = EnglishGrammaticalRelations.GetPrepC(mwp[0] + '_' + mwp[1]);
                        newTypedDeps.Add(new TypedDependency(gr, governor, pobj.Dep));
                    }
                }

                if (pobj == null)
                {
                    return;
                }
                // only if we found the three parts, set to KILL and remove
                // we know prep != null && dep != null && dep != null
                prep.Reln = GrammaticalRelation.Kill;
                dep.Reln = GrammaticalRelation.Kill;
                pobj.Reln = GrammaticalRelation.Kill;

                // now remove typed dependencies with reln "kill"
                // and promote possible orphans
                foreach (TypedDependency td1 in list)
                {
                    if (td1.Reln != GrammaticalRelation.Kill)
                    {
                        if (td1.Gov.Equals(mwp1))
                        {
                            td1.Gov = governor;
                        }
                        if (!newTypedDeps.Contains(td1))
                        {
                            newTypedDeps.Add(td1);
                        }
                    }
                }
                list.Clear();
                list.AddRange(newTypedDeps);
            }
        }

        /// <summary>
        /// Collapse multi-words preposition of the following format: advmod|prt(gov,
        /// mwp[0]) prep(gov,mwp[1]) pobj|pcomp(mwp[1], compl) ->
        /// prep_mwp[0]_mwp[1](gov, compl)
        /// </summary>
        /// <param name="list">List of typedDependencies to work on</param>
        private static void Collapse2WpBis(List<TypedDependency> list)
        {
            var newTypedDeps = new List<TypedDependency>();

            foreach (string[] mwp in MultiwordPreps)
            {
                newTypedDeps.Clear();

                IndexedWord mwp0 = null;
                IndexedWord mwp1 = null;
                IndexedWord governor = null;

                TypedDependency prep = null;
                TypedDependency dep = null;
                TypedDependency pobj = null;
                TypedDependency newtd = null;

                // first find the first part of the multi_preposition: advmod|prt(gov, mwp[0])

                foreach (TypedDependency td in list)
                {
                    if (td.Dep.Value().Equals(mwp[0], StringComparison.InvariantCultureIgnoreCase)
                        &&
                        (td.Reln == EnglishGrammaticalRelations.PhrasalVerbParticle ||
                         td.Reln == EnglishGrammaticalRelations.AdverbialModifier
                         || td.Reln == GrammaticalRelation.Dependent ||
                         td.Reln == EnglishGrammaticalRelations.MultiWordExpression))
                    {
                        // we found advmod(gov, mwp0) or prt(gov, mwp0)
                        governor = td.Gov;
                        mwp0 = td.Dep;
                        dep = td;
                    }
                }

                // now search for the second part: prep(gov, mwp1)
                // the two words in the mwp should be next to another in the sentence
                // (difference of indexes = 1)

                if (mwp0 == null || governor == null)
                {
                    continue;
                }

                foreach (TypedDependency td1 in list)
                {
                    if (td1.Reln == EnglishGrammaticalRelations.PrepositionalModifier
                        && td1.Dep.Value().Equals(mwp[1], StringComparison.InvariantCultureIgnoreCase)
                        && Math.Abs(td1.Dep.Index() - mwp0.Index()) == 1 && td1.Gov.Equals(governor))
                    {
// we
                        // found
                        // prep(gov,
                        // mwp1)
                        mwp1 = td1.Dep;
                        prep = td1;
                    }
                }

                if (mwp1 == null)
                {
                    continue;
                }

                // search for the complement: pobj|pcomp(mwp1,X)
                foreach (TypedDependency td2 in list)
                {
                    if (td2.Reln == EnglishGrammaticalRelations.PrepositionalObject && td2.Gov.Equals(mwp1))
                    {
                        pobj = td2;
                        // create the new gr relation
                        GrammaticalRelation gr = EnglishGrammaticalRelations.GetPrep(mwp[0] + '_' + mwp[1]);
                        newtd = new TypedDependency(gr, governor, pobj.Dep);
                    }
                    if (td2.Reln == EnglishGrammaticalRelations.PrepositionalComplement && td2.Gov.Equals(mwp1))
                    {
                        pobj = td2;
                        // create the new gr relation
                        GrammaticalRelation gr = EnglishGrammaticalRelations.GetPrepC(mwp[0] + '_' + mwp[1]);
                        newtd = new TypedDependency(gr, governor, pobj.Dep);
                    }
                }

                if (pobj == null)
                {
                    return;
                }

                // only if we found the three parts, set to KILL and remove
                // and add the new one
                // now prep != null, pobj != null and newtd != null

                prep.Reln = GrammaticalRelation.Kill;
                dep.Reln = GrammaticalRelation.Kill;
                pobj.Reln = GrammaticalRelation.Kill;
                newTypedDeps.Add(newtd);

                // now remove typed dependencies with reln "kill"
                // and promote possible orphans
                foreach (TypedDependency td1 in list)
                {
                    if (td1.Reln != GrammaticalRelation.Kill)
                    {
                        if (td1.Gov.Equals(mwp0) || td1.Gov.Equals(mwp1))
                        {
                            td1.Gov = governor;
                        }
                        if (!newTypedDeps.Contains(td1))
                        {
                            newTypedDeps.Add(td1);
                        }
                    }
                }
                list.Clear();
                list.AddRange(newTypedDeps);
            }
        }

        /// <summary>
        /// Collapse 3-word preposition of the following format:
        /// This will be the case when the preposition is analyzed as a NP
        /// prep(gov, mwp0) 
        /// X(mwp0,mwp1)
        /// X(mwp1,mwp2)
        /// pobj|pcomp(mwp2, compl)
        /// -> prep_mwp[0]_mwp[1]_mwp[2](gov, compl)
        /// 
        /// It also takes flat annotation into account:
        /// prep(gov,mwp0)
        /// X(mwp0,mwp1)
        /// X(mwp0,mwp2)
        /// pobj|pcomp(mwp0, compl)
        /// -> prep_mwp[0]_mwp[1]_mwp[2](gov, compl)
        /// </summary>
        /// <param name="list">List of typedDependencies to work on</param>
        private static void Collapse3Wp(List<TypedDependency> list)
        {
            var newTypedDeps = new List<TypedDependency>();

            // first, loop over the prepositions for NP annotation
            foreach (string[] mwp in ThreewordPreps)
            {
                newTypedDeps.Clear();

                IndexedWord mwp0 = null;
                IndexedWord mwp1 = null;
                IndexedWord mwp2 = null;

                TypedDependency dep1 = null;
                TypedDependency dep2 = null;

                // first find the first part of the 3word preposition: dep(mpw[0], mwp[1])
                // the two words should be next to another in the sentence (difference of
                // indexes = 1)

                foreach (TypedDependency td in list)
                {
                    if (td.Gov.Value().Equals(mwp[0], StringComparison.InvariantCultureIgnoreCase)
                        && td.Dep.Value().Equals(mwp[1], StringComparison.InvariantCultureIgnoreCase)
                        && Math.Abs(td.Gov.Index() - td.Dep.Index()) == 1)
                    {
                        mwp0 = td.Gov;
                        mwp1 = td.Dep;
                        dep1 = td;
                    }
                }

                // find the second part of the 3word preposition: dep(mpw[1], mwp[2])
                // the two words should be next to another in the sentence (difference of
                // indexes = 1)

                foreach (TypedDependency td in list)
                {
                    if (td.Gov.Equals(mwp1) &&
                        td.Dep.Value().Equals(mwp[2], StringComparison.InvariantCultureIgnoreCase)
                        && Math.Abs(td.Gov.Index() - td.Dep.Index()) == 1)
                    {
                        mwp2 = td.Dep;
                        dep2 = td;
                    }
                }

                if (dep1 != null && dep2 != null)
                {

                    // now search for prep(gov, mwp0)
                    IndexedWord governor = null;
                    TypedDependency prep = null;
                    foreach (TypedDependency td1 in list)
                    {
                        if (td1.Reln == EnglishGrammaticalRelations.PrepositionalModifier && td1.Dep.Equals(mwp0))
                        {
// we
                            // found
                            // prep(gov,
                            // mwp0)
                            prep = td1;
                            governor = prep.Gov;
                        }
                    }

                    // search for the complement: pobj|pcomp(mwp2,X)

                    TypedDependency pobj = null;
                    TypedDependency newtd = null;
                    foreach (TypedDependency td2 in list)
                    {
                        if (td2.Reln == EnglishGrammaticalRelations.PrepositionalObject && td2.Gov.Equals(mwp2))
                        {
                            pobj = td2;
                            // create the new gr relation
                            GrammaticalRelation gr =
                                EnglishGrammaticalRelations.GetPrep(mwp[0] + '_' + mwp[1] + '_' + mwp[2]);
                            if (governor != null)
                            {
                                newtd = new TypedDependency(gr, governor, pobj.Dep);
                            }
                        }
                        if (td2.Reln == EnglishGrammaticalRelations.PrepositionalComplement && td2.Gov.Equals(mwp2))
                        {
                            pobj = td2;
                            // create the new gr relation
                            GrammaticalRelation gr =
                                EnglishGrammaticalRelations.GetPrepC(mwp[0] + '_' + mwp[1] + '_' + mwp[2]);
                            if (governor != null)
                            {
                                newtd = new TypedDependency(gr, governor, pobj.Dep);
                            }
                        }
                    }

                    // only if we found the governor and complement parts, set to KILL and
                    // remove
                    // and add the new one
                    if (prep != null && pobj != null && newtd != null)
                    {
                        prep.Reln = GrammaticalRelation.Kill;
                        dep1.Reln = GrammaticalRelation.Kill;
                        dep2.Reln = GrammaticalRelation.Kill;
                        pobj.Reln = GrammaticalRelation.Kill;
                        newTypedDeps.Add(newtd);

                        // now remove typed dependencies with reln "kill"
                        // and promote possible orphans
                        foreach (TypedDependency td1 in list)
                        {
                            if (td1.Reln != GrammaticalRelation.Kill)
                            {
                                if (td1.Gov.Equals(mwp0) || td1.Gov.Equals(mwp1) || td1.Gov.Equals(mwp2))
                                {
                                    td1.Gov = governor;
                                }
                                if (!newTypedDeps.Contains(td1))
                                {
                                    newTypedDeps.Add(td1);
                                }
                            }
                        }
                        list.Clear();
                        list.AddRange(newTypedDeps);
                    }
                }
            }

            // second, loop again looking at flat annotation
            foreach (string[] mwp in ThreewordPreps)
            {
                newTypedDeps.Clear();

                IndexedWord mwp0 = null;
                IndexedWord mwp1 = null;
                IndexedWord mwp2 = null;

                TypedDependency dep1 = null;
                TypedDependency dep2 = null;

                // first find the first part of the 3word preposition: dep(mpw[0], mwp[1])
                // the two words should be next to another in the sentence (difference of
                // indexes = 1)
                foreach (TypedDependency td in list)
                {
                    if (td.Gov.Value().Equals(mwp[0], StringComparison.InvariantCultureIgnoreCase)
                        && td.Dep.Value().Equals(mwp[1], StringComparison.InvariantCultureIgnoreCase)
                        && Math.Abs(td.Gov.Index() - td.Dep.Index()) == 1)
                    {
                        mwp0 = td.Gov;
                        mwp1 = td.Dep;
                        dep1 = td;
                    }
                }

                // find the second part of the 3word preposition: dep(mpw[0], mwp[2])
                // the two words should be one word apart in the sentence (difference of
                // indexes = 2)
                foreach (TypedDependency td in list)
                {
                    if (td.Gov.Equals(mwp0) &&
                        td.Dep.Value().Equals(mwp[2], StringComparison.InvariantCultureIgnoreCase)
                        && Math.Abs(td.Gov.Index() - td.Dep.Index()) == 2)
                    {
                        mwp2 = td.Dep;
                        dep2 = td;
                    }
                }

                if (dep1 != null && dep2 != null)
                {

                    // now search for prep(gov, mwp0)
                    IndexedWord governor = null;
                    TypedDependency prep = null;
                    foreach (TypedDependency td1 in list)
                    {
                        if (td1.Dep.Equals(mwp0) && td1.Reln == EnglishGrammaticalRelations.PrepositionalModifier)
                        {
// we
                            // found
                            // prep(gov,
                            // mwp0)
                            prep = td1;
                            governor = prep.Gov;
                        }
                    }

                    // search for the complement: pobj|pcomp(mwp0,X)

                    TypedDependency pobj = null;
                    TypedDependency newtd = null;
                    foreach (TypedDependency td2 in list)
                    {
                        if (td2.Gov.Equals(mwp0) && td2.Reln == EnglishGrammaticalRelations.PrepositionalObject)
                        {
                            pobj = td2;
                            // create the new gr relation
                            GrammaticalRelation gr =
                                EnglishGrammaticalRelations.GetPrep(mwp[0] + '_' + mwp[1] + '_' + mwp[2]);
                            if (governor != null)
                            {
                                newtd = new TypedDependency(gr, governor, pobj.Dep);
                            }
                        }
                        if (td2.Gov.Equals(mwp0) && td2.Reln == EnglishGrammaticalRelations.PrepositionalComplement)
                        {
                            pobj = td2;
                            // create the new gr relation
                            GrammaticalRelation gr =
                                EnglishGrammaticalRelations.GetPrepC(mwp[0] + '_' + mwp[1] + '_' + mwp[2]);
                            if (governor != null)
                            {
                                newtd = new TypedDependency(gr, governor, pobj.Dep);
                            }
                        }
                    }

                    // only if we found the governor and complement parts, set to KILL and
                    // remove
                    // and add the new one
                    if (prep != null && pobj != null && newtd != null)
                    {
                        prep.Reln = GrammaticalRelation.Kill;
                        dep1.Reln = GrammaticalRelation.Kill;
                        dep2.Reln = GrammaticalRelation.Kill;
                        pobj.Reln = GrammaticalRelation.Kill;
                        newTypedDeps.Add(newtd);

                        // now remove typed dependencies with reln "kill"
                        // and promote possible orphans
                        foreach (TypedDependency td1 in list)
                        {
                            if (td1.Reln != GrammaticalRelation.Kill)
                            {
                                if (td1.Gov.Equals(mwp0) || td1.Gov.Equals(mwp1) || td1.Gov.Equals(mwp2))
                                {
                                    td1.Gov = governor;
                                }
                                if (!newTypedDeps.Contains(td1))
                                {
                                    newTypedDeps.Add(td1);
                                }
                            }
                        }
                        list.Clear();
                        list.AddRange(newTypedDeps);
                    }
                }
            }
        }

        private static void CollapsePrepAndPoss(List<TypedDependency> list)
        {

            // Man oh man, how gnarly is the logic of this method....
            var newTypedDeps = new List<TypedDependency>();

            // Construct a map from tree nodes to the set of typed
            // dependencies in which the node appears as governor.
            // cdm: could use CollectionValuedMap here!
            var map = new Dictionary<IndexedWord, Util.SortedSet<TypedDependency>>();
            var vmod = new List<IndexedWord>();

            foreach (TypedDependency typedDep in list)
            {
                if (!map.ContainsKey(typedDep.Gov))
                {
                    map.Add(typedDep.Gov, new TreeSet<TypedDependency>());
                }
                map[typedDep.Gov].Add(typedDep);

                if (typedDep.Reln == EnglishGrammaticalRelations.VerbalModifier)
                {
                    // look for aux deps which indicate this was a to-be verb
                    bool foundAux = false;
                    foreach (TypedDependency auxDep in list)
                    {
                        if (auxDep.Reln != EnglishGrammaticalRelations.AuxModifier)
                        {
                            continue;
                        }
                        if (!auxDep.Gov.Equals(typedDep.Dep) ||
                            !auxDep.Dep.Value().Equals("to", StringComparison.InvariantCultureIgnoreCase))
                        {
                            continue;
                        }
                        foundAux = true;
                        break;
                    }
                    if (!foundAux)
                    {
                        vmod.Add(typedDep.Dep);
                    }
                }
            }

            // Do preposition conjunction interaction for
            // governor p NP and p NP case ... a lot of special code cdm jan 2006

            foreach (TypedDependency td1 in list)
            {
                if (td1.Reln != EnglishGrammaticalRelations.PrepositionalModifier)
                {
                    continue;
                }
                if (td1.Reln == GrammaticalRelation.Kill)
                {
                    continue;
                }

                IndexedWord td1Dep = td1.Dep;
                Util.SortedSet<TypedDependency> possibles = map[td1Dep];
                if (possibles == null)
                {
                    continue;
                }

                // look for the "second half"

                // unique: the head prep and whether it should be pobj
                Tuple<TypedDependency, bool> prepDep = null;
                TypedDependency ccDep = null; // treat as unique
                // list of dep and prepOtherDep and pobj (or  pcomp)
                var conjs = new List<Tuple<TypedDependency, TypedDependency, bool>>();
                Set<TypedDependency> otherDtrs = new TreeSet<TypedDependency>();

                // first look for a conj(prep, prep) (there might be several conj relations!!!)
                bool samePrepositionInEachConjunct = true;
                int conjIndex = -1;
                foreach (TypedDependency td2 in possibles)
                {
                    if (td2.Reln == EnglishGrammaticalRelations.Conjunct)
                    {
                        IndexedWord td2Dep = td2.Dep;
                        string td2DepPOS = td2Dep.Tag();
                        if (td2DepPOS == PartsOfSpeech.PrepositionOrSubordinateConjunction 
                            || td2DepPOS == PartsOfSpeech.To)
                        {
                            samePrepositionInEachConjunct = samePrepositionInEachConjunct &&
                                                            td2Dep.Value().Equals(td1Dep.Value());
                            Set<TypedDependency> possibles2 = map[td2Dep];
                            bool pobj = true; // default of collapsing preposition is prep_
                            TypedDependency prepOtherDep = null;
                            if (possibles2 != null)
                            {
                                foreach (TypedDependency td3 in possibles2)
                                {
                                    IndexedWord td3Dep = td3.Dep;
                                    string td3DepPOS = td3Dep.Tag();
                                    // CDM Mar 2006: I put in disjunction here when I added in
                                    // PREPOSITIONAL_OBJECT. If it catches all cases, we should
                                    // be able to delete the DEPENDENT disjunct
                                    // maybe better to delete the DEPENDENT disjunct - it creates
                                    // problem with multiple prep (mcdm)
                                    if ((td3.Reln == EnglishGrammaticalRelations.PrepositionalObject ||
                                         td3.Reln == EnglishGrammaticalRelations.PrepositionalComplement) &&
                                        (!(td3DepPOS == PartsOfSpeech.PrepositionOrSubordinateConjunction || td3DepPOS == PartsOfSpeech.To)) && prepOtherDep == null)
                                    {
                                        prepOtherDep = td3;
                                        if (td3.Reln == EnglishGrammaticalRelations.PrepositionalComplement)
                                        {
                                            pobj = false;
                                        }
                                    }
                                    else
                                    {
                                        otherDtrs.Add(td3);
                                    }
                                }
                            }
                            if (conjIndex < td2Dep.Index())
                            {
                                conjIndex = td2Dep.Index();
                            }
                            conjs.Add(new Tuple<TypedDependency, TypedDependency, Boolean>(td2, prepOtherDep, pobj));
                        }
                    }
                } // end td2:possibles

                if (!conjs.Any())
                {
                    continue;
                }

                // if we have a conj under a preposition dependency, we look for the other
                // parts

                string td1DepPOS = td1Dep.Tag();
                foreach (TypedDependency td2 in possibles)
                {
                    // we look for the cc linked to this conjDep
                    // the cc dep must have an index smaller than the dep of conjDep
                    if (td2.Reln == EnglishGrammaticalRelations.Coordination && td2.Dep.Index() < conjIndex)
                    {
                        ccDep = td2;
                    }
                    else
                    {
                        IndexedWord td2Dep = td2.Dep;
                        string td2DepPOS = td2Dep.Tag();
                        if ((td2.Reln == GrammaticalRelation.Dependent ||
                             td2.Reln == EnglishGrammaticalRelations.PrepositionalObject ||
                             td2.Reln == EnglishGrammaticalRelations.PrepositionalComplement) &&
                            (PartsOfSpeech.PrepositionOrSubordinateConjunction == td1DepPOS || PartsOfSpeech.To == td1DepPOS
                            || PartsOfSpeech.VerbGerundOrPresentParticiple == td1DepPOS) 
                            && prepDep == null 
                            && (!(PartsOfSpeech.Adverb == td2DepPOS || PartsOfSpeech.PrepositionOrSubordinateConjunction == td2DepPOS || PartsOfSpeech.To == td2DepPOS)))
                        {
                            // same index trick, in case we have multiple deps
                            // I deleted this to see if it helped [cdm Jan 2010] &&
                            // td2.dep().index() < index)
                            prepDep = new Tuple<TypedDependency, Boolean>(td2,
                                td2.Reln != EnglishGrammaticalRelations.PrepositionalComplement);
                        }
                        else if (!inConjDeps(td2, conjs))
                        {
                            // don't want to add the conjDep
                            // again!
                            otherDtrs.Add(td2);
                        }
                    }
                }

                if (prepDep == null || ccDep == null)
                {
                    continue; // we can't deal with it in the hairy prep/conj interaction
                    // case!
                }
                
                // check if we have the same prepositions in the conjunction
                if (samePrepositionInEachConjunct)
                {
                    // conjDep != null && prepOtherDep !=
                    // null &&
                    // OK, we have a conjunction over parallel PPs: Fred flew to Greece and
                    // to Serbia.
                    GrammaticalRelation reln = DeterminePrepRelation(map, vmod, td1, td1, prepDep.Item2);

                    var tdNew = new TypedDependency(reln, td1.Gov, prepDep.Item1.Dep);
                    newTypedDeps.Add(tdNew);
                    td1.Reln = GrammaticalRelation.Kill; // remember these are "used up"
                    prepDep.Item1.Reln = GrammaticalRelation.Kill;
                    ccDep.Reln = GrammaticalRelation.Kill;

                    foreach (Tuple<TypedDependency, TypedDependency, Boolean> trip in conjs)
                    {
                        TypedDependency conjDep = trip.Item1;
                        TypedDependency prepOtherDep = trip.Item2;
                        if (prepOtherDep == null)
                        {
                            // CDM July 2010: I think this should only ever happen if there is a
                            // misparse, but it has happened in such circumstances. You have
                            // something like (PP in or in (NP Serbia)), with the two
                            // prepositions the same. We just clean up the mess.
                            ccDep.Reln = GrammaticalRelation.Kill;
                        }
                        else
                        {
                            var tdNew2 = new TypedDependency(ConjValue(ccDep.Dep.Value()),
                                prepDep.Item1.Dep, prepOtherDep.Dep);
                            newTypedDeps.Add(tdNew2);
                            prepOtherDep.Reln = GrammaticalRelation.Kill;
                        }
                        conjDep.Reln = GrammaticalRelation.Kill;
                    }

                    // promote dtrs that would be orphaned
                    foreach (TypedDependency otd in otherDtrs)
                    {
                        otd.Gov = td1.Gov;
                    }

                    // Now we need to see if there are any TDs that will be "orphaned"
                    // by this collapse. Example: if we have:
                    // dep(drew, on)
                    // dep(on, book)
                    // dep(on, right)
                    // the first two will be collapsed to on(drew, book), but then
                    // the third one will be orphaned, since its governor no
                    // longer appears. So, change its governor to 'drew'.
                    // CDM Feb 2010: This used to not move COORDINATION OR CONJUNCT, but now
                    // it does, since they're not automatically deleted
                    // Some things in possibles may have already been changed, so check gov
                    foreach (TypedDependency td2 in possibles)
                    {
                        if (td2.Reln != GrammaticalRelation.Kill && td2.Gov.Equals(td1.Dep))
                        {
                            // && td2.reln()
                            // != COORDINATION
                            // && td2.reln()
                            // != CONJUNCT
                            td2.Gov = td1.Gov;
                        }
                    }
                    continue; // This one has been dealt with successfully
                } // end same prepositions

                // case of "Lufthansa flies to and from Serbia". Make it look like next
                // case :-)
                // that is, the prepOtherDep should be the same as prepDep !
                var newConjList = new List<Tuple<TypedDependency, TypedDependency, bool>>();
                foreach (Tuple<TypedDependency, TypedDependency, Boolean> trip in conjs)
                {
                    if (trip.Item1 != null && trip.Item2 == null)
                    {
                        /*trip.Item2 = new TypedDependency(prepDep.Item1.reln(), trip.Item1.dep(), prepDep.Item1.dep());
          trip.Item3 = prepDep.Item2;*/
                        newConjList.Add(new Tuple<TypedDependency, TypedDependency, bool>(trip.Item1,
                            new TypedDependency(prepDep.Item1.Reln, trip.Item1.Dep, prepDep.Item1.Dep),
                            prepDep.Item2));
                    }
                    else
                    {
                        newConjList.Add(trip);
                    }
                }
                conjs = newConjList;

                // we have two different prepositions in the conjunction
                // in this case we need to add a node
                // "Bill jumped over the fence and through the hoop"
                // prep_over(jumped, fence)
                // conj_and(jumped, jumped)
                // prep_through(jumped, hoop)

                GrammaticalRelation _reln = DeterminePrepRelation(map, vmod, td1, td1, prepDep.Item2);
                var _tdNew = new TypedDependency(_reln, td1.Gov, prepDep.Item1.Dep);
                newTypedDeps.Add(_tdNew);

                td1.Reln = GrammaticalRelation.Kill; // remember these are "used up"
                prepDep.Item1.Reln = GrammaticalRelation.Kill;
                ccDep.Reln = GrammaticalRelation.Kill;
                // so far we added the first prep grammatical relation

                int copyNumber = 1;
                foreach (Tuple<TypedDependency, TypedDependency, bool> trip in conjs)
                {
                    TypedDependency conjDep = trip.Item1;
                    TypedDependency prepOtherDep = trip.Item2;
                    bool pobj = trip.Item3;
                    // OK, we have a conjunction over different PPs
                    // we create a new node;
                    // in order to make a distinction between the original node and its copy
                    // we add a "copy" entry in the CoreLabel
                    // existence of copy key is checked at printing (ToString method of
                    // TypedDependency)
                    IndexedWord label = td1.Gov.MakeCopy(copyNumber);
                    copyNumber++;

                    // now we add the conjunction relation between td1.gov and the copy
                    // the copy has the same label as td1.gov() but is another TreeGraphNode
                    var tdNew2 = new TypedDependency(ConjValue(ccDep.Dep.Value()), td1.Gov, label);
                    newTypedDeps.Add(tdNew2);

                    // now we still need to add the second prep grammatical relation
                    // between the copy and the dependent of the prepOtherDep node
                    GrammaticalRelation reln2 = DeterminePrepRelation(map, vmod, conjDep, td1, pobj);
                    var tdNew3 = new TypedDependency(reln2, label, prepOtherDep.Dep);
                    newTypedDeps.Add(tdNew3);

                    conjDep.Reln = GrammaticalRelation.Kill;
                    prepOtherDep.Reln = GrammaticalRelation.Kill;

                    // promote dtrs that would be orphaned
                    foreach (TypedDependency otd in otherDtrs)
                    {
                        // special treatment for prepositions: the original relation is
                        // likely to be a "dep" and we want this to be a "prep"
                        if (otd.Dep.Tag() == PartsOfSpeech.PrepositionOrSubordinateConjunction)
                        {
                            otd.Reln = EnglishGrammaticalRelations.PrepositionalModifier;
                        }
                        otd.Gov = td1.Gov;
                    }
                }

                // Now we need to see if there are any TDs that will be "orphaned" off
                // the first preposition
                // by this collapse. Example: if we have:
                // dep(drew, on)
                // dep(on, book)
                // dep(on, right)
                // the first two will be collapsed to on(drew, book), but then
                // the third one will be orphaned, since its governor no
                // longer appears. So, change its governor to 'drew'.
                // CDM Feb 2010: This used to not move COORDINATION OR CONJUNCT, but now
                // it does, since they're not automatically deleted
                foreach (TypedDependency td2 in possibles)
                {
                    if (td2.Reln != GrammaticalRelation.Kill)
                    {
                        // && td2.reln() != COORDINATION &&
                        // td2.reln() != CONJUNCT) {
                        td2.Gov = td1.Gov;
                    }
                }
                // end for different prepositions
            } // for TypedDependency td1 : list

            // below here is the single preposition/possessor basic case!!
            foreach (TypedDependency td1 in list)
            {
                if (td1.Reln == GrammaticalRelation.Kill)
                {
                    continue;
                }

                IndexedWord td1Dep = td1.Dep;
                string td1DepPOS = td1Dep.Tag();
                // find all other typedDeps having our dep as gov
                Set<TypedDependency> possibles = map[td1Dep];

                if (possibles != null &&
                    (td1.Reln == EnglishGrammaticalRelations.PrepositionalModifier ||
                     td1.Reln == EnglishGrammaticalRelations.PossessionModifier ||
                     td1.Reln == EnglishGrammaticalRelations.Conjunct))
                {

                    // look for the "second half"
                    bool pobj = true; // default for prep relation is prep_
                    foreach (TypedDependency td2 in possibles)
                    {
                        if (td2.Reln != EnglishGrammaticalRelations.Coordination &&
                            td2.Reln != EnglishGrammaticalRelations.Conjunct)
                        {

                            IndexedWord td2Dep = td2.Dep;
                            string td2DepPOS = td2Dep.Tag();
                            if ((td1.Reln == EnglishGrammaticalRelations.PossessionModifier ||
                                 td1.Reln == EnglishGrammaticalRelations.Conjunct))
                            {
                                if (td2.Reln == EnglishGrammaticalRelations.PossessiveModifier)
                                {
                                    if (! map.ContainsKey(td2Dep))
                                    {
                                        // if 's has no kids of its own (it shouldn't!)
                                        td2.Reln = GrammaticalRelation.Kill;
                                    }
                                }
                            }
                            else if ((td2.Reln == EnglishGrammaticalRelations.PrepositionalObject ||
                                      td2.Reln == EnglishGrammaticalRelations.PrepositionalComplement) &&
                                     (PartsOfSpeech.PrepositionOrSubordinateConjunction == td1DepPOS 
                                || PartsOfSpeech.To == td1DepPOS || PartsOfSpeech.VerbGerundOrPresentParticiple == td1DepPOS) &&
                                     (!(PartsOfSpeech.Adverb == td2DepPOS 
                                || PartsOfSpeech.PrepositionOrSubordinateConjunction == td2DepPOS || PartsOfSpeech.To == td2DepPOS)) &&
                                     !IsConjWithNoPrep(td2.Gov, possibles))
                            {
                                // we don't collapse preposition conjoined with a non-preposition
                                // to avoid disconnected constituents
                                // OK, we have a pair td1, td2 to collapse to td3

                                // check whether we are in a pcomp case:
                                if (td2.Reln == EnglishGrammaticalRelations.PrepositionalComplement)
                                {
                                    pobj = false;
                                }

                                GrammaticalRelation reln = DeterminePrepRelation(map, vmod, td1, td1, pobj);
                                var td3 = new TypedDependency(reln, td1.Gov, td2.Dep);
                                // add it to map to deal with recursive cases like "achieved this (PP (PP in part) with talent)"
                                map[td3.Gov].Add(td3);

                                newTypedDeps.Add(td3);
                                td1.Reln = GrammaticalRelation.Kill; // remember these are "used up"
                                td2.Reln = GrammaticalRelation.Kill; // remember these are "used up"
                            }
                        }
                    } // for TypedDependency td2
                }

                // Now we need to see if there are any TDs that will be "orphaned"
                // by this collapse. Example: if we have:
                // dep(drew, on)
                // dep(on, book)
                // dep(on, right)
                // the first two will be collapsed to on(drew, book), but then
                // the third one will be orphaned, since its governor no
                // longer appears. So, change its governor to 'drew'.
                // CDM Feb 2010: This used to not move COORDINATION OR CONJUNCT, but now
                // it does, since they're not automatically deleted
                if (possibles != null && td1.Reln == GrammaticalRelation.Kill)
                {
                    foreach (TypedDependency td2 in possibles)
                    {
                        if (td2.Reln != GrammaticalRelation.Kill)
                        {
                            // && td2.reln() != COORDINATION &&
                            // td2.reln() != CONJUNCT) {
                            td2.Gov = td1.Gov;
                        }
                    }
                }

            } // for TypedDependency td1

            // now remove typed dependencies with reln "kill" and add new ones.
            /*for (Iterator<TypedDependency> iter = list.iterator(); iter.hasNext();) {
      TypedDependency td = iter.next();
      if (td.reln() == KILL) {
        iter.remove();
      }
    }*/
            list.RemoveAll(td => td.Reln == GrammaticalRelation.Kill);
            list.AddRange(newTypedDeps);
        } // end collapsePrepAndPoss()

        private static bool inConjDeps(TypedDependency td, List<Tuple<TypedDependency, TypedDependency, bool>> conjs)
        {
            foreach (Tuple<TypedDependency, TypedDependency, bool> trip in conjs)
            {
                if (td.Equals(trip.Item1))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Given a list of typedDependencies, returns true if the node "node" is the
        /// governor of a conj relation with a dependent which is not a preposition
        /// </summary>
        /// <param name="node">A node in this GrammaticalStructure</param>
        /// <param name="list">A list of typedDependencies</param>
        /// <returns>
        /// true If node is the governor of a conj relation in the list with the dep not being a preposition
        /// </returns>
        private static bool IsConjWithNoPrep(IndexedWord node, List<TypedDependency> list)
        {
            foreach (TypedDependency td in list)
            {
                if (td.Gov.Equals(node) && td.Reln == EnglishGrammaticalRelations.Conjunct)
                {
                    // we have a conjunct
                    // check the POS of the dependent
                    string tdDepPos = td.Dep.Tag();
                    if (!(tdDepPos == PartsOfSpeech.PrepositionOrSubordinateConjunction 
                        || tdDepPos == PartsOfSpeech.To))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Work out prep relation name. pc is the dependency whose dep() is the
        /// preposition to do a name for. topPrep may be the same or different.
        /// Among the daughters of its gov is where to look for an auxpass.
        /// </summary>
        private static GrammaticalRelation DeterminePrepRelation(
            Dictionary<IndexedWord, Util.SortedSet<TypedDependency>> map, List<IndexedWord> vmod, TypedDependency pc,
            TypedDependency topPrep, bool pobj)
        {
            // handling the case of an "agent":
            // the governor of a "by" preposition must have an "auxpass" dependency
            // or be the dependent of a "vmod" relation
            // if it is the case, the "agent" variable becomes true
            bool agent = false;
            string preposition = pc.Dep.Value().ToLower();
            if (preposition.Equals("by"))
            {
                // look if we have an auxpass
                Set<TypedDependency> aux_pass_poss = map[topPrep.Gov];
                if (aux_pass_poss != null)
                {
                    foreach (TypedDependency td_pass in aux_pass_poss)
                    {
                        if (td_pass.Reln == EnglishGrammaticalRelations.AuxPassiveModifier)
                        {
                            agent = true;
                        }
                    }
                }
                // look if we have a vmod
                if (vmod.Any() && vmod.Contains(topPrep.Gov))
                {
                    agent = true;
                }
            }

            GrammaticalRelation reln;
            if (agent)
            {
                reln = EnglishGrammaticalRelations.Agent;
            }
            else
            {
                // for prepositions, use the preposition
                // for pobj: we collapse into "prep"; for pcomp: we collapse into "prepc"
                if (pobj)
                {
                    reln = EnglishGrammaticalRelations.GetPrep(preposition);
                }
                else
                {
                    reln = EnglishGrammaticalRelations.GetPrepC(preposition);
                }
            }
            return reln;
        }

        /// <summary>
        /// This rewrites the "conj" relation to "conj_word" and deletes cases of the
        /// "cc" relation providing this rewrite has occurred (but not if there is only
        /// something like a clause-initial and). For instance, cc(elected-5, and-9)
        /// conj(elected-5, re-elected-11) becomes conj_and(elected-5, re-elected-11)
        /// </summary>
        /// <param name="list">List of dependencies.</param>
        private static void CollapseConj(List<TypedDependency> list)
        {
            var govs = new List<IndexedWord>();
            // find typed deps of form cc(gov, dep)
            foreach (TypedDependency td in list)
            {
                if (td.Reln == EnglishGrammaticalRelations.Coordination)
                {
                    // i.e. "cc"
                    IndexedWord gov = td.Gov;
                    GrammaticalRelation conj = ConjValue(td.Dep.Value());

                    // find other deps of that gov having reln "conj"
                    bool foundOne = false;
                    foreach (TypedDependency td1 in list)
                    {
                        if (td1.Gov.Equals(gov))
                        {
                            if (td1.Reln == EnglishGrammaticalRelations.Conjunct)
                            {
                                // i.e., "conj"
                                // change "conj" to the actual (lexical) conjunction
                                td1.Reln = conj;
                                foundOne = true;
                            }
                            else if (td1.Reln == EnglishGrammaticalRelations.Coordination)
                            {
                                conj = ConjValue(td1.Dep.Value());
                            }
                        }
                    }

                    // register to remove cc from this governor
                    if (foundOne)
                    {
                        govs.Add(gov);
                    }
                }
            }

            // now remove typed dependencies with reln "cc" if we have successfully
            // collapsed
            /*for (Iterator<TypedDependency> iter = list.iterator(); iter.hasNext();) {
      TypedDependency td2 = iter.next();
      if (td2.reln() == COORDINATION && govs.contains(td2.gov())) {
        iter.remove();
      }
    }*/
            list.RemoveAll(td => td.Reln == EnglishGrammaticalRelations.Coordination && govs.Contains(td.Gov));
        }

        /// <summary>
        /// Does some hard coding to deal with relation in CONJP. For now we deal with:
        /// but not, if not, instead of, rather than, but rather GO TO negcc
        /// as well as, not to mention, but also, &amp; GO TO and.
        /// </summary>
        /// <param name="conj">The head dependency of the conjunction marker</param>
        /// <returns>A GrammaticalRelation made from a normalized form of that conjunction.</returns>
        private static GrammaticalRelation ConjValue(string conj)
        {
            string newConj = conj.ToLower();
            if (newConj.Equals("not") || newConj.Equals("instead") || newConj.Equals("rather"))
            {
                newConj = "negcc";
            }
            else if (newConj.Equals("mention") || newConj.Equals("to") || newConj.Equals("also") ||
                     newConj.Contains("well") || newConj.Equals("&"))
            {
                newConj = "and";
            }
            return EnglishGrammaticalRelations.GetConj(newConj);
        }

        /// <summary>
        /// Look for ref rules for a given word.  We look through the
        /// children and grandchildren of the rcmod dependency, and if any
        /// children or grandchildren depend on a that/what/which/etc word,
        /// we take the leftmost that/what/which/etc word as the dependent
        /// for the ref TypedDependency.
        /// </summary>
        private static void AddRef(List<TypedDependency> list)
        {
            var newDeps = new List<TypedDependency>();

            foreach (TypedDependency rcmod in list)
            {
                if (rcmod.Reln != EnglishGrammaticalRelations.RelativeClauseModifier)
                {
                    // we only add ref dependencies across relative clauses
                    continue;
                }

                IndexedWord head = rcmod.Gov;
                IndexedWord modifier = rcmod.Dep;

                TypedDependency leftChild = null;
                foreach (TypedDependency child in list)
                {
                    if (child.Gov.Equals(modifier) &&
                        EnglishPatterns.RelativizingWordPattern.IsMatch(child.Dep.Value()) &&
                        (leftChild == null || child.Dep.Index() < leftChild.Dep.Index()))
                    {
                        leftChild = child;
                    }
                }

                // TODO: could be made more efficient
                TypedDependency leftGrandchild = null;
                foreach (TypedDependency child in list)
                {
                    if (!child.Gov.Equals(modifier))
                    {
                        continue;
                    }
                    foreach (TypedDependency grandchild in list)
                    {
                        if (grandchild.Gov.Equals(child.Dep) &&
                            EnglishPatterns.RelativizingWordPattern.IsMatch(grandchild.Dep.Value()) &&
                            (leftGrandchild == null || grandchild.Dep.Index() < leftGrandchild.Dep.Index()))
                        {
                            leftGrandchild = grandchild;
                        }
                    }
                }

                TypedDependency newDep = null;
                if (leftGrandchild != null &&
                    (leftChild == null || leftGrandchild.Dep.Index() < leftChild.Dep.Index()))
                {
                    newDep = new TypedDependency(EnglishGrammaticalRelations.Referent, head, leftGrandchild.Dep);
                }
                else if (leftChild != null)
                {
                    newDep = new TypedDependency(EnglishGrammaticalRelations.Referent, head, leftChild.Dep);
                }
                if (newDep != null)
                {
                    newDeps.Add(newDep);
                }
            }

            foreach (TypedDependency newDep in newDeps)
            {
                if (!list.Contains(newDep))
                {
                    newDep.Extra = true;
                    list.Add(newDep);
                }
            }
        }

        /// <summary>
        /// This method will collapse a referent relation such as follows. e.g.:
        /// "The man that I love ... " ref(man, that) dobj(love, that) -> dobj(love, man)
        /// </summary>
        private static void CollapseReferent(List<TypedDependency> list)
        {
            // find typed deps of form ref(gov, dep)
            // put them in a List for processing; remove them from the set of deps
            var refs = new List<TypedDependency>();
            refs.AddRange(list.Where(td => td.Reln == EnglishGrammaticalRelations.Referent));
            list.RemoveAll(td => td.Reln == EnglishGrammaticalRelations.Referent);
            /*for (Iterator<TypedDependency> iter = list.iterator(); iter.hasNext();) {
              TypedDependency td = iter.next();
              if (td.reln() == REFERENT) {
                refs.add(td);
                iter.remove();
              }
            }*/

            // now substitute target of referent where possible
            foreach (TypedDependency re in refs)
            {
                IndexedWord dep = re.Dep; // take the relative word
                IndexedWord ant = re.Gov; // take the antecedent
                foreach (TypedDependency td in list)
                {
                    // the last condition below maybe shouldn't be necessary, but it has
                    // helped stop things going haywire a couple of times (it stops the
                    // creation of a unit cycle that probably leaves something else
                    // disconnected) [cdm Jan 2010]
                    if (td.Dep.Equals(dep) && td.Reln != EnglishGrammaticalRelations.Referent &&
                        !td.Gov.Equals(ant))
                    {
                        td.Dep = ant;
                    }
                }
            }
        }

        private static void TreatCc(List<TypedDependency> list)
        {
            // Construct a map from tree nodes to the set of typed
            // dependencies in which the node appears as dependent.
            var map = new Dictionary<IndexedWord, Set<TypedDependency>>();
            // Construct a map of tree nodes being governor of a subject grammatical
            // relation to that relation
            var subjectMap = new Dictionary<IndexedWord, TypedDependency>();
            // Construct a set of TreeGraphNodes with a passive auxiliary on them
            Set<IndexedWord> withPassiveAuxiliary = new Util.HashSet<IndexedWord>();
            // Construct a map of tree nodes being governor of an object grammatical
            // relation to that relation
            // Map<TreeGraphNode, TypedDependency> objectMap = new
            // HashMap<TreeGraphNode, TypedDependency>();

            var rcmodHeads = new List<IndexedWord>();
            var prepcDep = new List<IndexedWord>();

            foreach (TypedDependency typedDep in list)
            {
                if (!map.ContainsKey(typedDep.Dep))
                {
                    // NB: Here and in other places below, we use a TreeSet (which extends
                    // SortedSet) to guarantee that results are deterministic)
                    map.Add(typedDep.Dep, new TreeSet<TypedDependency>());
                }
                map[typedDep.Dep].Add(typedDep);

                if (typedDep.Reln.Equals(EnglishGrammaticalRelations.AuxPassiveModifier))
                {
                    withPassiveAuxiliary.Add(typedDep.Gov);
                }

                // look for subjects
                if (typedDep.Reln.GetParent() == EnglishGrammaticalRelations.NominalSubject ||
                    typedDep.Reln.GetParent() == EnglishGrammaticalRelations.Subject ||
                    typedDep.Reln.GetParent() == EnglishGrammaticalRelations.ClausalSubject)
                {
                    if (!subjectMap.ContainsKey(typedDep.Gov))
                    {
                        subjectMap.Add(typedDep.Gov, typedDep);
                    }
                }

                // look for objects
                // this map was only required by the code commented out below, so comment
                // it out too
                // if (typedDep.reln() == DIRECT_OBJECT) {
                // if (!objectMap.containsKey(typedDep.gov())) {
                // objectMap.put(typedDep.gov(), typedDep);
                // }
                // }

                // look for rcmod relations
                if (typedDep.Reln == EnglishGrammaticalRelations.RelativeClauseModifier)
                {
                    rcmodHeads.Add(typedDep.Gov);
                }
                // look for prepc relations: put the dependent of such a relation in the
                // list
                // to avoid wrong propagation of dobj
                if (typedDep.Reln.ToString().StartsWith("prepc"))
                {
                    prepcDep.Add(typedDep.Dep);
                }
            }
            
            // create a new list of typed dependencies
            var newTypedDeps = new List<TypedDependency>(list);

            // find typed deps of form conj(gov,dep)
            foreach (TypedDependency td in list)
            {
                if (EnglishGrammaticalRelations.GetConjs().Contains(td.Reln))
                {
                    IndexedWord gov = td.Gov;
                    IndexedWord dep = td.Dep;

                    // look at the dep in the conjunct
                    Set<TypedDependency> govRelations = map[gov];
                    if (govRelations != null)
                    {
                        foreach (TypedDependency td1 in govRelations)
                        {
                            IndexedWord newGov = td1.Gov;
                            // in the case of errors in the basic dependencies, it
                            // is possible to have overlapping newGov & dep
                            if (newGov.Equals(dep))
                            {
                                continue;
                            }
                            GrammaticalRelation newRel = td1.Reln;
                            if (newRel != GrammaticalRelation.Root)
                            {
                                if (rcmodHeads.Contains(gov) && rcmodHeads.Contains(dep))
                                {
                                    // to prevent wrong propagation in the case of long dependencies in relative clauses
                                    if (newRel != EnglishGrammaticalRelations.DirectObject &&
                                        newRel != EnglishGrammaticalRelations.NominalSubject)
                                    {
                                        newTypedDeps.Add(new TypedDependency(newRel, newGov, dep));
                                    }
                                }
                                else
                                {
                                    newTypedDeps.Add(new TypedDependency(newRel, newGov, dep));
                                }
                            }
                        }
                    }

                    // propagate subjects
                    // look at the gov in the conjunct: if it is has a subject relation,
                    // the dep is a verb and the dep doesn't have a subject relation
                    // then we want to add a subject relation for the dep.
                    // (By testing for the dep to be a verb, we are going to miss subject of
                    // copular verbs! but
                    // is it safe to relax this assumption?? i.e., just test for the subject
                    // part)
                    // CDM 2008: I also added in JJ, since participial verbs are often
                    // tagged JJ
                    string tag = dep.Tag();
                    if (subjectMap.ContainsKey(gov) && (PartsOfSpeech.IsVerb(tag) || PartsOfSpeech.IsAdjective(tag)) &&
                        ! subjectMap.ContainsKey(dep))
                    {
                        TypedDependency tdsubj = subjectMap[gov];
                        // check for wrong nsubjpass: if the new verb is VB or VBZ or VBP or JJ, then
                        // add nsubj (if it is tagged correctly, should do this for VBD too, but we don't)
                        GrammaticalRelation relation = tdsubj.Reln;
                        if (relation == EnglishGrammaticalRelations.NominalPassiveSubject)
                        {
                            if (IsDefinitelyActive(tag))
                            {
                                relation = EnglishGrammaticalRelations.NominalSubject;
                            }
                        }
                        else if (relation == EnglishGrammaticalRelations.ClausalPassiveSubject)
                        {
                            if (IsDefinitelyActive(tag))
                            {
                                relation = EnglishGrammaticalRelations.ClausalSubject;
                            }
                        }
                        else if (relation == EnglishGrammaticalRelations.NominalSubject)
                        {
                            if (withPassiveAuxiliary.Contains(dep))
                            {
                                relation = EnglishGrammaticalRelations.NominalPassiveSubject;
                            }
                        }
                        else if (relation == EnglishGrammaticalRelations.ClausalSubject)
                        {
                            if (withPassiveAuxiliary.Contains(dep))
                            {
                                relation = EnglishGrammaticalRelations.ClausalPassiveSubject;
                            }
                        }
                        newTypedDeps.Add(new TypedDependency(relation, dep, tdsubj.Dep));
                    }

                    // propagate objects
                    // cdm july 2010: This bit of code would copy a dobj from the first
                    // clause to a later conjoined clause if it didn't
                    // contain its own dobj or prepc. But this is too aggressive and wrong
                    // if the later clause is intransitive
                    // (including passivized cases) and so I think we have to not have this
                    // done always, and see no good "sometimes" heuristic.
                    // IF WE WERE TO REINSTATE, SHOULD ALSO NOT ADD OBJ IF THERE IS A ccomp
                    // (SBAR).
                    // if (objectMap.containsKey(gov) &&
                    // dep.tag().startsWith("VB") && ! objectMap.containsKey(dep)
                    // && ! prepcDep.contains(gov)) {
                    // TypedDependency tdobj = objectMap.get(gov);
                    // newTypedDeps.add(new TypedDependency(tdobj.reln(), dep,
                    // tdobj.dep()));
                    // }
                }
            }
            list.Clear();
            list.AddRange(newTypedDeps);
        }

        private static bool IsDefinitelyActive(string tag)
        {
            // we should include VBD, but don't as it is often a tagging mistake.
            return tag == PartsOfSpeech.VerbBaseForm
                || tag == PartsOfSpeech.Verb3rdPersSingPresent
                || tag == PartsOfSpeech.VerbNon3rdPersSingPresent 
                || PartsOfSpeech.IsAdjective(tag);
        }

        /// <summary>
        /// Add extra nsubj dependencies when collapsing basic dependencies.
        /// 
        /// In the general case, we look for an aux modifier under an xcomp
        /// modifier, and assuming there aren't already associated nsubj
        /// dependencies as daughters of the original xcomp dependency, we
        /// add nsubj dependencies for each nsubj daughter of the aux.
        /// 
        /// There is also a special case for "to" words, in which case we add
        /// a dependency if and only if there is no nsubj associated with the
        /// xcomp and there is no other aux dependency.  This accounts for
        /// sentences such as "he decided not to" with no following verb.
        /// </summary>
        private static void AddExtraNSubj(List<TypedDependency> list)
        {
            var newDeps = new List<TypedDependency>();

            foreach (TypedDependency xcomp in list)
            {
                if (xcomp.Reln != EnglishGrammaticalRelations.XclausalComplement)
                {
                    // we only add extra nsubj dependencies to some xcomp dependencies
                    continue;
                }

                IndexedWord modifier = xcomp.Dep;
                IndexedWord head = xcomp.Gov;

                bool hasSubjectDaughter = false;
                bool hasAux = false;
                var subjects = new List<IndexedWord>();
                var objects = new List<IndexedWord>();
                foreach (TypedDependency dep in list)
                {
                    // already have a subject dependency
                    if ((dep.Reln == EnglishGrammaticalRelations.NominalSubject ||
                         dep.Reln == EnglishGrammaticalRelations.NominalPassiveSubject) &&
                        dep.Gov.Equals(modifier))
                    {
                        hasSubjectDaughter = true;
                        break;
                    }

                    if (dep.Reln == EnglishGrammaticalRelations.AuxModifier && dep.Gov.Equals(modifier))
                    {
                        hasAux = true;
                    }

                    if ((dep.Reln == EnglishGrammaticalRelations.NominalSubject ||
                         dep.Reln == EnglishGrammaticalRelations.NominalPassiveSubject) && dep.Gov.Equals(head))
                    {
                        subjects.Add(dep.Dep);
                    }

                    if (dep.Reln == EnglishGrammaticalRelations.DirectObject && dep.Gov.Equals(head))
                    {
                        objects.Add(dep.Dep);
                    }
                }

                // if we already have an nsubj dependency, no need to add an extra nsubj
                if (hasSubjectDaughter)
                {
                    continue;
                }

                if ((modifier.Value().Equals("to", StringComparison.InvariantCultureIgnoreCase) && hasAux) ||
                    (!modifier.Value().Equals("to", StringComparison.InvariantCultureIgnoreCase) && !hasAux))
                {
                    continue;
                }

                // In general, we find that the objects of the verb are better
                // for extra nsubj than the original nsubj of the verb.  For example,
                // "Many investors wrote asking the SEC to require ..."
                // There is no nsubj of asking, but the dobj, SEC, is the extra nsubj of require.
                // Similarly, "The law tells them when to do so"
                // Instead of nsubj(do, law) we want nsubj(do, them)
                if (objects.Count > 0)
                {
                    foreach (IndexedWord obj in objects)
                    {
                        var newDep = new TypedDependency(EnglishGrammaticalRelations.NominalSubject,
                            modifier, obj);
                        newDeps.Add(newDep);
                    }
                }
                else
                {
                    foreach (IndexedWord subject in subjects)
                    {
                        var newDep = new TypedDependency(EnglishGrammaticalRelations.NominalSubject,
                            modifier, subject);
                        newDeps.Add(newDep);
                    }
                }
            }

            foreach (TypedDependency newDep in newDeps)
            {
                if (!list.Contains(newDep))
                {
                    newDep.Extra = true;
                    list.Add(newDep);
                }
            }
        }

        /// <summary>
        /// This method corrects subjects of verbs for which we identified an auxpass,
        /// but didn't identify the subject as passive.
        /// </summary>
        /// <param name="list">List of typedDependencies to work on</param>
        private static void CorrectSubjPass(List<TypedDependency> list)
        {
            // put in a list verbs having an auxpass
            var list_auxpass = new List<IndexedWord>();
            foreach (TypedDependency td in list)
            {
                if (td.Reln == EnglishGrammaticalRelations.AuxPassiveModifier)
                {
                    list_auxpass.Add(td.Gov);
                }
            }
            foreach (TypedDependency td in list)
            {
                // correct nsubj
                if (td.Reln == EnglishGrammaticalRelations.NominalSubject && list_auxpass.Contains(td.Gov))
                {
                    td.Reln = EnglishGrammaticalRelations.NominalPassiveSubject;
                }
                if (td.Reln == EnglishGrammaticalRelations.ClausalSubject && list_auxpass.Contains(td.Gov))
                {
                    td.Reln = EnglishGrammaticalRelations.ClausalPassiveSubject;
                }

                // correct unretrieved poss: dep relation in which the dependent is a
                // PRP$ or WP$
                // cdm: Now done in basic rules.  The only cases that this still matches
                // are (1) tagging mistakes where PRP in dobj position is mistagged PRP$
                // or a couple of parsing errors where the dependency is wrong anyway, so
                // it's probably okay to keep it a dep.  So I'm disabling this.
                // string tag = td.dep().tag();
                // if (td.reln() == DEPENDENT && (tag.equals(PartsOfSpeech.PossessivePronoun) || tag.equals(PartsOfSpeech.PossessiveWhPronoun))) {
                //  td.setReln(POSSESSION_MODIFIER);
                // }
            }
        }

        /// <summary>
        /// Remove duplicate relations: it can happen when collapsing stranded
        /// prepositions. E.g., "What does CPR stand for?" we get dep(stand, what), and
        /// after collapsing we also get prep_for(stand, what).
        /// </summary>
        /// <param name="list">A list of typed dependencies to check through</param>
        private static void RemoveDep(List<TypedDependency> list)
        {
            Set<GrammaticalRelation> prepRels = new Util.HashSet<GrammaticalRelation>(EnglishGrammaticalRelations.GetPreps());
            prepRels.AddAll(EnglishGrammaticalRelations.GetPrepsC());
            foreach (TypedDependency td1 in list)
            {
                if (prepRels.Contains(td1.Reln))
                {
                    // if we have a prep_ relation
                    IndexedWord gov = td1.Gov;
                    IndexedWord dep = td1.Dep;

                    foreach (TypedDependency td2 in list)
                    {
                        if (td2.Reln == GrammaticalRelation.Dependent && td2.Gov.Equals(gov) &&
                            td2.Dep.Equals(dep))
                        {
                            td2.Reln = GrammaticalRelation.Kill;
                        }
                    }
                }
            }

            // now remove typed dependencies with reln "kill"
            /*for (Iterator<TypedDependency> iter = list.iterator(); iter.hasNext();) {
      TypedDependency td = iter.next();
      if (td.reln() == KILL) {
        iter.remove();
      }
    }*/
            list.RemoveAll(td => td.Reln == GrammaticalRelation.Kill);
        }

        protected override void CollapseDependenciesTree(List<TypedDependency> list)
        {
            CollapseDependencies(list, false, false);
        }

        /// <summary>
        /// This method gets rid of multiwords in conjunctions to avoid having them
        /// creating disconnected constituents e.g.,
        /// "bread-1 as-2 well-3 as-4 cheese-5" will be turned into conj_and(bread,
        /// cheese) and then dep(well-3, as-2) and dep(well-3, as-4) cannot be attached
        /// to the graph, these dependencies are erased
        /// </summary>
        /// <param name="list">List of words to get rid of multiword conjunctions from</param>
        private static void EraseMultiConj(List<TypedDependency> list)
        {
            // find typed deps of form cc(gov, x)
            foreach (TypedDependency td1 in list)
            {
                if (td1.Reln == EnglishGrammaticalRelations.Coordination)
                {
                    IndexedWord x = td1.Dep;
                    // find typed deps of form dep(x,y) and kill them
                    foreach (TypedDependency td2 in list)
                    {
                        if (td2.Gov.Equals(x) &&
                            (td2.Reln == GrammaticalRelation.Dependent ||
                             td2.Reln == EnglishGrammaticalRelations.MultiWordExpression ||
                             td2.Reln == EnglishGrammaticalRelations.Coordination ||
                             td2.Reln == EnglishGrammaticalRelations.AdverbialModifier ||
                             td2.Reln == EnglishGrammaticalRelations.NegationModifier ||
                             td2.Reln == EnglishGrammaticalRelations.AuxModifier))
                        {
                            td2.Reln = GrammaticalRelation.Kill;
                        }
                    }
                }
            }

            FilterKill(list);
        }

        /// <summary>
        /// Alters a list in place by removing all the KILL relations
        /// </summary>
        private static void FilterKill(List<TypedDependency> deps)
        {
            var filtered = new List<TypedDependency>();
            foreach (TypedDependency dep in deps)
            {
                if (dep.Reln != GrammaticalRelation.Kill)
                {
                    filtered.Add(dep);
                }
            }
            deps.Clear();
            deps.AddRange(filtered);
        }

        /// <summary>
        /// used by collapse2WP(), collapseFlatMWP(), collapse2WPbis()
        /// KEPT IN ALPHABETICAL ORDER
        /// </summary>
        private static readonly string[][] MultiwordPreps =
        {
            new string[] {"according", "to"}, new string[] {"across", "from"}, new string[] {"ahead", "of"},
            new string[] {"along", "with"}, new string[] {"alongside", "of"}, new string[] {"apart", "from"},
            new string[] {"as", "for"}, new string[] {"as", "from"}, new string[] {"as", "of"},
            new string[] {"as", "per"}, new string[] {"as", "to"}, new string[] {"aside", "from"},
            new string[] {"away", "from"}, new string[] {"based", "on"}, new string[] {"because", "of"},
            new string[] {"close", "by"}, new string[] {"close", "to"}, new string[] {"contrary", "to"},
            new string[] {"compared", "to"}, new string[] {"compared", "with"}, new string[] {"due", "to"},
            new string[] {"depending", "on"}, new string[] {"except", "for"}, new string[] {"exclusive", "of"},
            new string[] {"far", "from"}, new string[] {"followed", "by"}, new string[] {"inside", "of"},
            new string[] {"instead", "of"}, new string[] {"irrespective", "of"}, new string[] {"next", "to"},
            new string[] {"near", "to"}, new string[] {"off", "of"}, new string[] {"out", "of"},
            new string[] {"outside", "of"}, new string[] {"owing", "to"}, new string[] {"preliminary", "to"},
            new string[] {"preparatory", "to"}, new string[] {"previous", "to"}, new string[] {"prior", "to"},
            new string[] {"pursuant", "to"}, new string[] {"regardless", "of"}, new string[] {"subsequent", "to"},
            new string[] {"such", "as"}, new string[] {"thanks", "to"}, new string[] {"together", "with"}
        };

        /// <summary>
        /// Used by collapse3WP() KEPT IN ALPHABETICAL ORDER
        /// </summary>
        private static readonly string[][] ThreewordPreps =
        {
            new string[] {"by", "means", "of"},
            new string[] {"in", "accordance", "with"}, new string[] {"in", "addition", "to"},
            new string[] {"in", "case", "of"}, new string[] {"in", "front", "of"}, new string[] {"in", "lieu", "of"},
            new string[] {"in", "place", "of"}, new string[] {"in", "spite", "of"}, new string[] {"on", "account", "of"},
            new string[] {"on", "behalf", "of"}, new string[] {"on", "top", "of"}, new string[] {"with", "regard", "to"},
            new string[] {"with", "respect", "to"}
        };

        /// <summary>
        /// * Collapse multiword preposition of the following format:
        /// prep|advmod|dep|amod(gov, mwp[0])
        /// dep(mpw[0],mwp[1])
        /// pobj|pcomp(mwp[1], compl) or pobj|pcomp(mwp[0], compl)
        /// -> prep_mwp[0]_mwp[1](gov, compl)
        /// prep|advmod|dep|amod(gov, mwp[1])
        /// dep(mpw[1],mwp[0])
        /// pobj|pcomp(mwp[1], compl) or pobj|pcomp(mwp[0], compl)
        /// -> prep_mwp[0]_mwp[1](gov, compl)
        /// 
        /// The collapsing has to be done at once in order to know exactly which node
        /// is the gov and the dep of the multiword preposition. Otherwise this can
        /// lead to problems: removing a non-multiword "to" preposition for example!!!
        /// This method replaces the old "collapsedMultiWordPreps"
        /// </summary>
        /// <param name="list">list of typedDependencies to work on</param>
        private static void Collapse2Wp(List<TypedDependency> list)
        {
            var newTypedDeps = new List<TypedDependency>();

            foreach (string[] mwp in MultiwordPreps)
            {
                // first look for patterns such as:
                // X(gov, mwp[0])
                // Y(mpw[0],mwp[1])
                // Z(mwp[1], compl) or Z(mwp[0], compl)
                // -> prep_mwp[0]_mwp[1](gov, compl)
                CollapseMultiWordPrep(list, newTypedDeps, mwp[0], mwp[1], mwp[0], mwp[1]);

                // now look for patterns such as:
                // X(gov, mwp[1])
                // Y(mpw[1],mwp[0])
                // Z(mwp[1], compl) or Z(mwp[0], compl)
                // -> prep_mwp[0]_mwp[1](gov, compl)
                CollapseMultiWordPrep(list, newTypedDeps, mwp[0], mwp[1], mwp[1], mwp[0]);
            }
        }
        
        /// <summary>
        /// Collapse multiword preposition of the following format:
        /// prep|advmod|dep|amod(gov, mwp0) dep(mpw0,mwp1) pobj|pcomp(mwp1, compl) or
        /// pobj|pcomp(mwp0, compl) -> prep_mwp0_mwp1(gov, compl)
        /// </summary>
        /// <param name="list">List of typedDependencies to work on</param>
        /// <param name="newTypedDeps">List of typedDependencies that we construct</param>
        /// <param name="strMwp0">First part of the multiword preposition to construct the collapsed preposition</param>
        /// <param name="strMwp1">Second part of the multiword preposition to construct the collapsed preposition</param>
        /// <param name="wMwp0">First part of the multiword preposition that we look for</param>
        /// <param name="wMwp1">Second part of the multiword preposition that we look for</param>
        private static void CollapseMultiWordPrep(List<TypedDependency> list, List<TypedDependency> newTypedDeps,
            string strMwp0, string strMwp1, string wMwp0, string wMwp1)
        {

            // first find the multiword_preposition: dep(mpw[0], mwp[1])
            // the two words should be next to another in the sentence (difference of
            // indexes = 1)
            IndexedWord mwp0 = null;
            IndexedWord mwp1 = null;
            TypedDependency dep = null;
            foreach (TypedDependency td in list)
            {
                if (td.Gov.Value().Equals(wMwp0, StringComparison.InvariantCultureIgnoreCase) &&
                    td.Dep.Value().Equals(wMwp1, StringComparison.InvariantCultureIgnoreCase) &&
                    Math.Abs(td.Gov.Index() - td.Dep.Index()) == 1)
                {
                    mwp0 = td.Gov;
                    mwp1 = td.Dep;
                    dep = td;
                }
            }

            if (mwp0 == null)
            {
                return;
            }

            // now search for prep|advmod|dep|amod(gov, mwp0)
            IndexedWord governor = null;
            TypedDependency prep = null;
            foreach (TypedDependency td1 in list)
            {
                if ((td1.Reln == EnglishGrammaticalRelations.PrepositionalModifier ||
                     td1.Reln == EnglishGrammaticalRelations.AdverbialModifier ||
                     td1.Reln == EnglishGrammaticalRelations.AdjectivalModifier ||
                     td1.Reln == GrammaticalRelation.Dependent ||
                     td1.Reln == EnglishGrammaticalRelations.MultiWordExpression) && td1.Dep.Equals(mwp0))
                {
                    // we found prep|advmod|dep|amod(gov, mwp0)
                    prep = td1;
                    governor = prep.Gov;
                }
            }

            if (prep == null)
            {
                return;
            }

            // search for the complement: pobj|pcomp(mwp1,X)
            // or for pobj|pcomp(mwp0,X)
            // There may be more than one in weird constructions; if there are several,
            // take the one with the LOWEST index!
            TypedDependency pobj = null;
            TypedDependency newtd = null;
            foreach (TypedDependency td2 in list)
            {
                if ((td2.Reln == EnglishGrammaticalRelations.PrepositionalObject ||
                     td2.Reln == EnglishGrammaticalRelations.PrepositionalComplement) &&
                    (td2.Gov.Equals(mwp1) || td2.Gov.Equals(mwp0)))
                {
                    if (pobj == null || pobj.Dep.Index() > td2.Dep.Index())
                    {
                        pobj = td2;
                        // create the new gr relation
                        GrammaticalRelation gr;
                        if (td2.Reln == EnglishGrammaticalRelations.PrepositionalComplement)
                        {
                            gr = EnglishGrammaticalRelations.GetPrepC(strMwp0 + '_' + strMwp1);
                        }
                        else
                        {
                            gr = EnglishGrammaticalRelations.GetPrep(strMwp0 + '_' + strMwp1);
                        }
                        if (governor != null)
                        {
                            newtd = new TypedDependency(gr, governor, pobj.Dep);
                        }
                    }
                }
            }

            if (pobj == null || newtd == null)
            {
                return;
            }

            // only if we found the three parts, set to KILL and remove
            // and add the new one
            // Necessarily from the above: prep != null, dep != null, pobj != null, newtd != null

            prep.Reln = GrammaticalRelation.Kill;
            dep.Reln = GrammaticalRelation.Kill;
            pobj.Reln = GrammaticalRelation.Kill;
            newTypedDeps.Add(newtd);

            // now remove typed dependencies with reln "kill"
            // and promote possible orphans
            foreach (TypedDependency td1 in list)
            {
                if (td1.Reln != GrammaticalRelation.Kill)
                {
                    if (td1.Gov.Equals(mwp0) || td1.Gov.Equals(mwp1))
                    {
                        // CDM: Thought of adding this in Jan 2010, but it causes
                        // conflicting relations tmod vs. pobj. Needs more thought
                        // maybe restrict pobj to first NP in PP, and allow tmod for a later
                        // one?
                        if (td1.Reln == EnglishGrammaticalRelations.TemporalModifier)
                        {
                            // special case when an extra NP-TMP is buried in a PP for
                            // "during the same period last year"
                            td1.Gov = pobj.Dep;
                        }
                        else
                        {
                            td1.Gov = governor;
                        }
                    }
                    if (!newTypedDeps.Contains(td1))
                    {
                        newTypedDeps.Add(td1);
                    }
                }
            }
            list.Clear();
            list.AddRange(newTypedDeps);
        }
    }
}