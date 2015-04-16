using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenNLP.Tools.Util;

namespace OpenNLP.Tools.Tokenize
{
    public class EnglishRuleBasedTokenizer : AbstractTokenizer
    {

        public override Span[] TokenizePositions(string input)
        {
            if (string.IsNullOrEmpty(input)) { return new Span[0]; }

            // split on spaces
            var parts = SplitOnWhitespaces(input);
            // tokenize each token (if necessary)
            var tokenSpans = parts
                .SelectMany(sp => SplitToken(input, sp))
                .ToList();

            // tokenize the last .
            var lastSpanWithWordChar = tokenSpans.LastOrDefault(sp => Regex.IsMatch(input.Substring(sp.Start, sp.Length()), "\\w+"));
            if (lastSpanWithWordChar != null)
            {
                var indexOfLastTokenWithWordChar = tokenSpans.LastIndexOf(lastSpanWithWordChar);
                if (input.Substring(lastSpanWithWordChar.Start, lastSpanWithWordChar.Length()).EndsWith("."))
                {
                    tokenSpans.RemoveAt(indexOfLastTokenWithWordChar);
                    //var lastTokenParts = Regex.Split(lastSpanWithWordChar, "(?=\\.$)");
                    var lastSpans = new List<Span>()
                    {
                        new Span(lastSpanWithWordChar.Start, lastSpanWithWordChar.End - 1),
                        new Span(lastSpanWithWordChar.End - 1, lastSpanWithWordChar.End)
                    };
                    tokenSpans.InsertRange(indexOfLastTokenWithWordChar, lastSpans);
                }
            }

            return tokenSpans.ToArray();
        }


        // Tokenize rules 
        private static readonly List<Regex> TokenizationRegexes = new List<Regex>()
        {
            // split before .{2,} if not preceded by '.'
            new Regex("(?<!\\.)(?=\\.{2,})", RegexOptions.Compiled),
            // split after .{2,} if not followed by '.'
            new Regex("(?<=\\.{2,})(?!\\.)", RegexOptions.Compiled),
            
            // split before !+ if not preceded by '!'
            new Regex("(?<!!)(?=!+)", RegexOptions.Compiled),
            // split after !+ if not followed by '!'
            new Regex("(?<=!+)(?!!)", RegexOptions.Compiled),

            // split before ?+ if not preceded by '?'
            new Regex("(?<!\\?)(?=\\?+)", RegexOptions.Compiled),
            // split after ?+ if not followed by '?'
            new Regex("(?<=\\?+)(?!\\?)", RegexOptions.Compiled),
            
            // split after ',' if not followed directly by figure
            new Regex("(?<=,)(?!\\d)", RegexOptions.Compiled), 
            // split before ',' if not followed directly by figure
            new Regex("((?=,\\D)|(?=,$))", RegexOptions.Compiled),
            
            // split after ':' if not followed directly by figure
            new Regex("(?<=:)(?!\\d)", RegexOptions.Compiled), 
            // split before ':' if not followed directly by figure
            new Regex("((?=:\\D)|(?=:$))", RegexOptions.Compiled),

            // split before 's, 'm, 've, 'll, 're, 'd when at the end of a token (’ == ')
            new Regex("(?=\\'s$|\\'m$|\\'ve$|\\'ll$|\\'re$|\\'d$|’s$|’m$|’ve$|’ll$|’re$|’d$)", RegexOptions.IgnoreCase | RegexOptions.Compiled),

            // split after ' at the beginning of a token (and not 's, 'm, 'll, 've, 're or 'd)
            new Regex("(?<=^\\')(?!s$|m$|ll$|ve$|re$|d$)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex("(?<=^’)(?!s$|m$|ll$|ve$|re$|d$)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            // split before ' at the end of a token
            new Regex("(?=\\'$)", RegexOptions.Compiled),
            new Regex("(?=’$)", RegexOptions.Compiled),

            // split before - when at the end of a token and not preceded by -
            new Regex("(?<!\\-)(?=\\-$)", RegexOptions.Compiled),
            // split after - when at the beginning of a token and not followed by -
            new Regex("(?<=^\\-)(?!\\-)", RegexOptions.Compiled),
            
            // split before ;, (, ), [, ], {, }, " in all cases
            new Regex("(?=;|\\(|\\)|\\{|\\}|\\[|\\]|\"|…)", RegexOptions.Compiled),
            // split after ;, (, ), [, ], {, }, " in all cases
            new Regex("(?<=;|\\(|\\)|\\{|\\}|\\[|\\]|\"|…)", RegexOptions.Compiled)
        };

        private static readonly Regex LettersOnlyRegex = new Regex("^[a-zA-Z]+$", RegexOptions.Compiled);
        private List<Span> SplitToken(string input, Span span)
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

            var splitTokens = new List<string>() { token };
            foreach (var tokenizationRegex in TokenizationRegexes)
            {
                /*var tempSpans = new List<Span>();
                foreach (var tempSpan in spans)
                {
                    var tempToken = input.Substring(tempSpan.Start, tempSpan.Length());
                    var matches = tokenizationRegex.Matches(tempToken);
                    
                    var matchIndices = new List<int>();
                    for (int i = 0; i < matches.Count; i++)
                    {
                        var index = matches[i].Index;
                        if (0 < index && index < tempToken.Length)
                        {
                            matchIndices.Add(index);
                        }
                    }

                    if (matchIndices.Any())
                    {
                        for (var i = 0; i < matchIndices.Count; i++)
                        {
                            var start = i == 0 ? 0 : matchIndices[i - 1] - 1;
                            tempSpans.Add(new Span(tempSpan.Start + start, tempSpan.Start + matchIndices[i]));
                        }
                        // add last one
                        tempSpans.Add(new Span(matchIndices.Last(), tempSpan.End));
                    }
                    else
                    {
                        tempSpans.Add(tempSpan);
                    }
                }
                spans = tempSpans;*/

                var tempTokens = splitTokens
                    .SelectMany(tok => tokenizationRegex.Split(tok))
                    .Where(p => !string.IsNullOrEmpty(p))
                    .ToList();
                splitTokens = tempTokens;
            }

            var spans = new List<Span>();
            var currentStart = span.Start;
            foreach (var splitToken in splitTokens)
            {
                spans.Add(new Span(currentStart, currentStart + splitToken.Length));
                currentStart += splitToken.Length;
            }
            return spans;
        }
    }
}
