using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees.TRegex.Tsurgeon
{
    public class RelabelNode : TsurgeonPattern
    {
        /// <summary>
        /// Overly complicated pattern to identify regexes surrounded by /,
        /// possibly with / escaped inside the regex.  
        /// The purpose of the [^/]*[^/\\\\] is to match characters that
        /// aren't / and to allow escaping of other characters.
        /// The purpose of the \\\\/ is to allow escaped / inside the pattern.
        /// The purpose of the \\\\\\\\ is to allow escaped \ at the end of
        /// the pattern, so you can match, for example, /\\/.  There need to
        /// be 8x\ because both java and regexes need escaping, resulting in 4x.
        /// </summary>
        private const string RegexPatternString = "((?:(?:[^/]*[^/\\\\])|\\\\/)*(?:\\\\\\\\)*)";
        private static readonly Regex RegexPattern = new Regex("/" + RegexPatternString + "/");

        /// <summary>
        /// This pattern finds relabel snippets that use a named node.
        /// </summary>
        private const string NodePatternString = "(=\\{[a-zA-Z0-9_]+\\})";
        private static readonly Regex NodePattern = new Regex(NodePatternString);
        
        /// <summary>
        /// This pattern finds relabel snippets that use a captured variable
        /// </summary>
        private const string VariablePatternString = "(%\\{[a-zA-Z0-9_]+\\})";
        private static readonly Regex VariablePattern = new Regex(VariablePatternString);
        
        /// <summary>
        /// Finds one chunk of a general relabel operation, either named node or captured variable
        /// </summary>
        private const string OneGeneralReplacement = ("(" + NodePatternString + "|" + VariablePatternString + ")");
        private static readonly Regex OneGeneralReplacementPattern = new Regex(OneGeneralReplacement);

        /// <summary>
        /// Identifies a node using the regex replacement strategy
        /// </summary>
        private static readonly Regex SubstPattern = new Regex("/" + RegexPatternString + "/(.*)/");

        private enum RelabelMode
        {
            Fixed,
            Regex
        };

        private readonly RelabelMode mode;
        private readonly string newLabel;
        private readonly Regex labelRegex;
        private readonly string replacementString;
        private readonly List<string> replacementPieces;

        public RelabelNode(TsurgeonPattern child, string newLabel) :
            base("relabel", new TsurgeonPattern[] {child})
        {
            var m1 = SubstPattern.Match(newLabel);
            if (m1.Success)
            {
                mode = RelabelMode.Regex;
                this.labelRegex = new Regex(m1.Groups[1].Value);
                this.replacementString = m1.Groups[2].Value;
                replacementPieces = new List<string>();
                var generalMatcher =
                    OneGeneralReplacementPattern.Match(m1.Groups[2].Value);
                int lastPosition = 0;
                var nextMatch = generalMatcher.NextMatch();
                while (nextMatch.Success)
                {
                    if (nextMatch.Index > lastPosition)
                    {
                        replacementPieces.Add(replacementString.Substring(lastPosition, nextMatch.Index));
                    }
                    lastPosition = nextMatch.Index + nextMatch.Length;
                    string piece = nextMatch.Value;
                    if (piece.Equals(""))
                    {
                        nextMatch = generalMatcher.NextMatch();
                        continue;
                    }
                    replacementPieces.Add(nextMatch.Value);
                    nextMatch = generalMatcher.NextMatch();
                }
                if (lastPosition < replacementString.Length)
                {
                    replacementPieces.Add(replacementString.Substring(lastPosition));
                }
                this.newLabel = null;
            }
            else
            {
                mode = RelabelMode.Fixed;
                var m2 = RegexPattern.Match(newLabel);
                if (m2.Success)
                {
                    // fixed relabel but surrounded by regex slashes
                    string unescapedLabel = m2.Groups[1].Value;
                    this.newLabel = RemoveEscapeSlashes(unescapedLabel);
                }
                else
                {
                    // just a node name to relabel to
                    this.newLabel = newLabel;
                }
                this.replacementString = null;
                this.replacementPieces = null;
                this.labelRegex = null;

            }
        }

        private static string RemoveEscapeSlashes(string input)
        {
            var output = new StringBuilder();
            int len = input.Length;
            bool lastIsBackslash = false;
            for (int i = 0; i < len; i++)
            {
                char ch = input[i];
                if (ch == '\\')
                {
                    if (lastIsBackslash || i == len - 1)
                    {
                        output.Append(ch);
                        lastIsBackslash = false;
                    }
                    else
                    {
                        lastIsBackslash = true;
                    }
                }
                else
                {
                    output.Append(ch);
                    lastIsBackslash = false;
                }
            }
            return output.ToString();
        }

        public override TsurgeonMatcher GetMatcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer)
        {
            return new RelabelMatcher(newNodeNames, coindexer, this);
        }

        private class RelabelMatcher : TsurgeonMatcher
        {
            private readonly RelabelNode node;

            public RelabelMatcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer,
                RelabelNode node) :
                    base(node, newNodeNames, coindexer)
            {
                this.node = node;
            }

            public override Tree Evaluate(Tree tree, TregexMatcher tregex)
            {
                Tree nodeToRelabel = ChildMatcher[0].Evaluate(tree, tregex);
                switch (node.mode)
                {
                    case RelabelMode.Fixed:
                    {
                        nodeToRelabel.Label().SetValue(node.newLabel);
                        break;
                    }
                    case RelabelMode.Regex:
                    {

                        var label = new StringBuilder();
                        foreach (string chunk in node.replacementPieces)
                        {
                            if (VariablePattern.IsMatch(chunk))
                            {
                                //String name = chunk.Substring(2, chunk.Length - 1);
                                string name = chunk.Substring(2, chunk.Length - 3);
                                //label.Append(Matcher.quoteReplacement(tregex.getVariableString(name)));
                                label.Append(tregex.GetVariableString(name).Replace("'", "").Replace("\"", ""));
                            }
                            else if (NodePattern.IsMatch(chunk))
                            {
                                //String name = chunk.Substring(2, chunk.Length - 1);
                                string name = chunk.Substring(2, chunk.Length - 3);
                                //label.Append(Matcher.quoteReplacement(tregex.getNode(name).value()));
                                label.Append(tregex.GetNode(name).Value().Replace("'", "").Replace("\"", ""));
                            }
                            else
                            {
                                label.Append(chunk);
                            }
                        }
                        //var m = node.labelRegex.Match(nodeToRelabel.label().value());
                        //nodeToRelabel.label().setValue(m.replaceAll(label.ToString()));
                        var newS = node.labelRegex.Replace(nodeToRelabel.Label().Value(), label.ToString());
                        nodeToRelabel.Label().SetValue( /*m.replaceAll(label.ToString())*/newS);
                        break;
                    }
                    default:
                        throw new ArgumentException("Unsupported relabel mode " + node.mode);
                }
                return tree;
            }
        }

        public override string ToString()
        {
            string result;
            switch (mode)
            {
                case RelabelMode.Fixed:
                    return label + '(' + children[0].ToString() + ',' + newLabel + ')';
                case RelabelMode.Regex:
                    return label + '(' + children[0].ToString() + ',' + labelRegex.ToString() + ',' + replacementString +
                           ')';
                default:
                    throw new InvalidEnumArgumentException("Unsupported relabel mode " + mode);
            }
        }
    }
}