using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Trees.TRegex;

namespace OpenNLP.Tools.Util.Trees
{
    public class GrammaticalRelation:IComparable<GrammaticalRelation>
    {
        private static readonly long serialVersionUID = 892618003417550128L;

  //private static readonly bool DEBUG = System.getProperty("GrammaticalRelation", null) != null;

  private static readonly Dictionary<Language, Dictionary<String, GrammaticalRelation>>
    stringsToRelations = new Dictionary<Language, Dictionary<String, GrammaticalRelation>>(/*Language.class*/);

  /**
   * The "governor" grammatical relation, which is the inverse of "dependent".<p>
   * <p/>
   * Example: "the red car" &rarr; <code>gov</code>(red, car)
   */
  public static readonly GrammaticalRelation GOVERNOR =
    new GrammaticalRelation(Language.Any, "gov", "governor", null);


  /**
   * The "dependent" grammatical relation, which is the inverse of "governor".<p>
   * <p/>
   * Example: "the red car" &rarr; <code>dep</code>(car, red)
   */
  public static readonly GrammaticalRelation DEPENDENT =
    new GrammaticalRelation(Language.Any, "dep", "dependent", null);


  /**
   *  The "root" grammatical relation between a faked "ROOT" node, and the root of the sentence.
   */
  public static readonly GrammaticalRelation ROOT =
    new GrammaticalRelation(Language.Any, "root", "root", null);


  /**
   * Dummy relation, used while collapsing relations, e.g., in English &amp; Chinese GrammaticalStructure
   */
  public static readonly GrammaticalRelation KILL =
    new GrammaticalRelation(Language.Any, "KILL", "dummy relation kill", null);


  /**
   * Returns the GrammaticalRelation having the given string
   * representation (e.g. "nsubj"), or null if no such is found.
   *
   * @param s The short name of the GrammaticalRelation
   * @param values The set of GrammaticalRelations to look for it among.
   * @return The GrammaticalRelation with that name
   */
  public static GrammaticalRelation valueOf(String s, ICollection<GrammaticalRelation> values) {
    foreach(GrammaticalRelation reln in values) {
      if (reln.ToString().Equals(s)) return reln;
    }

    return null;
  }

  /** Convert from a String representation of a GrammaticalRelation to a
   *  GrammaticalRelation.  Where possible, you should avoid using this
   *  method and simply work with true GrammaticalRelations rather than
   *  String representations.  Correct behavior of this method depends
   *  on the underlying data structure resources used being kept in sync
   *  with the ToString() and Equals() methods.  However, there is really
   *  no choice but to use this method when storing GrammaticalRelations
   *  to text files and then reading them back in, so this method is not
   *  deprecated.
   *
   *  @param s The String representation of a GrammaticalRelation
   *  @return The grammatical relation represented by this String
   */
  public static GrammaticalRelation valueOf(Language language, String s) {
    GrammaticalRelation reln = (stringsToRelations[language] != null ? valueOf(s, stringsToRelations[language].Values) : null);
    if (reln == null) {
      // TODO this breaks the hierarchical structure of the classes,
      //      but it makes English relations that much likelier to work.
      reln = EnglishGrammaticalRelations.valueOf(s);
    }
    if (reln == null) {
      // the block below fails when 'specific' includes underscores.
      // this is possible on weird web text, which generates relations such as prep______
      /*
      String[] names = s.split("_");
      String specific = names.length > 1? names[1] : null;
      reln = new GrammaticalRelation(language, names[0], null, null, null, specific);
      */
      String name;
      String specific;
      int underscorePosition = s.IndexOf('_');
      if (underscorePosition > 0) {
        name = s.Substring(0, underscorePosition);
        specific = s.Substring(underscorePosition + 1);
      } else {
        name = s;
        specific = null;
      }
      reln = new GrammaticalRelation(language, name, null, null, specific);

    }
    return reln;
  }

  private static Dictionary<String, GrammaticalRelation> valueOfCache = new Dictionary<string, GrammaticalRelation>();
  public static GrammaticalRelation valueOf(String s) {
      if (!valueOfCache.ContainsKey(s))
      {
          var value = valueOf(Language.English, s);
          valueOfCache.Add(s, value);
      }
      return valueOfCache[s];
    /*GrammaticalRelation value = null;
    SoftReference<GrammaticalRelation> possiblyCachedValue = valueOfCache[s);
    if (possiblyCachedValue != null) { value = possiblyCachedValue[); }
    if (value == null) {
      value = valueOf(Language.English, s);
      valueOfCache.Add(s, new SoftReference<GrammaticalRelation>(value));
    }
    return value;*/
  }

  /**
   * This function is used to determine whether the GrammaticalRelation in
   * question is one that was created to be a thin wrapper around a String
   * representation by valueOf(String), or whether it is a full-fledged
   * GrammaticalRelation created by direct invocation of the constructor.
   *
   * @return Whether this relation is just a wrapper created by valueOf(String)
   */
  public bool isFromString() {
    return longName == null;
  }


