using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees.TRegex.Tsurgeon
{
    public class RelabelNode: TsurgeonPattern
    {
        // Overly complicated pattern to identify regexes surrounded by /,
  // possibly with / escaped inside the regex.  
  // The purpose of the [^/]*[^/\\\\] is to match characters that
  // aren't / and to allow escaping of other characters.
  // The purpose of the \\\\/ is to allow escaped / inside the pattern.
  // The purpose of the \\\\\\\\ is to allow escaped \ at the end of
  // the pattern, so you can match, for example, /\\/.  There need to
  // be 8x\ because both java and regexes need escaping, resulting in 4x.
  static readonly String regexPatternString = 
    "((?:(?:[^/]*[^/\\\\])|\\\\/)*(?:\\\\\\\\)*)";

  static readonly Regex regexPattern = new Regex("/" + regexPatternString + "/");

  /**
   * This pattern finds relabel snippets that use a named node.
   */
  static readonly String nodePatternString = "(=\\{[a-zA-Z0-9_]+\\})";
  static readonly Regex nodePattern = new Regex(nodePatternString);
  /**
   * This pattern finds relabel snippets that use a captured variable.
   */
  static readonly String variablePatternString = "(%\\{[a-zA-Z0-9_]+\\})";
  static readonly Regex variablePattern = new Regex(variablePatternString);
  /**
   * Finds one chunk of a general relabel operation, either named node
   * or captured variable
   */
  static readonly String oneGeneralReplacement = 
    ("(" + nodePatternString + "|" + variablePatternString + ")");
  static readonly Regex oneGeneralReplacementPattern = new Regex(oneGeneralReplacement);

  /**
   * Identifies a node using the regex replacement strategy.
   */
  static readonly Regex substPattern = new Regex("/" + regexPatternString + "/(.*)/");

  enum RelabelMode { FIXED, REGEX };
  private readonly RelabelMode mode;

  private readonly String newLabel;

  private readonly Regex labelRegex;
  private readonly String replacementString;
  private readonly List<String> replacementPieces;

  public RelabelNode(TsurgeonPattern child, String newLabel):
    base("relabel", new TsurgeonPattern[] { child }){
    var m1 = substPattern.Match(newLabel);
    if (m1.Success) {
      mode = RelabelMode.REGEX;
      this.labelRegex = new Regex(m1.Groups[1].Value);
      this.replacementString = m1.Groups[2].Value;
      replacementPieces = new List<String>();
      var generalMatcher = 
        oneGeneralReplacementPattern.Match(m1.Groups[2].Value);
      int lastPosition = 0;
        var nextMatch = generalMatcher.NextMatch();
      while (nextMatch != null) {
        if (nextMatch.Index > lastPosition) {
          replacementPieces.Add(replacementString.Substring(lastPosition, nextMatch.Index));
        }
        lastPosition = nextMatch.Index + nextMatch.Length;
        String piece = nextMatch.Value;
          if (piece.Equals(""))
          {
              nextMatch = generalMatcher.NextMatch();
              continue;
          }
        replacementPieces.Add(nextMatch.Value);
          nextMatch = generalMatcher.NextMatch();
      }
      if (lastPosition < replacementString.Length) {
        replacementPieces.Add(replacementString.Substring(lastPosition));
      }
      this.newLabel = null;
    } else {
      mode = RelabelMode.FIXED;
      var m2 = regexPattern.Match(newLabel);
      if (m2.Success) {
        // fixed relabel but surrounded by regex slashes
        String unescapedLabel = m2.Groups[1].Value;
        this.newLabel = removeEscapeSlashes(unescapedLabel);
      } else {
        // just a node name to relabel to
        this.newLabel = newLabel;
      }
      this.replacementString = null;
      this.replacementPieces = null;
      this.labelRegex = null;

    }
  }

  private static String removeEscapeSlashes(String input) {
    StringBuilder output = new StringBuilder();
    int len = input.Length;
    bool lastIsBackslash = false;
    for (int i = 0; i < len; i++) {
      char ch = input[i];
      if (ch == '\\') {
        if (lastIsBackslash || i == len - 1 ) {
          output.Append(ch);
          lastIsBackslash = false;
        } else {
          lastIsBackslash = true;
        }
      } else {
        output.Append(ch);
        lastIsBackslash = false;
      }
    }
    return output.ToString();
  }


  //@Override
  public override TsurgeonMatcher matcher(Dictionary<String,Tree> newNodeNames, CoindexationGenerator coindexer) {
    return new RelabelMatcher(newNodeNames, coindexer, this);
  }

  private class RelabelMatcher : TsurgeonMatcher
  {
      private RelabelNode node;
      public RelabelMatcher(Dictionary<String, Tree> newNodeNames, CoindexationGenerator coindexer, RelabelNode node) :
          base(node, newNodeNames, coindexer)
      {
          this.node = node;
      }

    //@Override
    public override Tree evaluate(Tree tree, TregexMatcher tregex) {
      Tree nodeToRelabel = childMatcher[0].evaluate(tree, tregex);
      switch (node.mode) {
          case RelabelMode.FIXED:
              {
        nodeToRelabel.label().setValue(node.newLabel);
        break;
      }
          case RelabelMode.REGEX:
              {
        
        StringBuilder label = new StringBuilder();
        foreach (String chunk in node.replacementPieces) {
          if (variablePattern.IsMatch(chunk)) {
            String name = chunk.Substring(2, chunk.Length - 1);
            //label.Append(Matcher.quoteReplacement(tregex.getVariableString(name)));
            label.Append(tregex.getVariableString(name).Replace("'", "").Replace("\"",""));
          } else if (nodePattern.IsMatch(chunk)) {
            String name = chunk.Substring(2, chunk.Length - 1);
            //label.Append(Matcher.quoteReplacement(tregex.getNode(name).value()));
            label.Append(tregex.getNode(name).value().Replace("'", "").Replace("\"",""));
          } else {
            label.Append(chunk);
          }
        }
        //var m = node.labelRegex.Match(nodeToRelabel.label().value());
        //nodeToRelabel.label().setValue(m.replaceAll(label.toString()));
                  var newS = node.labelRegex.Replace(nodeToRelabel.label().value(), label.ToString());
        nodeToRelabel.label().setValue(/*m.replaceAll(label.toString())*/newS);
        break;
      }
      default:
        throw new ArgumentException("Unsupported relabel mode " + node.mode);
      }
      return tree;
    }
  }

  //@Override
  public override String ToString() {
    String result;
    switch(mode) {
        case RelabelMode.FIXED:
      return label + '(' + children[0].ToString() + ',' + newLabel + ')';
        case RelabelMode.REGEX:
      return label + '(' + children[0].ToString() + ',' + labelRegex.ToString() + ',' + replacementString + ')';
    default:
      throw new InvalidEnumArgumentException("Unsupported relabel mode " + mode);
    }
  }
    }
}
