using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenNLP.Tools.Util;

namespace OpenNLP.Tools.Trees.TRegex
{
    public class DescriptionPattern : TregexPattern
    {
        private enum DescriptionMode
        {
            PATTERN,
            STRINGS,
            EXACT,
            ANYTHING
        }

        private readonly Relation rel;
        private readonly bool negDesc;

        private readonly DescriptionMode? descriptionMode;
        private readonly string exactMatch;
        private readonly Regex descPattern;
        private readonly Predicate<string> stringFilter;

        /// <summary>
        /// What size string matchers to use before switching to regex for disjunction matches
        /// </summary>
        private const int MaxStringMatcherSize = 8;

        private readonly string stringDesc;
        /// <summary>The name to give the matched node</summary>
        private readonly string name;
        /// <summary>If this pattern is a link, this is the node linked to</summary>
        private readonly string linkedName;
        private readonly bool isLink;
        // TODO: conceptually readonly, but we'd need to rewrite TregexParser to make it so.
        private TregexPattern child;
        /// <summary>also conceptually readonly, but it depends on the child</summary>
        private readonly List<Tuple<int, string>> variableGroups;
        /// <summary>specifies the groups in a regex that are captured as matcher-global string variables</summary>
        private readonly Func<string, string> basicCatFunction;

        /// <summary>
        /// Used to detect regex expressions which can be simplified to exact matches
        /// </summary>
        private static readonly Regex SingleWordPattern = new Regex("/\\^(.)\\$/" + "|" + // for example, /^:$/
                                                                      "/\\^\\[(.)\\]\\$/" + "|" +
                                                                      // for example, /^[$]$/
                                                                      "/\\^([-a-zA-Z']+)\\$/");

        /// <summary>for example, /^-NONE-$/</summary>
        private static readonly Regex MultiWordPattern = new Regex("/\\^\\(\\?\\:((?:[-a-zA-Z|]|\\\\\\$)+)\\)\\$\\/");

        private static readonly Regex CaseInsensitivePattern =
            new Regex("/\\^\\(\\?i\\:((?:[-a-zA-Z|]|\\\\\\$)+)\\)\\$\\/");

        /// <summary>
        /// Used to detect regex expressions which can be simplified to exact matches
        /// </summary>
        private static readonly Regex PrefixPattern = new Regex("/\\^([-a-zA-Z|]+)\\/" + "|" + // for example, /^JJ/
                                                                 "/\\^\\(\\?\\:([-a-zA-Z|]+)\\)\\/");

