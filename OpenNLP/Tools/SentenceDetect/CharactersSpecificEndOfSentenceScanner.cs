using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.SentenceDetect
{
    /// <summary>
    /// End of sentence scanner based on a collection of characters,
    /// ie only a list of specific characters can indicate a potential end of sentence, nothing else.
    /// </summary>
    public class CharactersSpecificEndOfSentenceScanner : IEndOfSentenceScanner
    {

        private readonly List<char> _endOfSentencePotentialCharacters;


        // Constructors --------------
        
        /// <summary>
        /// Constructor to specify the potential end of sentence characters 
        /// </summary>
        public CharactersSpecificEndOfSentenceScanner(params char[] c)
        {
            _endOfSentencePotentialCharacters = c.ToList();
        }


        // Methods -------------------

        public List<int> GetPositions(string input)
        {
            return GetPositions(input.ToCharArray());
        }

        public List<int> GetPositions(StringBuilder buffer)
        {
            return GetPositions(buffer.ToString().ToCharArray());
        }

        public List<int> GetPositions(char[] characterBuffer)
        {
            var positionList = new List<int>();

            for (int currentChar = 0; currentChar < characterBuffer.Length; currentChar++)
            {
                var character = characterBuffer[currentChar];
                if (_endOfSentencePotentialCharacters.Contains(character))
                {
                    positionList.Add(currentChar);
                }
            }
            return positionList;
        }

        public List<char> GetPotentialEndOfSentenceCharacters()
        {
            return _endOfSentencePotentialCharacters;
        }
    }
}
