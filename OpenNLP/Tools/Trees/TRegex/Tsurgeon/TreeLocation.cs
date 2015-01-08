using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees.TRegex.Tsurgeon
{
    public class TreeLocation
    {
        private readonly string relation;

        private readonly TsurgeonPattern child;

        public TreeLocation(string relation, TsurgeonPattern p)
        {
            this.relation = relation;
            this.child = p;
        }

        public void SetRoot(TsurgeonPatternRoot root)
        {
            child.SetRoot(root);
        }

        private static readonly Regex DaughterPattern = new Regex(">-?([0-9]+)", RegexOptions.Compiled);

        public LocationMatcher Matcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer)
        {
            return new LocationMatcher(newNodeNames, coindexer, this);
        }

        /** TODO: it would be nice to refactor this with TsurgeonMatcher somehow */

        public class LocationMatcher
        {
            private Dictionary<string, Tree> newNodeNames;
            private CoindexationGenerator coindexer;
            private readonly TreeLocation location;

            private readonly TsurgeonMatcher childMatcher;

            public LocationMatcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer,
                TreeLocation location)
            {
                this.newNodeNames = newNodeNames;
                this.coindexer = coindexer;
                this.location = location;

                this.childMatcher = location.child.GetMatcher(newNodeNames, coindexer);
            }

            public Tuple<Tree, int> Evaluate(Tree tree, TregexMatcher tregex)
            {
                int newIndex = -1;
                Tree parent = null;
                Tree relativeNode = childMatcher.Evaluate(tree, tregex);
                //Matcher m = daughterPattern.matcher(relation);
                var isMatch = DaughterPattern.IsMatch(location.relation);
                if ( /*m.matches()*/isMatch)
                {
                    newIndex = int.Parse(DaughterPattern.Match(location.relation).Groups[1].Value) - 1;
                    parent = relativeNode;
                    if (location.relation[1] == '-') // backwards.
                        newIndex = parent.Children().Length - newIndex;
                }
                else
                {
                    parent = relativeNode.Parent(tree);
                    if (parent == null)
                    {
                        throw new SystemException("Error: looking for a non-existent parent in tree " + tree + " for \"" +
                                                  ToString() + "\"");
                    }
                    int index = parent.ObjectIndexOf(relativeNode);
                    if (location.relation.Equals("$+"))
                    {
                        newIndex = index;
                    }
                    else if (location.relation.Equals("$-"))
                    {
                        newIndex = index + 1;
                    }
                    else
                    {
                        throw new SystemException("Error: Haven't dealt with relation " + location.relation + " yet.");
                    }
                }
                return new Tuple<Tree, int>(parent, newIndex);
            }
        }

        public override string ToString()
        {
            return relation + " " + child;
        }
    }
}