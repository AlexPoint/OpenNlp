using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees.TRegex
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

        // what size string matchers to use before switching to regex for
        // disjunction matches
        private static readonly int MAX_STRING_MATCHER_SIZE = 8;

        private readonly string stringDesc;
        /** The name to give the matched node */
        private readonly string name;
        /** If this pattern is a link, this is the node linked to */
        private readonly string linkedName;
        private readonly bool isLink;
        // todo: conceptually readonly, but we'd need to rewrite TregexParser
        // to make it so.
        private TregexPattern child;
        // also conceptually readonly, but it depends on the child
        private readonly List<Tuple<int, string>> variableGroups;
            // specifies the groups in a regex that are captured as matcher-global string variables

        private readonly Func<string, string> basicCatFunction;

        /** Used to detect regex expressions which can be simplified to exact matches */

        private static readonly Regex SINGLE_WORD_PATTERN = new Regex("/\\^(.)\\$/" + "|" + // for example, /^:$/
                                                                      "/\\^\\[(.)\\]\\$/" + "|" +
                                                                      // for example, /^[$]$/
                                                                      "/\\^([-a-zA-Z']+)\\$/");
            // for example, /^-NONE-$/

        private static readonly Regex MULTI_WORD_PATTERN = new Regex("/\\^\\(\\?\\:((?:[-a-zA-Z|]|\\\\\\$)+)\\)\\$\\/");

        private static readonly Regex CASE_INSENSITIVE_PATTERN =
            new Regex("/\\^\\(\\?i\\:((?:[-a-zA-Z|]|\\\\\\$)+)\\)\\$\\/");

        /** Used to detect regex expressions which can be simplified to exact matches */

        private static readonly Regex PREFIX_PATTERN = new Regex("/\\^([-a-zA-Z|]+)\\/" + "|" + // for example, /^JJ/
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
                else if (SINGLE_WORD_PATTERN.IsMatch(desc))
                {
                    // Expressions are written like this to put special characters
                    // in the tregex matcher, but a regular expression is less
                    // efficient than a simple string match
                    descriptionMode = DescriptionMode.EXACT;
                    descPattern = null;
                    var matcher = SINGLE_WORD_PATTERN.Match(desc);
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
                    //System.err.println("DescriptionPattern: converting " + desc + " to " + exactMatch);
                }
                else if (MULTI_WORD_PATTERN.IsMatch(desc))
                {
                    var matcher = MULTI_WORD_PATTERN.Match(desc);
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
                    if (matchedGroup.Split('|').Length > MAX_STRING_MATCHER_SIZE)
                    {
                        descriptionMode = DescriptionMode.PATTERN;
                        //descPattern = new Regex(desc.Substring(1, desc.Length - 1));
                        descPattern = new Regex(desc.Substring(1, desc.Length - 2));
                        exactMatch = null;
                        stringFilter = null;
                        //System.err.println("DescriptionPattern: not converting " + desc);
                    }
                    else
                    {
                        descriptionMode = DescriptionMode.STRINGS;
                        descPattern = null;
                        exactMatch = null;
                        //stringFilter = new ArrayStringFilter(ArrayStringFilter.Mode.EXACT, matchedGroup.Split(new []{"[|]"}, StringSplitOptions.None)); 
                        stringFilter = a => matchedGroup.Split('|').Any(s => s == a);
                        //System.err.println("DescriptionPattern: converting " + desc + " to " + stringFilter);
                    }
                }
                else if (CASE_INSENSITIVE_PATTERN.IsMatch(desc))
                {
                    var matcher = CASE_INSENSITIVE_PATTERN.Match(desc);
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
                    if (matchedGroup.Split('|').Length > MAX_STRING_MATCHER_SIZE)
                    {
                        descriptionMode = DescriptionMode.PATTERN;
                        //descPattern = new Regex(desc.Substring(1, desc.Length - 1));
                        descPattern = new Regex(desc.Substring(1, desc.Length - 2));
                        exactMatch = null;
                        stringFilter = null;
                        //System.err.println("DescriptionPattern: not converting " + desc);
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
                        //System.err.println("DescriptionPattern: converting " + desc + " to " + stringFilter);
                    }
                }
                else if (PREFIX_PATTERN.IsMatch(desc))
                {
                    var matcher = PREFIX_PATTERN.Match(desc);
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
                    if (matchedGroup.Split('|').Length > MAX_STRING_MATCHER_SIZE)
                    {
                        descriptionMode = DescriptionMode.PATTERN;
                        //descPattern = new Regex(desc.Substring(1, desc.Length - 1));
                        descPattern = new Regex(desc.Substring(1, desc.Length - 2));
                        exactMatch = null;
                        stringFilter = null;
                        //System.err.println("DescriptionPattern: not converting " + desc);
                    }
                    else
                    {
                        descriptionMode = DescriptionMode.STRINGS;
                        descPattern = null;
                        exactMatch = null;
                        //stringFilter = new ArrayStringFilter(ArrayStringFilter.Mode.PREFIX, matchedGroup.Split(new []{"[|]"}, StringSplitOptions.None)); 
                        stringFilter = a => matchedGroup.Split('|').Any(s => a.StartsWith(s));
                        //System.err.println("DescriptionPattern: converting " + desc + " to " + stringFilter);
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
                    if (words.Length <= MAX_STRING_MATCHER_SIZE)
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
            setChild(null);
            this.basicCatFunction = (useBasicCat ? basicCatFunction : null);
            //    System.out.println("Made " + (negDesc ? "negated " : "") + "DescNode with " + desc);
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
            this.setChild(oldPattern.child);
            this.basicCatFunction = oldPattern.basicCatFunction;
            this.variableGroups = oldPattern.variableGroups;
        }

        //@Override
        public override string localString()
        {
            return rel.ToString() + ' ' + (negDesc ? "!" : "") + (basicCatFunction != null ? "@" : "") + stringDesc +
                   (name == null ? "" : '=' + name);
        }

        //@Override
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (isNegated())
            {
                sb.Append('!');
            }
            if (isOptional())
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

        public void setChild(TregexPattern n)
        {
            child = n;
        }

        //@Override
        public override List<TregexPattern> getChildren()
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

        //@Override
        public override TregexMatcher matcher(Tree root, Tree tree,
            IdentityDictionary<Tree, Tree> nodesToParents,
            Dictionary<string, Tree> namesToNodes,
            VariableStrings variableStrings,
            HeadFinder headFinder)
        {
            return new DescriptionMatcher(this, root, tree, nodesToParents,
                namesToNodes, variableStrings, headFinder);
        }

        // TODO: Why is this a static class with a pointer to the containing
        // class?  There seems to be no reason for such a thing.
        // cdm: agree: It seems like it should just be a non-static inner class.  Try this and check it works....
        private /*static*/ class DescriptionMatcher : TregexMatcher
        {
            private IEnumerator<Tree> treeNodeMatchCandidateIterator;
            private readonly DescriptionPattern myNode;

            // a DescriptionMatcher only has a single child; if it is the left
            // side of multiple relations, a CoordinationMatcher is used.

            // childMatcher is null until the first time a matcher needs to check the child 

            // myNode.child == null OR resetChild has never been called
            private TregexMatcher childMatcher;

            private Tree nextTreeNodeMatchCandidate;
                // the Tree node that this DescriptionMatcher node is trying to match on.

            private bool finished = false;
                // when finished = true, it means I have exhausted my potential tree node match candidates.

            private bool matchedOnce = false;
            private bool committedVariables = false;


            public DescriptionMatcher(DescriptionPattern n, Tree root, Tree tree,
                IdentityDictionary<Tree, Tree> nodesToParents,
                Dictionary<string, Tree> namesToNodes,
                VariableStrings variableStrings,
                HeadFinder headFinder) :
                    base(root, tree, nodesToParents, namesToNodes, variableStrings, headFinder)
            {
                myNode = n;
                // no need to reset anything - everything starts out as null or false.  
                // lazy initialization of children to save time.
                // resetChildIter();
            }

            //@Override
            public override void resetChildIter()
            {
                decommitVariableGroups();
                removeNamedNodes();
                // lazy initialization saves quite a bit of time in use cases
                // where we call something other than matches()
                treeNodeMatchCandidateIterator = null;
                finished = false;
                nextTreeNodeMatchCandidate = null;
                if (childMatcher != null)
                {
                    // need to tell the children to clean up any preexisting data
                    childMatcher.resetChildIter();
                }
            }

            private void resetChild()
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
                    childMatcher.resetChildIter(nextTreeNodeMatchCandidate);
                }
            }

            /* goes to the next node in the tree that is a successful match to my description pattern.
     * This is the hotspot method in running tregex, but not clear how to make it faster. */
            // when finished = false; break; is called, it means I successfully matched.
            private void goToNextTreeNodeMatch()
            {
                Console.WriteLine("goToNextTreeNodeMatch()");
                decommitVariableGroups(); // make sure variable groups are free.
                removeNamedNodes(); // if we named a node, it should now be unnamed
                finished = true;
                Match m = null;
                string value = null;
                if (treeNodeMatchCandidateIterator == null)
                {
                    treeNodeMatchCandidateIterator = myNode.rel.searchNodeIterator(tree, this);
                }
//        var success = treeNodeMatchCandidateIterator.MoveNext();
                //while (success) {
                while (treeNodeMatchCandidateIterator.MoveNext())
                {
                    Console.WriteLine("success = true");
                    nextTreeNodeMatchCandidate = treeNodeMatchCandidateIterator.Current;
                    if (myNode.descriptionMode == null)
                    {
                        Console.WriteLine("myNode.descriptionMode == null");
                        // this is a backreference or link
                        if (myNode.isLink)
                        {
                            Console.WriteLine("myNode.isLink");
                            Tree otherTree = namesToNodes[myNode.linkedName];
                            if (otherTree != null)
                            {
                                Console.WriteLine("otherTree != null");
                                string otherValue = myNode.basicCatFunction == null
                                    ? otherTree.value()
                                    : myNode.basicCatFunction(otherTree.value());
                                string myValue = myNode.basicCatFunction == null
                                    ? nextTreeNodeMatchCandidate.value()
                                    : myNode.basicCatFunction(nextTreeNodeMatchCandidate.value());
                                if (otherValue.Equals(myValue))
                                {
                                    Console.WriteLine("otherValue.Equals(myValue)");
                                    finished = false;
                                    break;
                                }
                            }
                        }
                        else if (namesToNodes[myNode.name] == nextTreeNodeMatchCandidate)
                        {
                            Console.WriteLine("namesToNodes[myNode.name] == nextTreeNodeMatchCandidate");
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
                        Console.WriteLine("else");
                        bool found;
                        value = nextTreeNodeMatchCandidate.value();
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
                            Console.WriteLine("found = true");
                            foreach (Tuple<int, string> varGroup in myNode.variableGroups)
                            {
                                // if variables have been captured from a regex, they must match any previous matchings
                                string thisVariable = varGroup.Item2;
                                string thisVarString = variableStrings.getString(thisVariable);
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
                            Console.WriteLine("found != myNode.negDesc");
                            finished = false;
                            break;
                        }
                    }
                    //success = treeNodeMatchCandidateIterator.MoveNext();
                }
                if (!finished)
                {
                    // I successfully matched.
                    Console.WriteLine("!finished");
                    resetChild();
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
                        commitVariableGroups(m);
                    }
                    else if (value != null)
                    {
                        // commit using a set string (all groups are treated as the string)
                        commitVariableGroups(value);
                    }
                }
                // finished is false exiting this if and only if nextChild exists
                // and has a label or backreference that matches
                // (also it will just have been reset)
            }

            private void commitVariableGroups(Match m)
            {
                committedVariables = true; // commit all my variable groups.
                foreach (Tuple<int, string> varGroup in myNode.variableGroups)
                {
                    string thisVarString = m.Groups[varGroup.Item1].Value;
                    variableStrings.setVar(varGroup.Item2, thisVarString);
                }
            }

            private void commitVariableGroups(string value)
            {
                committedVariables = true;
                foreach (Tuple<int, string> varGroup in myNode.variableGroups)
                {
                    variableStrings.setVar(varGroup.Item2, value);
                }
            }

            private void decommitVariableGroups()
            {
                if (committedVariables)
                {
                    foreach (Tuple<int, string> varGroup in myNode.variableGroups)
                    {
                        variableStrings.unsetVar(varGroup.Item2);
                    }
                }
                committedVariables = false;
            }

            private void removeNamedNodes()
            {
                if ((myNode.descriptionMode != null || myNode.isLink) &&
                    myNode.name != null)
                {
                    namesToNodes.Remove(myNode.name);
                }
            }


            /* tries to match the unique child of the DescriptionPattern node to a Tree node.  Returns "true" if succeeds.*/

            private bool matchChild()
            {
                Console.WriteLine("matchChild()");
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
                    childMatcher = myNode.child.matcher(root, nextTreeNodeMatchCandidate, nodesToParents, namesToNodes,
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
                return childMatcher.matches();
            }

            // find the next local match
            //@Override
            public override bool matches()
            {
                Console.WriteLine("matches()");
                // this is necessary so that a negated/optional node matches only once
                if (finished)
                {
                    return false;
                }
                while (!finished)
                {
                    if (matchChild())
                    {
                        if (myNode.isNegated())
                        {
                            // negated node only has to fail once
                            finished = true;
                            return false; // cannot be optional and negated
                        }
                        else
                        {
                            if (myNode.isOptional())
                            {
                                finished = true;
                            }
                            return true;
                        }
                    }
                    else
                    {
                        goToNextTreeNodeMatch();
                    }
                }
                if (myNode.isNegated())
                {
                    // couldn't match my relation/pattern, so succeeded!
                    return true;
                }
                else
                {
                    // couldn't match my relation/pattern, so failed!
                    decommitVariableGroups();
                    removeNamedNodes();
                    nextTreeNodeMatchCandidate = null;
                    // didn't match, but return true anyway if optional
                    return myNode.isOptional();
                }
            }

            //@Override
            public override Tree getMatch()
            {
                return nextTreeNodeMatchCandidate;
            }

        } // end class DescriptionMatcher

        private static readonly long serialVersionUID = 1179819056757295757L;
    }
}