  public enum Language { Any, English, Chinese }


  /* Non-static stuff */
  private readonly Language language;
  private readonly String shortName;
  private readonly String longName;
  private readonly GrammaticalRelation parent;
  private readonly List<GrammaticalRelation> children = new List<GrammaticalRelation>();
  // a regexp for node values at which this relation can hold
  private readonly Regex sourcePattern;
  private readonly List<TregexPattern> targetPatterns = new List<TregexPattern>();
  private readonly String specific; // to hold the specific prep or conjunction associated with the grammatical relation

  // TODO document constructor
  // TODO change to put specificString after longName, and then use String... for targetPatterns
  private GrammaticalRelation(Language language,
                             String shortName,
                             String longName,
                             GrammaticalRelation parent,
                             String sourcePattern,
                             TregexPatternCompiler tregexCompiler,
                             String[] targetPatterns,
                             String specificString) {
    this.language = language;
    this.shortName = shortName;
    this.longName = longName;
    this.parent = parent;
    this.specific = specificString; // this can be null!

    if (parent != null) {
      parent.addChild(this);
    }

    if (sourcePattern != null) {
      /*try {*/
        this.sourcePattern = new Regex(sourcePattern, RegexOptions.Compiled);
      /*} catch (java.util.regex.PatternSyntaxException e) {
        throw new RuntimeException("Bad pattern: " + sourcePattern);
      }*/
    } else {
      this.sourcePattern = null;
    }

    foreach(String pattern in targetPatterns) {
      try {
        TregexPattern p = tregexCompiler.compile(pattern);
        this.targetPatterns.Add(p);
      } catch (TregexParseException pe) {
        throw new SystemException("Bad pattern: " + pattern, pe);
      }
    }

    Dictionary<String, GrammaticalRelation> sToR = stringsToRelations[language];
    if (sToR == null) {
      sToR = new Dictionary<string, GrammaticalRelation>();
      stringsToRelations.Add(language, sToR);
    }
      if (sToR.ContainsKey(ToString()))
      {
          var previous = sToR[ToString()];
          if (!previous.isFromString() && !isFromString())
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

  // This is the main constructor used
  public GrammaticalRelation(Language language,
                             String shortName,
                             String longName,
                             GrammaticalRelation parent,
                             String sourcePattern,
                             TregexPatternCompiler tregexCompiler,
                             String[] targetPatterns):
    this(language, shortName, longName, parent, sourcePattern, tregexCompiler, targetPatterns, null){
  }

  // Used for non-leaf relations with no patterns
  public GrammaticalRelation(Language language,
                             String shortName,
                             String longName,
                             GrammaticalRelation parent):this(language, shortName, longName, parent, null, null, new string[0], null){
  }

  // used to create collapsed relations with specificString
  public GrammaticalRelation(Language language,
                             String shortName,
                             String longName,
                             GrammaticalRelation parent,
                             String specificString):
    this(language, shortName, longName, parent, null, null, new string[0], specificString){
  }

  private void addChild(GrammaticalRelation child) {
    children.Add(child);
  }

  /** Given a {@code Tree} node {@code t}, attempts to
   *  return a list of nodes to which node {@code t} has this
   *  grammatical relation, with {@code t} as the governor.
   *
   *  @param t Target for finding dependents of t related by this GR
   *  @param root The root of the Tree
   *  @return A Collection of dependent nodes to which t bears this GR
   */
  public ICollection<TreeGraphNode> getRelatedNodes(TreeGraphNode t, TreeGraphNode root, HeadFinder headFinder) {
    Set<TreeGraphNode> nodeList = new HashSet<TreeGraphNode>();
    foreach(TregexPattern p in targetPatterns) {    // cdm: I deleted: && nodeList.isEmpty()
      // Initialize the TregexMatcher with the HeadFinder so that we
      // can use the same HeadFinder through the entire process of
      // building the dependencies
      TregexMatcher m = p.matcher(root, headFinder);
      while (m.findAt(t)) {
        TreeGraphNode target = (TreeGraphNode) m.getNode("target");
        if (target == null) {
          throw new InvalidDataException("Expression has no target: " + p);
        }
        nodeList.Add(target);
        /*if (DEBUG) {
          System.err.println("found " + this + "(" + t + "-" + t.headWordNode() + ", " + m.getNode("target") + "-" + ((TreeGraphNode) m.getNode("target")).headWordNode() + ") using pattern " + p);
          foreach(String nodeName : m.getNodeNames()) {
            if (nodeName.Equals("target"))
              continue;
            System.err.println("  node " + nodeName + ": " + m.getNode(nodeName));
          }
        }*/
      }
    }
    return nodeList;
  }

  /** Returns <code>true</code> iff the value of <code>Tree</code>
   *  node <code>t</code> matches the <code>sourcePattern</code> for
   *  this <code>GrammaticalRelation</code>, indicating that this
   *  <code>GrammaticalRelation</code> is one that could hold between
   *  <code>Tree</code> node <code>t</code> and some other node.
   */
  public bool isApplicable(Tree t) {
    // System.err.println("Testing whether " + sourcePattern + " matches " + ((TreeGraphNode) t).toOneLineString());
    return (sourcePattern != null) && (t.value() != null) &&
             sourcePattern.IsMatch(t.value())/*matcher(t.value()).matches()*/;
  }

  /** Returns whether this is equal to or an ancestor of gr in the grammatical relations hierarchy. */
  public bool isAncestor(GrammaticalRelation gr) {
    while (gr != null) {
      // Changed this test from this == gr (mrsmith)
      if (this.Equals(gr)) { return true; }
      gr = gr.parent;
    }
    return false;
  }

  /**
   * Returns short name (abbreviation) for this
   * <code>GrammaticalRelation</code>.  ToString() for collapsed
   * relations will include the word that was collapsed.
   * <br/>
   * <i>Implementation note:</i> Note that this method must be synced with
   * the Equals() and valueOf(String) methods
   */
  //@Override
  public override String ToString() {
    if (specific == null) {
      return shortName;
    } else {
      return shortName + '_' + specific;
    }
  }

  /**
   * Returns a <code>String</code> representation of this
   * <code>GrammaticalRelation</code> and the hierarchy below
   * it, with one node per line, indented according to level.
   *
   * @return <code>String</code> representation of this
   *         <code>GrammaticalRelation</code>
   */
  public String toPrettyString() {
    StringBuilder buf = new StringBuilder("\n");
    toPrettyString(0, buf);
    return buf.ToString();
  }

  /**
   * Returns a <code>String</code> representation of this
   * <code>GrammaticalRelation</code> and the hierarchy below
   * it, with one node per line, indented according to
   * <code>indentLevel</code>.
   *
   * @param indentLevel how many levels to indent (0 for root node)
   */
  private void toPrettyString(int indentLevel, StringBuilder buf) {
    for(int i = 0; i < indentLevel; i++) {
      buf.Append("  ");
    }
    buf.Append(shortName).Append(" (").Append(longName).Append("): ").Append(targetPatterns);
    foreach(GrammaticalRelation child in children) {
      buf.Append('\n');
      child.toPrettyString(indentLevel + 1, buf);
    }
  }

  /** Grammatical relations are equal with other grammatical relations if they
   *  have the same shortName and specific (if present).
   *  <i>Implementation note:</i> Note that this method must be synced with
   *  the ToString() and valueOf(String) methods
   *
   *  @param o Object to be compared
   *  @return Whether equal
   */
  //@SuppressWarnings({"StringEquality", "ThrowableInstanceNeverThrown"})
  //@Override
  public override bool Equals(Object o) {
    if (this == o) return true;
    if (o is String) {
      // TODO: Remove this. It's broken but was meant to cover legacy code. It would be correct to just return false.
      //new Throwable("Warning: comparing GrammaticalRelation to String").printStackTrace();
      return this.ToString().Equals(o);
    }
    if (!(o is GrammaticalRelation)) return false;

    /*final*/ GrammaticalRelation gr = (GrammaticalRelation) o;
    // == okay for language as enum!
    return this.language == gr.language &&
             this.shortName.Equals(gr.shortName) &&
             (this.specific == gr.specific ||
              (this.specific != null && this.specific.Equals(gr.specific)));
  }

  //@Override
        public override int GetHashCode()
        {
    int result = 17;
    result = 29 * result + (language != null ? language.ToString().GetHashCode() : 0);
    result = 29 * result + (shortName != null ? shortName.GetHashCode() : 0);
    result = 29 * result + (specific != null ? specific.GetHashCode() : 0);
    return result;
  }

  //@Override
  public int CompareTo(GrammaticalRelation o) {
    String thisN = this.ToString();
    String oN = o.ToString();
    return thisN.CompareTo(oN);
  }

  public String getLongName() {
    return longName;
  }

  public String getShortName() {
    return shortName;
  }

  public String getSpecific() {
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

  /**
   * Returns the parent of this <code>GrammaticalRelation</code>.
   */
  public GrammaticalRelation getParent() {
    return parent;
  }

  /*public static void main(String[] args) {
    final String[] names = {"dep", "pred", "prep_to","rcmod"};
    foreach(String name : names) {
      GrammaticalRelation reln = valueOf(Language.English, name);
      System.out.println("Data for GrammaticalRelation loaded as valueOf(\"" + name + "\"):");
      System.out.println("\tShort name:    " + reln.getShortName());
      System.out.println("\tLong name:     " + reln.getLongName());
      System.out.println("\tSpecific name: " + reln.getSpecific());
    }
  }*/
    }
}
