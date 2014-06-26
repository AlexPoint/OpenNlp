using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenNLP.Tools.Tokenize;

namespace OpenNLP.Tools.Chunker
{
    public class RegexDictionaryDechunker : Dechunker
    {
        private readonly Dictionary<Regex, DechunkOperation> _regexToDechunkOperation;
        

        // Constructors -----------------

        public RegexDictionaryDechunker()
        {
            _regexToDechunkOperation = new Dictionary<Regex, DechunkOperation>()
            {
                // starts with punctuation, apostrophe, ), ] or € symbol --> merge to left
                {new Regex(@"^[\.,\!\?;\)\]'€]+", RegexOptions.Compiled), DechunkOperation.MERGE_TO_LEFT},
                // starts with (, [ r $ --> merge to right
                {new Regex(@"^[\(\[$]+", RegexOptions.Compiled), DechunkOperation.MERGE_TO_RIGHT},
                // behavior before/after " depends on the " occurences before/after
                {new Regex(@"^\""+$", RegexOptions.Compiled), DechunkOperation.RIGHT_LEFT_MATCHING},
                // '-' --> merge both only if two words on both sides
                {new Regex(@"^-$", RegexOptions.Compiled), DechunkOperation.MERGE_BOTH_IF_SURROUNDED_BY_WORDS},
            };
        }

        public RegexDictionaryDechunker(Dictionary<string, DechunkOperation> dict)
        {
            // compile regex for performance
            this._regexToDechunkOperation = dict.ToDictionary(ent => new Regex(ent.Key, RegexOptions.Compiled), ent => ent.Value);
        }


        // Methods ----------------------------------------------------

        private static readonly Regex EndByWordRegex = new Regex(@"\w+$", RegexOptions.Compiled);
        private static readonly Regex StartByWordRegex = new Regex(@"^\w+", RegexOptions.Compiled);


        public override DechunkOperation[] GetDechunkerOperations(string[] chunks)
        {
            var operations = new DechunkOperation[chunks.Length];

            var matchingTokens = new HashSet<string>();

            for (int i = 0; i < chunks.Length; i++)
            {
                var matchingRegexes = _regexToDechunkOperation
                    .Where(ent => ent.Key.IsMatch(chunks[i]))
                    .ToList();

                if (!matchingRegexes.Any())
                {
                    operations[i] = DechunkOperation.NO_OPERATION;
                    continue;
                }
                else
                {
                    if (matchingRegexes.Count > 1)
                    {
                        // TODO: log issue, should not happen
                    }
                    var operation = matchingRegexes.First().Value;

                    if (operation == DechunkOperation.MERGE_TO_LEFT
                    || operation == DechunkOperation.MERGE_TO_RIGHT
                    || operation == DechunkOperation.MERGE_BOTH)
                    {
                        operations[i] = operation;
                    }
                    else if (operation == DechunkOperation.MERGE_BOTH_IF_SURROUNDED_BY_WORDS)
                    {
                        if (0 < i && i < chunks.Length - 1 && EndByWordRegex.IsMatch(chunks[i - 1]) && StartByWordRegex.IsMatch(chunks[i + 1]))
                        {
                            operations[i] = DechunkOperation.MERGE_BOTH;
                        }
                        else
                        {
                            operations[i] = DechunkOperation.NO_OPERATION;
                        }
                    }
                    else if (operation == DechunkOperation.RIGHT_LEFT_MATCHING)
                    {
                        if (matchingTokens.Contains(chunks[i]))
                        {
                            // The token already occurred once, move it to the left and clear the occurrence flag
                            operations[i] = DechunkOperation.MERGE_TO_LEFT;
                            matchingTokens.Remove(chunks[i]);
                        }
                        else
                        {
                            // First time this token is seen, move it to the right and remember it
                            operations[i] = DechunkOperation.MERGE_TO_RIGHT;
                            matchingTokens.Add(chunks[i]);
                        }
                    }
                    else
                    {
                        throw new InvalidEnumArgumentException("Unknown operation: " + operation);
                    }
                }
            }

            return operations;
        }

    }
}
