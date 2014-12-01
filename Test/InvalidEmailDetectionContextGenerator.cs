using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SharpEntropy;

namespace Test
{
    public class InvalidEmailDetectionContextGenerator: IContextGenerator<string>
    {
        private readonly Regex _vowelsRegex = new Regex("[aeiouy]+", RegexOptions.Compiled);
        private readonly Regex _digitsRegex = new Regex("\\d+", RegexOptions.Compiled);

        public string[] GetContext(string input)
        {
            var results = new List<string>();

            var parts = input.Split('@');
            var rootPart = parts.First();
            var domain = parts.Last();
            var domainParts = domain.Split('.');

            // root part

            // nb of characters
            var nbOfCharacters = rootPart.Length;
            results.Add("nb=" + nbOfCharacters);
            // nb of different characters
            var nbOfDifferentCharacters = rootPart.ToCharArray().Distinct().Count();
            results.Add("nbdiff=" + nbOfDifferentCharacters);
            // percent of distinct characters
            var percentOfDistinctCharacters = ((nbOfDifferentCharacters*100)/(5*nbOfCharacters))*5;//only 5 in 5
            results.Add("perDistChar="+ percentOfDistinctCharacters);

            // has vowels
            var hasVowels = _vowelsRegex.IsMatch(rootPart);
            results.Add("hVow=" + hasVowels);
            // has digits
            var hasDigits = _digitsRegex.IsMatch(rootPart);
            results.Add("hDig=" + hasDigits);
            // contains '.'
            var hasDot = rootPart.Contains(".");
            results.Add("hDot=" + hasDot);
            // contains '_'
            var hasUnderscore = rootPart.Contains("_");
            results.Add("hUnders=" + hasUnderscore);
            // contains '-'
            var hasDash = rootPart.Contains("-");
            results.Add("hDash=" + hasDash);

            // repeated n-grams
            for (var i = 2; i <= 3; i++)
            {
                var nGramsToOccurrences = new Dictionary<string, int>();
                for (var j = 0; j < rootPart.Length - i + 1; j++)
                {
                    var ngram = rootPart.Substring(j, i);
                    if (!nGramsToOccurrences.ContainsKey(ngram))
                    {
                        var nbOfOccurences = rootPart.Split(new[] {ngram},StringSplitOptions.None).Count() - 1;
                        if (nbOfOccurences > 1)
                        {
                            nGramsToOccurrences.Add(ngram, nbOfOccurences); 
                        }
                    }
                }

                var nbOfRepetitionOfRepeatedNgrams = nGramsToOccurrences.Sum(ent => ent.Value);
                if (nbOfRepetitionOfRepeatedNgrams > 0)
                {
                    results.Add("ngram" + i + "=" + nbOfRepetitionOfRepeatedNgrams); 
                }
            }

            // add domain and extension
            results.Add("d=" + domainParts.First());
            results.Add("dExt=" + domainParts.Last());

            return results.ToArray();
        }
    }
}
