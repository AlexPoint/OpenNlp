using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Tokenize
{
    /// <summary>
    /// An abstract for Detokenizers implementing the Detokenize method
    /// gluing back together tokens given a collection of DetokenizationOperations.
    /// </summary>
    public abstract class Detokenizer: IDetokenizer
    {
        /// <summary>
        /// Computes a collection of detokenization operations given a collection of tokens.
        /// </summary>
        public abstract DetokenizationOperation[] GetDetokenizationOperations(string[] tokens);

        /// <summary>
        /// Plugs back a list of tokens, split from the same sentence with a tokenizer
        /// </summary>
        /// <param name="tokens">The collection of tokens</param>
        /// <param name="splitMarker">A specific marker to insert between tokens</param>
        public string Detokenize(string[] tokens, string splitMarker = "")
        {
            DetokenizationOperation[] operations = GetDetokenizationOperations(tokens);

            if (tokens.Length != operations.Length)
            {
                throw new ArgumentException("tokens and operations array must have same length: tokens=" +
                                            tokens.Length + ", operations=" + operations.Length + "!");
            }

            var untokenizedString = new StringBuilder();
            for (int i = 0; i < tokens.Length; i++)
            {
                // attach token to string buffer
                untokenizedString.Append(tokens[i]);

                bool isAppendSpace;
                bool isAppendSplitMarker;

                // if this token is the last token do not attach a space
                if (i + 1 == operations.Length)
                {
                    isAppendSpace = false;
                    isAppendSplitMarker = false;
                }
                // if next token move left, no space after this token,
                // its safe to access next token
                else if (operations[i + 1] == DetokenizationOperation.MERGE_TO_LEFT
                            || operations[i + 1] == DetokenizationOperation.MERGE_BOTH)
                {
                    isAppendSpace = false;
                    isAppendSplitMarker = true;
                }
                // if this token is move right, no space
                else if (operations[i] == DetokenizationOperation.MERGE_TO_RIGHT
                            || operations[i] == DetokenizationOperation.MERGE_BOTH)
                {
                    isAppendSpace = false;
                    isAppendSplitMarker = true;
                }
                else
                {
                    isAppendSpace = true;
                    isAppendSplitMarker = false;
                }

                if (isAppendSpace)
                {
                    untokenizedString.Append(' ');
                }

                if (isAppendSplitMarker && splitMarker != null)
                {
                    untokenizedString.Append(splitMarker);
                }
            }

            return untokenizedString.ToString();
        }
    }
}