        public DescriptionPattern(Relation rel, bool negDesc, string desc,
            string name, bool useBasicCat,
            Func<string, string> basicCatFunction,
            List<Tuple<int, string>> variableGroups,
            bool isLink, string linkedName)
        {
            this.rel = rel;
            this.negDesc = negDesc;
            this.isLink = isLink;
            this.linkedName = linkedName;
            if (desc != null)
            {
                stringDesc = desc;
                // TODO: factor out some of these blocks of code
                if (desc.Equals("__") || desc.Equals("/.*/") || desc.Equals("/^.*$/"))
                {
                    descriptionMode = DescriptionMode.ANYTHING;
                    descPattern = null;
                    exactMatch = null;
                    stringFilter = null;
                }
                else if (SingleWordPattern.IsMatch(desc))
                {
                    // Expressions are written like this to put special characters
                    // in the tregex matcher, but a regular expression is less
                    // efficient than a simple string match
                    descriptionMode = DescriptionMode.EXACT;
                    descPattern = null;
                    var matcher = SingleWordPattern.Match(desc);
                    //matcher.matches();
                    string matchedGroup = null;
                    for (int i = 1; i <= matcher.Groups.Count; ++i)
                    {
                        if (!string.IsNullOrEmpty(matcher.Groups[i].Value))
                        {
                            matchedGroup = matcher.Groups[i].Value;
                            break;
                        }
                    }
                    exactMatch = matchedGroup;
                    stringFilter = null;
                }
                else if (MultiWordPattern.IsMatch(desc))
                {
                    var matcher = MultiWordPattern.Match(desc);
                    //matcher.matches();
                    string matchedGroup = null;
                    for (int i = 1; i <= matcher.Groups.Count; ++i)
                    {
                        if (!string.IsNullOrEmpty(matcher.Groups[i].Value))
                        {
                            matchedGroup = matcher.Groups[i].Value;
                            break;
                        }
                    }
                    //matchedGroup = matchedGroup.Replace("\\\\", "");
                    matchedGroup = matchedGroup.Replace("\\", "");
                    //if (matchedGroup.Split(new []{"[|]"}, StringSplitOptions.None).Length > MAX_STRING_MATCHER_SIZE) {
                    if (matchedGroup.Split('|').Length > MaxStringMatcherSize)
                    {
                        descriptionMode = DescriptionMode.PATTERN;
                        //descPattern = new Regex(desc.Substring(1, desc.Length - 1));
                        descPattern = new Regex(desc.Substring(1, desc.Length - 2));
                        exactMatch = null;
                        stringFilter = null;
                    }
                    else
                    {
                        descriptionMode = DescriptionMode.STRINGS;
                        descPattern = null;
                        exactMatch = null;
                        //stringFilter = new ArrayStringFilter(ArrayStringFilter.Mode.EXACT, matchedGroup.Split(new []{"[|]"}, StringSplitOptions.None)); 
                        stringFilter = a => matchedGroup.Split('|').Any(s => s == a);
                    }
                }
                else if (CaseInsensitivePattern.IsMatch(desc))
                {
                    var matcher = CaseInsensitivePattern.Match(desc);
                    //matcher.matches();
                    string matchedGroup = null;
                    for (int i = 1; i <= matcher.Groups.Count; ++i)
                    {
                        if (!string.IsNullOrEmpty(matcher.Groups[i].Value))
                        {
                            matchedGroup = matcher.Groups[i].Value;
                            break;
                        }
                    }
                    //matchedGroup = matchedGroup.Replace("\\\\", "");
                    matchedGroup = matchedGroup.Replace("\\", "");
                    //if (matchedGroup.Split(new []{"[|]"}, StringSplitOptions.None).Length > MAX_STRING_MATCHER_SIZE) {
                    if (matchedGroup.Split('|').Length > MaxStringMatcherSize)
                    {
                        descriptionMode = DescriptionMode.PATTERN;
                        //descPattern = new Regex(desc.Substring(1, desc.Length - 1));
                        descPattern = new Regex(desc.Substring(1, desc.Length - 2));
                        exactMatch = null;
                        stringFilter = null;
                    }
                    else
                    {
                        descriptionMode = DescriptionMode.STRINGS;
                        descPattern = null;
                        exactMatch = null;
                        //stringFilter = new ArrayStringFilter(ArrayStringFilter.Mode.CASE_INSENSITIVE, matchedGroup.Split(new []{"[|]"}, StringSplitOptions.None)); 
                        stringFilter =
                            a =>
                                matchedGroup.Split('|')
                                    .Any(s => s.Equals(a, StringComparison.InvariantCultureIgnoreCase));
                    }
                }
                else if (PrefixPattern.IsMatch(desc))
                {
                    var matcher = PrefixPattern.Match(desc);
                    //matcher.matches();
                    string matchedGroup = null;
                    for (int i = 1; i <= matcher.Groups.Count; ++i)
                    {
                        if (!string.IsNullOrEmpty(matcher.Groups[i].Value))
                        {
                            matchedGroup = matcher.Groups[i].Value;
                            break;
                        }
                    }
                    //if (matchedGroup.Split(new []{"\\|"}, StringSplitOptions.None).Length > MAX_STRING_MATCHER_SIZE) {
                    if (matchedGroup.Split('|').Length > MaxStringMatcherSize)
                    {
                        descriptionMode = DescriptionMode.PATTERN;
                        //descPattern = new Regex(desc.Substring(1, desc.Length - 1));
                        descPattern = new Regex(desc.Substring(1, desc.Length - 2));
                        exactMatch = null;
                        stringFilter = null;
                    }
                    else
                    {
                        descriptionMode = DescriptionMode.STRINGS;
                        descPattern = null;
                        exactMatch = null;
                        //stringFilter = new ArrayStringFilter(ArrayStringFilter.Mode.PREFIX, matchedGroup.Split(new []{"[|]"}, StringSplitOptions.None)); 
                        stringFilter = a => matchedGroup.Split('|').Any(s => a.StartsWith(s));
                    }
                }
                else if (Regex.IsMatch(desc, "/.*/"))
                {
                    descriptionMode = DescriptionMode.PATTERN;
                    //descPattern = new Regex(desc.Substring(1, desc.Length - 1));
                    descPattern = new Regex(desc.Substring(1, desc.Length - 2));
                    exactMatch = null;
                    stringFilter = null;
                }
                else if (desc.IndexOf('|') >= 0)
                {
                    // patterns which contain ORs are a special case; we either
                    // promote those to regex match or make a string matcher out
                    // of them.  for short enough disjunctions, a simple string
                    // matcher can be more efficient than a regex.
                    //string[] words = desc.Split(new []{"[|]"}, StringSplitOptions.None);
                    string[] words = desc.Split('|');
                    if (words.Length <= MaxStringMatcherSize)
                    {
                        descriptionMode = DescriptionMode.STRINGS;
                        descPattern = null;
                        exactMatch = null;
                        //stringFilter = new ArrayStringFilter(ArrayStringFilter.Mode.EXACT, words);
                        stringFilter = a => words.Any(s => s == a);
                    }
                    else
                    {
                        descriptionMode = DescriptionMode.PATTERN;
                        descPattern = new Regex("^(?:" + desc + ")$");
                        exactMatch = null;
                        stringFilter = null;
                    }
                }
                else
                {
                    // raw description
                    descriptionMode = DescriptionMode.EXACT;
                    descPattern = null;
                    exactMatch = desc;
                    stringFilter = null;
                }
            }
            else
            {
                if (name == null && linkedName == null)
                {
                    throw new InvalidDataException(
                        "Illegal description pattern.  Does not describe a node or link/name a variable");
                }
                stringDesc = " ";
                descriptionMode = null;
                descPattern = null;
                exactMatch = null;
                stringFilter = null;
            }
            this.name = name;
            SetChild(null);
            this.basicCatFunction = (useBasicCat ? basicCatFunction : null);
            this.variableGroups = variableGroups;
        }

