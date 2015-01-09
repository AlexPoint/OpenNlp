using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenNLP.Tools.Trees.TRegex;
using OpenNLP.Tools.Util;

namespace OpenNLP.Tools.Trees
{
    /// <summary>
    /// {@code GrammaticalRelation} is used to define a
    /// standardized, hierarchical set of grammatical relations,
    /// together with patterns for identifying them in
    /// parse trees.
    ///
    /// Each <code>GrammaticalRelation</code> has:
    /// <ul>
    /// <li>A <code>String</code> short name, which should be a lowercase
    /// abbreviation of some kind (in the fure mainly Universal Dependency names).</li>
    /// <li>A <code>String</code> long name, which should be descriptive.</li>
    /// <li>A parent in the <code>GrammaticalRelation</code> hierarchy.</li>
    /// <li>A {@link Pattern <code>Pattern</code>} called
    /// <code>sourcePattern</code> which matches (parent) nodes from which
    /// this <code>GrammaticalRelation</code> could hold.  (Note: this is done
    /// with the Java regex Pattern <code>matches()</code> predicate. The pattern
    /// must match the
    /// whole node name, and <code>^</code> or <code>$</code> aren't needed.
    /// Tregex constructions like __ do not work. Use ".*" to be applicable
    /// at all nodes. This prefiltering is used for efficiency.)</li>
    /// <li>A list of zero or more {@link TregexPattern
    /// <code>TregexPattern</code>s} called <code>targetPatterns</code>,
    /// which describe the local tree structure which must hold between
    /// the source node and a target node for the
    /// <code>GrammaticalRelation</code> to apply. (Note: {@code tregex}
    /// regular expressions match with the {@code find()} method, while
    /// literal string label descriptions that are not regular expressions must
    /// be {@code equals()}.)</li>
    /// </ul>
    ///
    /// The <code>targetPatterns</code> associated
    /// with a <code>GrammaticalRelation</code> are designed as follows.
    /// In order to recognize a grammatical relation X holding between
    /// nodes A and B in a parse tree, we want to associate with
    /// <code>GrammaticalRelation</code> X a {@link TregexPattern
    /// <code>TregexPattern</code>} such that:
    /// <ul>
    /// <li>the root of the pattern matches A, and</li>
    /// <li>the pattern includes a node labeled "target", which matches B.</li>
    /// </ul>
    /// For example, for the grammatical relation <code>PREDICATE</code>
    /// which holds between a clause and its primary verb phrase, we might
    /// want to use the pattern {@code "S &lt; VP=target"}, in which the
    /// root will match a clause and the node labeled <code>"target"</code>
    /// will match the verb phrase.
    ///
    /// For a given grammatical relation, the method {@link
    /// GrammaticalRelation#getRelatedNodes <code>getRelatedNodes()</code>}
    /// takes a <code>Tree</code> node as an argument and attempts to
    /// return other nodes which have this grammatical relation to the
    /// argument node.  By default, this method operates as follows: it
    /// steps through the patterns in the pattern list, trying to match
    /// each pattern against the argument node, until it finds some
    /// matches.  If a pattern matches, all matching nodes (that is, each
    /// node which corresponds to node label "target" in some match) are
    /// returned as a list; otherwise the next pattern is tried.
    ///
    /// For some grammatical relations, we need more sophisticated logic to
    /// identify related nodes.  In such cases, {@link
    /// GrammaticalRelation#getRelatedNodes <code>getRelatedNodes()</code>}
    /// can be overridden on a per-relation basis using anonymous subclassing.
    ///
    /// @see GrammaticalStructure
    /// @see EnglishGrammaticalStructure
    /// @see EnglishGrammaticalRelations
    /// @see edu.stanford.nlp.trees.international.pennchinese.ChineseGrammaticalRelations
    ///
    /// @author Bill MacCartney
    /// @author Galen Andrew (refactoring English-specific stuff)
    /// @author Ilya Sherman (refactoring annotation-relation pairing, which is now gone)
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public class GrammaticalRelation : IComparable<GrammaticalRelation>
    {
        private static readonly Dictionary<Language, Dictionary<string, GrammaticalRelation>>
            StringsToRelations = new Dictionary<Language, Dictionary<string, GrammaticalRelation>>( /*Language.class*/);

        /// <summary>
        /// The "governor" grammatical relation, which is the inverse of "dependent".
        /// Example: "the red car" -> <code>gov</code>(red, car)
        /// </summary>
        public static readonly GrammaticalRelation Governor =
            new GrammaticalRelation(Language.Any, "gov", "governor", null);

        /// <summary>
        /// The "dependent" grammatical relation, which is the inverse of "governor".
        /// Example: "the red car" -> <code>dep</code>(car, red)
        /// </summary>
        public static readonly GrammaticalRelation Dependent = new GrammaticalRelation(Language.Any, "dep", "dependent",
            null);

        /// <summary>
        /// The "root" grammatical relation between a faked "ROOT" node, and the root of the sentence.
        /// </summary>
        public static readonly GrammaticalRelation Root =
            new GrammaticalRelation(Language.Any, "root", "root", null);

        /// <summary>
        /// Dummy relation, used while collapsing relations, e.g., in English &amp; Chinese GrammaticalStructure
        /// </summary>
        public static readonly GrammaticalRelation Kill =
            new GrammaticalRelation(Language.Any, "KILL", "dummy relation kill", null);

        /// <summary>
        /// Returns the GrammaticalRelation having the given string
        /// representation (e.g. "nsubj"), or null if no such is found.
        /// </summary>
        /// <param name="s">The short name of the GrammaticalRelation</param>
        /// <param name="values">The set of GrammaticalRelations to look for it among.</param>
        /// <returns>The GrammaticalRelation with that name</returns>
        public static GrammaticalRelation ValueOf(string s, ICollection<GrammaticalRelation> values)
        {
            foreach (GrammaticalRelation reln in values)
            {
                if (reln.ToString().Equals(s)) return reln;
            }

            return null;
        }

        /// <summary>
        /// Convert from a string representation of a GrammaticalRelation to a
        /// GrammaticalRelation.  Where possible, you should avoid using this
        /// method and simply work with true GrammaticalRelations rather than
        /// string representations.  Correct behavior of this method depends
        /// on the underlying data structure resources used being kept in sync
        /// with the ToString() and Equals() methods.  However, there is really
        /// no choice but to use this method when storing GrammaticalRelations
        /// to text files and then reading them back in, so this method is not deprecated.
        /// </summary>
        /// <param name="s">The string representation of a GrammaticalRelation</param>
        /// <returns>The grammatical relation represented by this String</returns>
        public static GrammaticalRelation ValueOf(Language language, string s)
        {
            GrammaticalRelation reln = (StringsToRelations[language] != null
                ? ValueOf(s, StringsToRelations[language].Values)
                : null);
            if (reln == null)
            {
                // TODO this breaks the hierarchical structure of the classes,
                //      but it makes English relations that much likelier to work.
                reln = EnglishGrammaticalRelations.ValueOf(s);
            }
            if (reln == null)
            {
                // the block below fails when 'specific' includes underscores.
                // this is possible on weird web text, which generates relations such as prep______
                /*
      string[] names = s.split("_");
      string specific = names.length > 1? names[1] : null;
      reln = new GrammaticalRelation(language, names[0], null, null, null, specific);
      */
                string name;
                string specific;
                int underscorePosition = s.IndexOf('_');
                if (underscorePosition > 0)
                {
                    name = s.Substring(0, underscorePosition);
                    specific = s.Substring(underscorePosition + 1);
                }
                else
                {
                    name = s;
                    specific = null;
                }
                reln = new GrammaticalRelation(language, name, null, null, specific);

            }
            return reln;
        }

        private static readonly Dictionary<string, GrammaticalRelation> ValueOfCache =
            new Dictionary<string, GrammaticalRelation>();

        public static GrammaticalRelation ValueOf(string s)
        {
            if (!ValueOfCache.ContainsKey(s))
            {
                var value = ValueOf(Language.English, s);
                ValueOfCache.Add(s, value);
            }
            return ValueOfCache[s];
            /*GrammaticalRelation value = null;
            SoftReference<GrammaticalRelation> possiblyCachedValue = valueOfCache[s);
            if (possiblyCachedValue != null) { value = possiblyCachedValue[); }
            if (value == null) {
                value = valueOf(Language.English, s);
                valueOfCache.Add(s, new SoftReference<GrammaticalRelation>(value));
            }
            return value;*/
        }

        /// <summary>
        /// This function is used to determine whether the GrammaticalRelation in
        /// question is one that was created to be a thin wrapper around a String
        /// representation by valueOf(string), or whether it is a full-fledged
        /// GrammaticalRelation created by direct invocation of the constructor.
        /// </summary>
        /// <returns>Whether this relation is just a wrapper created by valueOf(string)</returns>
        public bool IsFromString()
        {
            return longName == null;
        }


        public enum Language
        {
            Any,
            English,
            Chinese
        }


        /* Non-static stuff */
        private readonly Language? language;
        private readonly string shortName;
        private readonly string longName;
        private readonly GrammaticalRelation parent;
        private readonly List<GrammaticalRelation> children = new List<GrammaticalRelation>();
        // a regexp for node values at which this relation can hold
        private readonly Regex sourcePattern;
        private readonly List<TregexPattern> targetPatterns = new List<TregexPattern>();

        private readonly string specific;
            // to hold the specific prep or conjunction associated with the grammatical relation

        // TODO document constructor
        // TODO change to put specificString after longName, and then use string... for targetPatterns
        private GrammaticalRelation(Language language,
            string shortName,
            string longName,
            GrammaticalRelation parent,
            string sourcePattern,
            TregexPatternCompiler tregexCompiler,
            string[] targetPatterns,
            string specificString)
        {
            this.language = language;
            this.shortName = shortName;
            this.longName = longName;
            this.parent = parent;
            this.specific = specificString; // this can be null!

            if (parent != null)
            {
                parent.AddChild(this);
            }

            if (sourcePattern != null)
            {
                /*try {*/
                this.sourcePattern = new Regex("^(" + sourcePattern +")$", RegexOptions.Compiled);
                /*} catch (java.util.regex.PatternSyntaxException e) {
        throw new RuntimeException("Bad pattern: " + sourcePattern);
      }*/
            }
            else
            {
                this.sourcePattern = null;
            }

            foreach (string pattern in targetPatterns)
            {
                try
                {
                    TregexPattern p = tregexCompiler.Compile(pattern);
                    this.targetPatterns.Add(p);
                }
                catch (TregexParseException pe)
                {
                    throw new SystemException("Bad pattern: " + pattern, pe);
                }
            }

            Dictionary<string, GrammaticalRelation> sToR;
            StringsToRelations.TryGetValue(language, out sToR);
            if (sToR == null)
            {
                sToR = new Dictionary<string, GrammaticalRelation>();
                StringsToRelations.Add(language, sToR);
            }
            if (sToR.ContainsKey(ToString()))
            {
                var previous = sToR[ToString()];
                if (!previous.IsFromString() && !IsFromString())
                {
                    throw new ArgumentException("There is already a relation named " + ToString() + '!');
                }
                else
                {
                    /* We get here if we previously just built a fake relation from a string
         * we previously read in from a file.
         */
                    // TODO is it worth copying all of the information from this real
                    //      relation into the old fake one?
                }
                sToR[ToString()] = this;
            }
            else
            {
                sToR.Add(ToString(), this);
            }
            /*GrammaticalRelation previous = sToR.put(ToString(), this);
    if (previous != null) {
      if (!previous.isFromString() && !isFromString()) {
        throw new ArgumentException("There is already a relation named " + ToString() + '!');
      } else {
        /* We get here if we previously just built a fake relation from a string
         * we previously read in from a file.
         #1#
        // TODO is it worth copying all of the information from this real
        //      relation into the old fake one?
      }
    }*/
        }

        /// <summary>
        /// This is the main constructor used
        /// </summary>
        public GrammaticalRelation(Language language,
            string shortName,
            string longName,
            GrammaticalRelation parent,
            string sourcePattern,
            TregexPatternCompiler tregexCompiler,
            string[] targetPatterns) :
                this(language, shortName, longName, parent, sourcePattern, tregexCompiler, targetPatterns, null)
        {
        }

        /// <summary>
        /// Used for non-leaf relations with no patterns
        /// </summary>
        public GrammaticalRelation(Language language,
            string shortName,
            string longName,
            GrammaticalRelation parent) : this(language, shortName, longName, parent, null, null, new string[0], null)
        {
        }

        /// <summary>
        /// used to create collapsed relations with specificString
        /// </summary>
        public GrammaticalRelation(Language language,
            string shortName,
            string longName,
            GrammaticalRelation parent,
            string specificString) :
                this(language, shortName, longName, parent, null, null, new string[0], specificString)
        {
        }

        private void AddChild(GrammaticalRelation child)
        {
            children.Add(child);
        }

        /// <summary>
        /// Given a {@code Tree} node {@code t}, attempts to
        /// return a list of nodes to which node {@code t} has this
        /// grammatical relation, with {@code t} as the governor.
        /// </summary>
        /// <param name="t">Target for finding dependents of t related by this GR</param>
        /// <param name="root">The root of the Tree</param>
        /// <returns>A Collection of dependent nodes to which t bears this GR</returns>
        public ICollection<TreeGraphNode> GetRelatedNodes(TreeGraphNode t, TreeGraphNode root, IHeadFinder headFinder)
        {
            Set<TreeGraphNode> nodeList = new Util.HashSet<TreeGraphNode>();
            foreach (TregexPattern p in targetPatterns)
            {
                // cdm: I deleted: && nodeList.isEmpty()
                // Initialize the TregexMatcher with the HeadFinder so that we
                // can use the same HeadFinder through the entire process of
                // building the dependencies
                TregexMatcher m = p.Matcher(root, headFinder);
                while (m.FindAt(t))
                {
                    var target = (TreeGraphNode) m.GetNode("target");
                    if (target == null)
                    {
                        throw new InvalidDataException("Expression has no target: " + p);
                    }
                    nodeList.Add(target);
                }
            }
            return nodeList;
        }

        /// <summary>
        /// Returns <code>true</code> iff the value of <code>Tree</code>
        /// node <code>t</code> matches the <code>sourcePattern</code> for
        /// this <code>GrammaticalRelation</code>, indicating that this
        /// <code>GrammaticalRelation</code> is one that could hold between
        /// <code>Tree</code> node <code>t</code> and some other node.
        /// </summary>
        public bool IsApplicable(Tree t)
        {
            return (sourcePattern != null) && (t.Value() != null) &&
                   sourcePattern.IsMatch(t.Value()) /*matcher(t.value()).matches()*/;
        }

        /// <summary>
        /// Returns whether this is equal to or an ancestor of gr in the grammatical relations hierarchy.
        /// </summary>
        public bool IsAncestor(GrammaticalRelation gr)
        {
            while (gr != null)
            {
                // Changed this test from this == gr (mrsmith)
                if (this.Equals(gr))
                {
                    return true;
                }
                gr = gr.parent;
            }
            return false;
        }

        /// <summary>
        /// Returns short name (abbreviation) for this <code>GrammaticalRelation</code>.
        /// ToString() for collapsed relations will include the word that was collapsed.
        /// <i>Implementation note:</i> Note that this method must be synced with
        /// the Equals() and valueOf(string) methods
        /// </summary>
        public override string ToString()
        {
            if (specific == null)
            {
                return shortName;
            }
            else
            {
                return shortName + '_' + specific;
            }
        }

        /// <summary>
        /// Returns a <code>string</code> representation of this
        /// <code>GrammaticalRelation</code> and the hierarchy below
        /// it, with one node per line, indented according to level.
        /// </summary>
        public string ToPrettyString()
        {
            var buf = new StringBuilder("\n");
            ToPrettyString(0, buf);
            return buf.ToString();
        }

        /// <summary>
        /// Returns a <code>string</code> representation of this
        /// <code>GrammaticalRelation</code> and the hierarchy below
        /// it, with one node per line, indented according to <code>indentLevel</code>.
        /// </summary>
        /// <param name="indentLevel">how many levels to indent (0 for root node)</param>
        private void ToPrettyString(int indentLevel, StringBuilder buf)
        {
            for (int i = 0; i < indentLevel; i++)
            {
                buf.Append("  ");
            }
            buf.Append(shortName).Append(" (").Append(longName).Append("): ").Append(targetPatterns);
            foreach (GrammaticalRelation child in children)
            {
                buf.Append('\n');
                child.ToPrettyString(indentLevel + 1, buf);
            }
        }

        /// <summary>
        /// Grammatical relations are equal with other grammatical relations if they
        /// have the same shortName and specific (if present).
        /// <i>Implementation note:</i> Note that this method must be synced with
        /// the ToString() and valueOf(string) methods
        /// </summary>
        public override bool Equals(Object o)
        {
            if (this == o) return true;
            if (o is String)
            {
                // TODO: Remove this. It's broken but was meant to cover legacy code. It would be correct to just return false.
                //new Throwable("Warning: comparing GrammaticalRelation to String").printStackTrace();
                return this.ToString().Equals(o);
            }
            if (!(o is GrammaticalRelation)) return false;

            var gr = (GrammaticalRelation) o;
            // == okay for language as enum!
            return this.language == gr.language &&
                   this.shortName.Equals(gr.shortName) &&
                   (this.specific == gr.specific ||
                    (this.specific != null && this.specific.Equals(gr.specific)));
        }

        public override int GetHashCode()
        {
            int result = 17;
            result = 29*result + (language != null ? language.ToString().GetHashCode() : 0);
            result = 29*result + (shortName != null ? shortName.GetHashCode() : 0);
            result = 29*result + (specific != null ? specific.GetHashCode() : 0);
            return result;
        }

        public int CompareTo(GrammaticalRelation o)
        {
            string thisN = this.ToString();
            string oN = o.ToString();
            return thisN.CompareTo(oN);
        }

        public string GetLongName()
        {
            return longName;
        }

        public string GetShortName()
        {
            return shortName;
        }

        public string GetSpecific()
        {
            return specific;
        }

        /**
   * When deserializing a GrammaticalRelation, it needs to be matched
   * up with the existing singleton relation of the same type.
   *
   * TODO: there are a bunch of things wrong with this.  For one
   * thing, it's crazy slow, since it goes through all the existing
   * relations in an array.  For another, it would be cleaner to have
   * subclasses for the English and Chinese relations
   */
        /*protected Object readResolve() /*throws ObjectStreamException#1# {
    switch (language) {
    case Any: {
      if (shortName.Equals(GOVERNOR.shortName)) {
        return GOVERNOR;
      } else if (shortName.Equals(DEPENDENT.shortName)) {
        return DEPENDENT;
      } else if (shortName.Equals(ROOT.shortName)) {
        return ROOT;
      } else if (shortName.Equals(KILL.shortName)) {
        return KILL;
      } else {
        throw new Exception("Unknown general relation " + shortName);
      }
    }
    case English: {
      GrammaticalRelation rel = EnglishGrammaticalRelations.valueOf(ToString());
      if (rel == null) {
        switch (shortName) {
          case "conj":
            return EnglishGrammaticalRelations.getConj(specific);
          case "prep":
            return EnglishGrammaticalRelations.getPrep(specific);
          case "prepc":
            return EnglishGrammaticalRelations.getPrepC(specific);
          default:
            // TODO: we need to figure out what to do with relations
            // which were serialized and then deprecated.  Perhaps there
            // is a good way to make them singletons
            return this;
          //throw new RuntimeException("Unknown English relation " + this);
        }
      } else {
        return rel;
      }
    }
    case Chinese: {
      GrammaticalRelation rel = ChineseGrammaticalRelations.valueOf(ToString());
      if (rel == null) {
        // TODO: we need to figure out what to do with relations
        // which were serialized and then deprecated.  Perhaps there
        // is a good way to make them singletons
        return this;
        //throw new RuntimeException("Unknown Chinese relation " + this);
      }
      return rel;
    }
    default: {
      throw new Exception("Unknown language " + language);
    }
    }
  }*/

        /// <summary>
        /// Returns the parent of this <code>GrammaticalRelation</code>.
        /// </summary>
        public GrammaticalRelation GetParent()
        {
            return parent;
        }

    }
}