using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util;

namespace OpenNLP.Tools.Tokenize
{
    /// <summary>
    /// Class containing a data set for testing a Tokenizer, 
    /// ie. a sentence and the collection of spans (representing the tokens) associated.
    /// </summary>
    public class TokenizerTestData
    {
        public string SentenceWithMarks { get; private set; }
        public string TokenMarker { get; private set; }

        public TokenizerTestData(string sentenceWithMarks, string tokenMarker)
        {
            this.SentenceWithMarks = sentenceWithMarks;
            this.TokenMarker = tokenMarker;
        }

        public string GetCleanSentence()
        {
            return !string.IsNullOrEmpty(this.SentenceWithMarks) ? 
                this.SentenceWithMarks.Replace(TokenMarker, "") : "";
        }

        public List<Span> GetSpans()
        {
            int tokenStart = -1;
            var spans = new List<Span>();
            bool isInToken = false;
            int endPosition = this.SentenceWithMarks.Length;
            var tokenOffset = 0;
            for (int currentChar = 0; currentChar < endPosition; currentChar++)
            {
                var character = this.SentenceWithMarks[currentChar];
                if (char.IsWhiteSpace(character) || character == '|')
                {
                    if (isInToken)
                    {
                        spans.Add(new Span(tokenStart - tokenOffset, currentChar - tokenOffset));
                        isInToken = false;
                        tokenStart = -1;
                    }
                    if (character == '|')
                    {
                        tokenOffset++;
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
                spans.Add(new Span(tokenStart - tokenOffset, endPosition - tokenOffset));
            }

            return spans;
        }

        public int GetNumberOfWhitespaceOccurencesInSentence()
        {
            return !string.IsNullOrEmpty(this.SentenceWithMarks)
                ? this.SentenceWithMarks.Count(c => c == ' ')
                : 0;
        }
    }
}