        public DescriptionPattern(Relation newRelation, DescriptionPattern oldPattern)
        {
            this.rel = newRelation;
            this.negDesc = oldPattern.negDesc;
            this.isLink = oldPattern.isLink;
            this.linkedName = oldPattern.linkedName;
            this.stringDesc = oldPattern.stringDesc;
            this.descriptionMode = oldPattern.descriptionMode;
            this.descPattern = oldPattern.descPattern;
            this.exactMatch = oldPattern.exactMatch;
            this.stringFilter = oldPattern.stringFilter;
            this.name = oldPattern.name;
            this.SetChild(oldPattern.child);
            this.basicCatFunction = oldPattern.basicCatFunction;
            this.variableGroups = oldPattern.variableGroups;
        }

        public override string LocalString()
        {
            return rel.ToString() + ' ' + (negDesc ? "!" : "") + (basicCatFunction != null ? "@" : "") + stringDesc +
                   (name == null ? "" : '=' + name);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (IsNegated())
            {
                sb.Append('!');
            }
            if (IsOptional())
            {
                sb.Append('?');
            }
            sb.Append(rel.ToString());
            sb.Append(' ');
            if (child != null)
            {
                sb.Append('(');
            }
            if (negDesc)
            {
                sb.Append('!');
            }
            if (basicCatFunction != null)
            {
                sb.Append('@');
            }
            sb.Append(stringDesc);
            if (isLink)
            {
                sb.Append('~');
                sb.Append(linkedName);
            }
            if (name != null)
            {
                sb.Append('=');
                sb.Append(name);
            }
            sb.Append(' ');
            if (child != null)
            {
                sb.Append(child.ToString());
                sb.Append(')');
            }
            return sb.ToString();
        }

