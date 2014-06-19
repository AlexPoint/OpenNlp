using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Tokenize
{
    /// <summary>
    /// A detokenizer based on regex.
    /// For a given token, find the regex that matches and apply the corresponding detokenization operation.
    /// </summary>
    public class RegexDictionaryDetokenizer : Detokenizer
    {
        private readonly Dictionary<Regex, DetokenizationOperation> _regexToDetokenizationOperation;
        

        // Constructors -----------------

        public RegexDictionaryDetokenizer()
        {
            _regexToDetokenizationOperation = new Dictionary<Regex, DetokenizationOperation>()
            {
                // Punctuation
                {new Regex(@"^[\.,\!\?;\)\]'€]+"), DetokenizationOperation.MERGE_TO_LEFT},
                {new Regex(@"^[\(\[$]+"), DetokenizationOperation.MERGE_TO_RIGHT},
                /*{".", DetokenizationOperation.MERGE_TO_LEFT},
                {"...", DetokenizationOperation.MERGE_TO_LEFT},
                {",", DetokenizationOperation.MERGE_TO_LEFT},
                {"!", DetokenizationOperation.MERGE_TO_LEFT},
                {"?", DetokenizationOperation.MERGE_TO_LEFT},
                {";", DetokenizationOperation.MERGE_TO_LEFT},*/
                /*{"(", DetokenizationOperation.MERGE_TO_RIGHT},*/
                /*{")", DetokenizationOperation.MERGE_TO_LEFT},*/
                /*{"[", DetokenizationOperation.MERGE_TO_RIGHT},*/
                /*{"]", DetokenizationOperation.MERGE_TO_LEFT},*/
                {new Regex(@"^\""+$"), DetokenizationOperation.RIGHT_LEFT_MATCHING},
                {new Regex(@"^-$"), DetokenizationOperation.MERGE_BOTH_IF_SURROUNDED_BY_WORDS},
                // Contractions
                /*{"'t", DetokenizationOperation.MERGE_TO_LEFT},
                {"'m", DetokenizationOperation.MERGE_TO_LEFT},
                {"'s", DetokenizationOperation.MERGE_TO_LEFT},
                {"'re", DetokenizationOperation.MERGE_TO_LEFT},
                {"'ve", DetokenizationOperation.MERGE_TO_LEFT},
                {"'d", DetokenizationOperation.MERGE_TO_LEFT},
                {"'ll", DetokenizationOperation.MERGE_TO_LEFT},*/
                // Currencies
                /*{"$", DetokenizationOperation.MERGE_TO_RIGHT},*/
                /*{"€", DetokenizationOperation.MERGE_TO_LEFT},*/
            };
        }

        public RegexDictionaryDetokenizer(Dictionary<string, DetokenizationOperation> dict)
        {
            // compile regex for performance
            this._regexToDetokenizationOperation = dict.ToDictionary(ent => new Regex(ent.Key, RegexOptions.Compiled), ent => ent.Value);
        }


        // Methods ----------------------------------------------------

        private static readonly Regex EndByWordRegex = new Regex(@"\w+$");
        private static readonly Regex StartByWordRegex = new Regex(@"^\w+");

        public override DetokenizationOperation[] GetDetokenizationOperations(string[] tokens)
        {
            var operations = new DetokenizationOperation[tokens.Length];

            var matchingTokens = new HashSet<string>();

            for (int i = 0; i < tokens.Length; i++)
            {
                var matchingRegexes = _regexToDetokenizationOperation
                    .Where(ent => ent.Key.IsMatch(tokens[i]))
                    .ToList();

                if (!matchingRegexes.Any())
                {
                    operations[i] = DetokenizationOperation.NO_OPERATION;
                    continue;
                }
                else
                {
                    if (matchingRegexes.Count > 1)
                    {
                        // TODO: log issue, should not happen
                    }
                    var operation = matchingRegexes.First().Value;

                    if (operation == DetokenizationOperation.MERGE_TO_LEFT
                    || operation == DetokenizationOperation.MERGE_TO_RIGHT
                    || operation == DetokenizationOperation.MERGE_BOTH)
                    {
                        operations[i] = operation;
                    }
                    else if (operation == DetokenizationOperation.MERGE_BOTH_IF_SURROUNDED_BY_WORDS)
                    {
                        if (0 < i && i < tokens.Length - 1 && EndByWordRegex.IsMatch(tokens[i - 1]) && StartByWordRegex.IsMatch(tokens[i + 1]))
                        {
                            operations[i] = DetokenizationOperation.MERGE_BOTH;
                        }
                        else
                        {
                            operations[i] = DetokenizationOperation.NO_OPERATION;
                        }
                    }
                    else if (operation == DetokenizationOperation.RIGHT_LEFT_MATCHING)
                    {
                        if (matchingTokens.Contains(tokens[i]))
                        {
                            // The token already occurred once, move it to the left and clear the occurrence flag
                            operations[i] = DetokenizationOperation.MERGE_TO_LEFT;
                            matchingTokens.Remove(tokens[i]);
                        }
                        else
                        {
                            // First time this token is seen, move it to the right and remember it
                            operations[i] = DetokenizationOperation.MERGE_TO_RIGHT;
                            matchingTokens.Add(tokens[i]);
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
