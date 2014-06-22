using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Chunker
{
    public class SentenceChunk
    {
        // Properties ----------------------

        public int IndexInSentence { get; set; }
        public string Tag { get; set; }
        public List<TaggedWord> TaggedWords { get; set; }


        // Constructors --------------------

        public SentenceChunk(int index)
        {
            this.IndexInSentence = index;
            this.TaggedWords = new List<TaggedWord>();
        }

        public SentenceChunk(string tag, int index):this(index)
        {
            this.Tag = tag;
        }


        // Methods ------------------------

        public override string ToString()
        {
            return string.Format("[{0}{1}]", !string.IsNullOrEmpty(this.Tag) ? this.Tag + " " : "", string.Join(" ", this.TaggedWords));
        }
    }
}
