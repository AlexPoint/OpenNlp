using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenNLP.Tools.Util;

namespace OpenNLP.Tools.Tokenize
{
    /// <summary>
    /// A tokenizer based on rules intended for English language only
    /// (rule-based tokenizers are language specific)
    /// </summary>
    public class EnglishRuleBasedTokenizer : AbstractTokenizer
    {
        private static readonly Regex WhitespaceRegex = new Regex("\\s+", RegexOptions.Compiled);
        private readonly Regex _tokenizationRegex;

        // Constructors --------------

        /// <summary>
        /// Base constructor for the rule based tokenizer for English language.
        /// </summary>
        /// <param name="splitOnHyphen">Wether words with hyphens should be tokenized.</param>
        public EnglishRuleBasedTokenizer(bool splitOnHyphen)
        {
            var tokenizationRules = TokenizationRules;
            if (splitOnHyphen)
            {
                tokenizationRules.AddRange(HyphenSpecificTokenizationRules);
            }
            _tokenizationRegex = new Regex(string.Format("({0})", string.Join("|", tokenizationRules)), RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        // Methods ------------------

        public override Span[] TokenizePositions(string input)
        {
            if (string.IsNullOrEmpty(input)) { return new Span[0]; }

            // split on spaces
            var parts = new List<Span>();
            var currentStartOfSpan = 0;
            var matches = WhitespaceRegex.Matches(input);
            for(var i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                parts.Add(new Span(currentStartOfSpan, match.Index));
                currentStartOfSpan = match.Index + match.Length;
            }
            parts.Add(new Span(currentStartOfSpan, input.Length));
            // tokenize each token (if necessary)
            var tokenSpans = parts
                .SelectMany(sp => SplitToken(input, sp))
                .ToList();

            // tokenize the last .
            var indexOfLastTokenWithWordChar = -1;
            for (var i = tokenSpans.Count - 1; i >= 0; i--)
            {
                var span = tokenSpans[i];
                if (input.Substring(span.Start, span.Length()).EndsWith("."))
                {
                    // We found a token ending with a '.'
                    // Split it outside the loop!
                    indexOfLastTokenWithWordChar = i;
                    break;
                }

                // If we find a token with at least one word letter, stop looking for a '.'
                if (Regex.IsMatch(input.Substring(span.Start, span.Length()), "\\w+"))
                {
                    break;
                }
            }

            if (indexOfLastTokenWithWordChar >= 0)
            {
                var lastSpanWithWordChar = tokenSpans[indexOfLastTokenWithWordChar];
                tokenSpans.RemoveAt(indexOfLastTokenWithWordChar);
                var lastSpans = new List<Span>()
                {
                    new Span(lastSpanWithWordChar.Start, lastSpanWithWordChar.End - 1),
                    new Span(lastSpanWithWordChar.End - 1, lastSpanWithWordChar.End)
                };
                tokenSpans.InsertRange(indexOfLastTokenWithWordChar, lastSpans);
            }
            
            return tokenSpans.ToArray();
        }

        private static readonly List<string> TokenizationRules = new List<string>()
        {
            // split before .{2,} if not preceded by '.'
            "(?<!\\.)(?=\\.{2,})",
            // split after .{2,} if not followed by '.'
            "(?<=\\.{2,})(?!\\.)",
            
            // split before !+ if not preceded by '!'
            "(?<!!)(?=!+)",
            // split after !+ if not followed by '!'
            "(?<=!+)(?!!)",

            // split before ?+ if not preceded by '?'
            "(?<!\\?)(?=\\?+)",
            // split after ?+ if not followed by '?'
            "(?<=\\?+)(?!\\?)",
            
            // split after ',' if not followed directly by figure
            "(?<=,)(?!\\d)",
            // split before ',' if not followed directly by figure
            "((?=,\\D)|(?=,$))",
            
            // split after ':' if not followed directly by figure
            "(?<=:)(?!\\d)",
            // split before ':' if not followed directly by figure
            "((?=:\\D)|(?=:$))",

            // split after '/' if not followed directly by figure
            "(?<=\\/)(?!\\d)",
            // split before '/' if not followed directly by figure
            "((?=\\/\\D)|(?=\\/$))",

            // split before ' at the end of a token (or before non word character)
            "(?=\\'$)",
            "(?=\\'\\W)",
            "(?=’$)",
            "(?=’\\W)",

            // split before - when at the end of a token and not preceded by -
            "(?<!\\-)(?=\\-$)",
            // split after - when at the beginning of a token and not followed by -
            "(?<=^\\-)(?!\\-)",

            // split before ;, (, ), [, ], {, }, " in all cases
            "(?=;|\\(|\\)|\\{|\\}|\\[|\\]|\"|…)",
            // split after ;, (, ), [, ], {, }, " in all cases
            "(?<=;|\\(|\\)|\\{|\\}|\\[|\\]|\"|…)",
            
            // split after ' at the beginning of a token (and not 's, 'm, 'll, 've, 're or 'd)
            "(?<=^\\')(?!s$|m$|ll$|ve$|re$|d$)",
            "(?<=^’)(?!s$|m$|ll$|ve$|re$|d$)",

            // split before 's, 'm, 've, 'll, 're, 'd when at the end of a token (’ == ')
            "(?=\\'s$|\\'m$|\\'ve$|\\'ll$|\\'re$|\\'d$|’s$|’m$|’ve$|’ll$|’re$|’d$)"
        };

        private static readonly List<string> HyphenSpecificTokenizationRules = new List<string>()
        {
            // The two following rules are used to split compound words. Ex: x-ray --> x|-|ray
            // Not sure this is the right way to go ultimately so remove them if necessary
            // split after - when followed by a letter
            "(?<=\\-)(?=\\w)",
            // split before - when preceded by a letter
            "(?<=\\w)(?=\\-)",
        };

        private static readonly Regex LettersOnlyRegex = new Regex("^[a-zA-Z]+$", RegexOptions.Compiled);
        private IEnumerable<Span> SplitToken(string input, Span span)
        {
            var token = input.Substring(span.Start, span.Length());
            if (string.IsNullOrEmpty(token))
            {
                return new List<Span>();
            }

            // optimization - don't tokenize token of 1 character or token with letters only
            if (span.Length() <= 1 || LettersOnlyRegex.IsMatch(token))
            {
                return new List<Span>(){ span };
            }

            var splitTokens = _tokenizationRegex.Split(token);
            
            var spans = new List<Span>();
            var currentStart = span.Start;
            foreach (var splitToken in splitTokens)
            {
                if (splitToken.Length > 0)
                {
                    spans.Add(new Span(currentStart, currentStart + splitToken.Length)); 
                }
                currentStart += splitToken.Length;
            }
            return spans;
        }
    }
}