        public void SetChild(TregexPattern n)
        {
            child = n;
        }

        public override List<TregexPattern> GetChildren()
        {
            if (child == null)
            {
                return new List<TregexPattern>();
            }
            else
            {
                return new List<TregexPattern>() {child};
            }
        }

        public override TregexMatcher Matcher(Tree root, Tree tree,
            IdentityDictionary<Tree, Tree> nodesToParents,
            Dictionary<string, Tree> namesToNodes,
            VariableStrings variableStrings,
            IHeadFinder headFinder)
        {
            return new DescriptionMatcher(this, root, tree, nodesToParents,
                namesToNodes, variableStrings, headFinder);
        }

        // TODO: Why is this a static class with a pointer to the containing class?  There seems to be no reason for such a thing.
        // cdm: agree: It seems like it should just be a non-static inner class.  Try this and check it works....
        private class DescriptionMatcher : TregexMatcher
        {
            private IEnumerator<Tree> treeNodeMatchCandidateIterator;
            private readonly DescriptionPattern myNode;

            // a DescriptionMatcher only has a single child; if it is the left
            // side of multiple relations, a CoordinationMatcher is used.

            // childMatcher is null until the first time a matcher needs to check the child 

            // myNode.child == null OR resetChild has never been called
            private TregexMatcher childMatcher;

            /// <summary>
            /// The Tree node that this DescriptionMatcher node is trying to match on
            /// </summary>
            private Tree nextTreeNodeMatchCandidate;
            
            /// <summary>
            /// when finished = true, it means I have exhausted my potential tree node match candidates
            /// </summary>
            private bool finished = false;

            private bool matchedOnce = false;
            private bool committedVariables = false;


            public DescriptionMatcher(DescriptionPattern n, Tree root, Tree tree,
                IdentityDictionary<Tree, Tree> nodesToParents,
                Dictionary<string, Tree> namesToNodes,
                VariableStrings variableStrings,
                IHeadFinder headFinder) :
                    base(root, tree, nodesToParents, namesToNodes, variableStrings, headFinder)
            {
                myNode = n;
                // no need to reset anything - everything starts out as null or false.  
                // lazy initialization of children to save time.
                // resetChildIter();
            }

            public override void ResetChildIter()
            {
                DecommitVariableGroups();
                RemoveNamedNodes();
                // lazy initialization saves quite a bit of time in use cases
                // where we call something other than matches()
                treeNodeMatchCandidateIterator = null;
                finished = false;
                nextTreeNodeMatchCandidate = null;
                if (childMatcher != null)
                {
                    // need to tell the children to clean up any preexisting data
                    childMatcher.ResetChildIter();
                }
            }

            private void ResetChild()
            {
                if (childMatcher == null)
                {
                    if (myNode.child == null)
                    {
                        matchedOnce = false;
                    }
                }
                else
                {
                    childMatcher.ResetChildIter(nextTreeNodeMatchCandidate);
                }
            }

