using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenNLP.Tools.Trees.TRegex;
using OpenNLP.Tools.Trees.TRegex.Tsurgeon;

namespace OpenNLP.Tools.Trees
{
    /// <summary>
    /// Transforms an English structure parse tree in order to get the dependencies right:
    /// -- put a ROOT node
    /// -- remove NONE nodes
    /// -- retain only NP-TMP, NP-ADV, UCP-TMP tags
    /// The UCP- tags will later be turned into NP- anyway
    /// 
    /// (Note [cdm]: A lot of this overlaps other existing functionality in trees. Could aim to unify it.)
    /// 
    /// @author mcdm
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public class DependencyTreeTransformer : ITreeTransformer
    {
        private static readonly Regex TmpPattern = new Regex("(NP|UCP).*-TMP.*", RegexOptions.Compiled);
        private static readonly Regex AdvPattern = new Regex("(NP|UCP).*-ADV.*", RegexOptions.Compiled);
        protected readonly AbstractTreebankLanguagePack Tlp;

        public DependencyTreeTransformer()
        {
            Tlp = new PennTreebankLanguagePack();
        }

        public Tree TransformTree(Tree t)
        {
            //deal with empty root
            t.SetValue(CleanUpRoot(t.Value()));
            //strips tags
            StripTag(t);

            // strip empty nodes
            return StripEmptyNode(t);
        }

        protected static string CleanUpRoot(string label)
        {
            if (label == null || label.Equals(AbstractCollinsHeadFinder.TOP))
            {
                return AbstractCollinsHeadFinder.ROOT;
                // string constants are always interned
            }
            else
            {
                return label;
            }
        }

        // only leaves NP-TMP and NP-ADV
        protected string CleanUpLabel(string label)
        {
            if (label == null)
            {
                return "";
                    // This shouldn't really happen, but can happen if there are unlabeled nodes further down a tree, as apparently happens in at least the 20100730 era American National Corpus
            }
            bool nptemp = TmpPattern.IsMatch(label);
            bool npadv = AdvPattern.IsMatch(label);
            label = Tlp.BasicCategory(label);
            if (nptemp)
            {
                label = label + "-TMP";
            }
            else if (npadv)
            {
                label = label + "-ADV";
            }
            return label;
        }

        protected void StripTag(Tree t)
        {
            if (! t.IsLeaf())
            {
                string label = CleanUpLabel(t.Value());
                t.SetValue(label);
                foreach (Tree child in t.GetChildrenAsList())
                {
                    StripTag(child);
                }
            }
        }

        private static readonly TregexPattern MatchPattern =
            TregexPattern.SafeCompile("-NONE-=none", true);

        private static readonly TsurgeonPattern Operation =
            Tsurgeon.ParseOperation("prune none");

        protected static Tree StripEmptyNode(Tree t)
        {
            return Tsurgeon.ProcessPattern(MatchPattern, Operation, t);
        }
    }
}