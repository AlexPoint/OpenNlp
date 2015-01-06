using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Trees.TRegex;
using OpenNLP.Tools.Util.Trees.TRegex.Tsurgeon;

namespace OpenNLP.Tools.Util.Trees
{
    /**
 * Transforms an English structure parse tree in order to get the dependencies right:  <br>
 *  -- put a ROOT node  <br>
 *  -- remove NONE nodes  <br>
 *  -- retain only NP-TMP, NP-ADV, UCP-TMP tags  <br>
 * The UCP- tags will later be turned into NP- anyway <br>
 *
 * (Note [cdm]: A lot of this overlaps other existing functionality in trees.
 * Could aim to unify it.)
 *
 * @author mcdm
 */

    public class DependencyTreeTransformer : TreeTransformer
    {
        private static readonly Regex TmpPattern = new Regex("(NP|UCP).*-TMP.*", RegexOptions.Compiled);
        private static readonly Regex AdvPattern = new Regex("(NP|UCP).*-ADV.*", RegexOptions.Compiled);
        protected readonly AbstractTreebankLanguagePack tlp;

        public DependencyTreeTransformer()
        {
            tlp = new PennTreebankLanguagePack();
        }

        //@Override
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
            if (label == null || label.Equals("TOP"))
            {
                return "ROOT";
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
            label = tlp.BasicCategory(label);
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