using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees.TRegex.Tsurgeon
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

        public void setRoot(TsurgeonPatternRoot root)
        {
            child.setRoot(root);
        }

        private static readonly Regex daughterPattern = new Regex(">-?([0-9]+)", RegexOptions.Compiled);

        public LocationMatcher matcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer)
        {
            return new LocationMatcher(newNodeNames, coindexer, this);
        }

        /** TODO: it would be nice to refactor this with TsurgeonMatcher somehow */

        public class LocationMatcher
        {
            private Dictionary<string, Tree> newNodeNames;
            private CoindexationGenerator coindexer;
            private TreeLocation location;

            private TsurgeonMatcher childMatcher;

            public LocationMatcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer,
                TreeLocation location)
            {
                this.newNodeNames = newNodeNames;
                this.coindexer = coindexer;
                this.location = location;

                this.childMatcher = location.child.matcher(newNodeNames, coindexer);
            }

            public Tuple<Tree, int> evaluate(Tree tree, TregexMatcher tregex)
            {
                int newIndex = -1;
                Tree parent = null;
                Tree relativeNode = childMatcher.evaluate(tree, tregex);
                //Matcher m = daughterPattern.matcher(relation);
                var isMatch = daughterPattern.IsMatch(location.relation);
                if ( /*m.matches()*/isMatch)
                {
                    newIndex = int.Parse(daughterPattern.Match(location.relation).Groups[1].Value) - 1;
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

        //@Override
        public override string ToString()
        {
            return relation + " " + child;
        }
    }
}