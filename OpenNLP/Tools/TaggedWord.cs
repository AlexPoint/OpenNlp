using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools
{
    /// <summary>
    /// A word tagged by a part of speech tagger
    /// ex: will/MD | to/TO | etc.
    /// </summary>
    public class TaggedWord
    {
        public string Tag { get; set; }
        public string Word { get; set; }
        public int Index { get; set; }


        // Constructors ----------------

        /// <summary>
        /// Constructor for string of format "will/MD"
        /// </summary>
        public TaggedWord(string stringTaggedWord, int indexInGroup)
        {
            if (stringTaggedWord.Contains("/"))
            {
                this.Word = stringTaggedWord.Split('/').First();
                this.Tag = stringTaggedWord.Split('/').Last();
                this.Index = indexInGroup;
            }
        }

        public TaggedWord(string word, string tag, int index)
        {
            this.Word = word;
            this.Tag = tag;
            this.Index = index;
        }

        // Methods -------------------

        public override string ToString()
        {
            return string.Format("{0}/{1}", this.Word, this.Tag);
        }

        /*// Utilities -----------------

        public static string RecreateSentence(List<TaggedWord> taggedWords)
        {
            return RecreateSentence(taggedWords, tw => tw.Word);
        }

        public static string RecreateSentence(List<TaggedWord> taggedWords, Func<TaggedWord, string> printTaggedWord)
        {
            var detokenizer = new DictionaryDetokenizer();

            var tokens = taggedWords.OrderBy(w => w.Index)
                .Select(printTaggedWord)
                .ToArray();
            return detokenizer.Detokenize(tokens, "");
        }*/
    }
}