            /// <summary>
            /// goes to the next node in the tree that is a successful match to my description pattern.
            /// This is the hotspot method in running tregex, but not clear how to make it faster.
            /// When finished = false; break; is called, it means I successfully matched
            /// </summary>
            private void GoToNextTreeNodeMatch()
            {
                //Console.WriteLine("goToNextTreeNodeMatch()");
                DecommitVariableGroups(); // make sure variable groups are free.
                RemoveNamedNodes(); // if we named a node, it should now be unnamed
                finished = true;
                Match m = null;
                string value = null;
                if (treeNodeMatchCandidateIterator == null)
                {
                    treeNodeMatchCandidateIterator = myNode.rel.GetSearchNodeIterator(tree, this);
                }
                while (treeNodeMatchCandidateIterator.MoveNext())
                {
                    //Console.WriteLine("success = true");
                    nextTreeNodeMatchCandidate = treeNodeMatchCandidateIterator.Current;
                    if (myNode.descriptionMode == null)
                    {
                        //Console.WriteLine("myNode.descriptionMode == null");
                        // this is a backreference or link
                        if (myNode.isLink)
                        {
                            //Console.WriteLine("myNode.isLink");
                            Tree otherTree = namesToNodes[myNode.linkedName];
                            if (otherTree != null)
                            {
                                //Console.WriteLine("otherTree != null");
                                string otherValue = myNode.basicCatFunction == null
                                    ? otherTree.Value()
                                    : myNode.basicCatFunction(otherTree.Value());
                                string myValue = myNode.basicCatFunction == null
                                    ? nextTreeNodeMatchCandidate.Value()
                                    : myNode.basicCatFunction(nextTreeNodeMatchCandidate.Value());
                                if (otherValue.Equals(myValue))
                                {
                                    //Console.WriteLine("otherValue.Equals(myValue)");
                                    finished = false;
                                    break;
                                }
                            }
                        }
                        else if (namesToNodes[myNode.name] == nextTreeNodeMatchCandidate)
                        {
                            //Console.WriteLine("namesToNodes[myNode.name] == nextTreeNodeMatchCandidate");
                            finished = false;
                            break;
                        }
                    }
                    else
                    {
                        // try to match the description pattern.
                        // cdm: Nov 2006: Check for null label, just make found false
                        // string value = (myNode.basicCatFunction == null ? nextTreeNodeMatchCandidate.value() : myNode.basicCatFunction.apply(nextTreeNodeMatchCandidate.value()));
                        // m = myNode.descPattern.matcher(value);
                        // bool found = m.find();
                        //Console.WriteLine("else");
                        bool found;
                        value = nextTreeNodeMatchCandidate.Value();
                        if (value == null)
                        {
                            found = false;
                        }
                        else
                        {
                            if (myNode.basicCatFunction != null)
                            {
                                value = myNode.basicCatFunction(value);
                            }
                            switch (myNode.descriptionMode)
                            {
                                case DescriptionMode.EXACT:
                                    found = value.Equals(myNode.exactMatch);
                                    break;
                                case DescriptionMode.PATTERN:
                                    m = myNode.descPattern.Match(value);
                                    found = m.Success;
                                    break;
                                case DescriptionMode.ANYTHING:
                                    found = true;
                                    break;
                                case DescriptionMode.STRINGS:
                                    found = myNode.stringFilter(value);
                                    break;
                                default:
                                    throw new ArgumentException("Unexpected match mode");
                            }
                        }
                        if (found)
                        {
                            //Console.WriteLine("found = true");
                            foreach (Tuple<int, string> varGroup in myNode.variableGroups)
                            {
                                // if variables have been captured from a regex, they must match any previous matchings
                                string thisVariable = varGroup.Item2;
                                string thisVarString = variableStrings.GetString(thisVariable);
                                if (m != null)
                                {
                                    if (thisVarString != null &&
                                        !thisVarString.Equals(m.Groups[varGroup.Item1].Value))
                                    {
                                        // failed to match a variable
                                        found = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    if (thisVarString != null &&
                                        !thisVarString.Equals(value))
                                    {
                                        // here we treat any variable group # as a match
                                        found = false;
                                        break;
                                    }
                                }
                            }
                        }
                        if (found != myNode.negDesc)
                        {
                            //Console.WriteLine("found != myNode.negDesc");
                            finished = false;
                            break;
                        }
                    }
                }
                if (!finished)
                {
                    // I successfully matched.
                    //Console.WriteLine("!finished");
                    ResetChild();
                        // reset my unique TregexMatcher child based on the Tree node I successfully matched at.
                    // cdm bugfix jul 2009: on next line need to check for descPattern not null, or else this is a backreference or a link to an already named node, and the map should _not_ be updated
                    if ((myNode.descriptionMode != null || myNode.isLink) && myNode.name != null)
                    {
                        // note: have to fill in the map as we go for backreferencing
                        namesToNodes.Add(myNode.name, nextTreeNodeMatchCandidate);
                    }
                    if (m != null)
                    {
                        // commit variable groups using a matcher, meaning
                        // it extracts the expressions from that matcher
                        CommitVariableGroups(m);
                    }
                    else if (value != null)
                    {
                        // commit using a set string (all groups are treated as the string)
                        CommitVariableGroups(value);
                    }
                }
                // finished is false exiting this if and only if nextChild exists
                // and has a label or backreference that matches
                // (also it will just have been reset)
            }

            private void CommitVariableGroups(Match m)
            {
                committedVariables = true; // commit all my variable groups.
                foreach (Tuple<int, string> varGroup in myNode.variableGroups)
                {
                    string thisVarString = m.Groups[varGroup.Item1].Value;
                    variableStrings.SetVar(varGroup.Item2, thisVarString);
                }
            }

            private void CommitVariableGroups(string value)
            {
                committedVariables = true;
                foreach (Tuple<int, string> varGroup in myNode.variableGroups)
                {
                    variableStrings.SetVar(varGroup.Item2, value);
                }
            }

            private void DecommitVariableGroups()
            {
                if (committedVariables)
                {
                    foreach (Tuple<int, string> varGroup in myNode.variableGroups)
                    {
                        variableStrings.UnsetVar(varGroup.Item2);
                    }
                }
                committedVariables = false;
            }

            private void RemoveNamedNodes()
            {
                if ((myNode.descriptionMode != null || myNode.isLink) &&
                    myNode.name != null)
                {
                    namesToNodes.Remove(myNode.name);
                }
            }

            /// <summary>
            /// Tries to match the unique child of the DescriptionPattern node to a Tree node.
            /// </summary>
            /// <returns>"true" if succeeds</returns>
            private bool MatchChild()
            {
                //Console.WriteLine("matchChild()");
                // entering here (given that it's called only once in matches())
                // we know finished is false, and either nextChild == null
                // (meaning goToNextChild has not been called) or nextChild exists
                // and has a label or backreference that matches
                if (nextTreeNodeMatchCandidate == null)
                {
                    // I haven't been initialized yet, so my child certainly can't be matched yet.
                    return false;
                }
                // lazy initialization of the child matcher
                if (childMatcher == null && myNode.child != null)
                {
                    childMatcher = myNode.child.Matcher(root, nextTreeNodeMatchCandidate, nodesToParents, namesToNodes,
                        variableStrings, headFinder);
                    //childMatcher.resetChildIter();
                }
                if (childMatcher == null)
                {
                    if (!matchedOnce)
                    {
                        matchedOnce = true;
                        return true;
                    }
                    return false;
                }
                return childMatcher.Matches();
            }

            /// <summary>
            /// find the next local match
            /// </summary>
            public override bool Matches()
            {
                //Console.WriteLine("matches()");
                // this is necessary so that a negated/optional node matches only once
                if (finished)
                {
                    return false;
                }
                while (!finished)
                {
                    if (MatchChild())
                    {
                        if (myNode.IsNegated())
                        {
                            // negated node only has to fail once
                            finished = true;
                            return false; // cannot be optional and negated
                        }
                        else
                        {
                            if (myNode.IsOptional())
                            {
                                finished = true;
                            }
                            return true;
                        }
                    }
                    else
                    {
                        GoToNextTreeNodeMatch();
                    }
                }
                if (myNode.IsNegated())
                {
                    // couldn't match my relation/pattern, so succeeded!
                    return true;
                }
                else
                {
                    // couldn't match my relation/pattern, so failed!
                    DecommitVariableGroups();
                    RemoveNamedNodes();
                    nextTreeNodeMatchCandidate = null;
                    // didn't match, but return true anyway if optional
                    return myNode.IsOptional();
                }
            }

            public override Tree GetMatch()
            {
                return nextTreeNodeMatchCandidate;
            }

        } // end class DescriptionMatcher

    }
}