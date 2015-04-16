using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util;

namespace OpenNLP.Tools.Tokenize
{
    public abstract class AbstractTokenizer : ITokenizer
    {
        public abstract Span[] TokenizePositions(string input);
        
        /// <summary>Tokenize a string</summary>
        /// <param name="input">The string to be tokenized</param>
        /// <returns>A string array containing individual tokens as elements</returns>
        public virtual string[] Tokenize(string input)
        {
            Span[] tokenSpans = TokenizePositions(input);
            var tokens = new string[tokenSpans.Length];
            for (int currentToken = 0, tokenCount = tokens.Length; currentToken < tokenCount; currentToken++)
            {
                tokens[currentToken] = input.Substring(tokenSpans[currentToken].Start, tokenSpans[currentToken].Length());
            }
            return tokens;
        }

        public TokenizationTestResults RunAgainstTestData(List<TokenizerTestData> dataPoints)
        {
            var result = new TokenizationTestResults();

            foreach (var dataPoint in dataPoints)
            {
                var sentence = dataPoint.GetCleanSentence();
                var computedPositions = TokenizePositions(sentence);
                var correctPositions = dataPoint.GetSpans();

                var nbOfCorrectTokenizations = computedPositions.Intersect(correctPositions).Count();
                var nbOfIncorrectTokenizations = correctPositions.Except(computedPositions).Count();
                // count the number of tokens due to whitespaces (not relevant for the accuracy of the model)
                var nbOfWhiteSpaceTokens = dataPoint.GetNumberOfWhitespaceOccurencesInSentence() + 1;
                result.NbOfCorrectTokenizations += Math.Max(nbOfCorrectTokenizations - nbOfWhiteSpaceTokens, 0);
                result.NbOfIncorrectTokenizations += nbOfIncorrectTokenizations;
            }

            return result;
        }


        // Utilities ----------------------

        /// <summary>
        /// Constructs a list of Span objects, one for each whitespace delimited token.
        /// Token strings can be constructed form these spans as follows: input.Substring(span.Start, span.Length());
        /// </summary>
        /// <param name="input">string to tokenize</param>
        /// <returns>Array of spans</returns>
        internal static Span[] SplitOnWhitespaces(string input)
        {
            if (string.IsNullOrEmpty(input)) { return new Span[0]; }

            int tokenStart = -1;
            var tokens = new List<Span>();
            bool isInToken = false;

            //gather up potential tokens
            int endPosition = input.Length;
            for (int currentChar = 0; currentChar < endPosition; currentChar++)
            {
                if (char.IsWhiteSpace(input[currentChar]))
                {
                    if (isInToken)
                    {
                        tokens.Add(new Span(tokenStart, currentChar));
                        isInToken = false;
                        tokenStart = -1;
                    }
                }
                else
                {
                    if (!isInToken)
                    {
                        tokenStart = currentChar;
                        isInToken = true;
                    }
                }
            }
            if (isInToken)
            {
                tokens.Add(new Span(tokenStart, endPosition));
            }
            return tokens.ToArray();
        }
    }
}
