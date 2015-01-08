using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees
{
    /// <summary>
    /// Vends {@link PennTreeReader} objects.
    /// 
    /// @author Roger Levy (rog@nlp.stanford.edu)
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public class PennTreeReaderFactory : ITreeReaderFactory
    {
        private readonly ITreeFactory tf;
        private readonly TreeNormalizer tn;

        /// <summary>
        /// Default constructor; uses a {@link LabeledScoredTreeFactory},
        /// with StringLabels, a {@link PennTreebankTokenizer}, and a {@link TreeNormalizer}.
        /// </summary>
        public PennTreeReaderFactory() :
            this(new LabeledScoredTreeFactory())
        {
        }

        /// <summary>
        /// Specify your own {@link TreeFactory};
        /// uses a {@link PennTreebankTokenizer}, and a {@link TreeNormalizer}.
        /// </summary>
        /// <param name="tf">The TreeFactory to use in building Tree objects to return</param>
        public PennTreeReaderFactory(ITreeFactory tf) :
            this(tf, new TreeNormalizer())
        {
        }

        /// <summary>
        /// Specify your own {@link TreeNormalizer};
        /// uses a {@link PennTreebankTokenizer}, and a {@link LabeledScoredTreeFactory}.
        /// </summary>
        /// <param name="tn">The TreeNormalizer to use in building Tree objects to return</param>
        public PennTreeReaderFactory(TreeNormalizer tn) :
            this(new LabeledScoredTreeFactory(), tn)
        {
        }

        /// <summary>
        /// Specify your own {@link TreeFactory};
        /// uses a {@link PennTreebankTokenizer}, and a {@link TreeNormalizer}.
        /// </summary>
        /// <param name="tf">The TreeFactory to use in building Tree objects to return</param>
        /// <param name="tn">The TreeNormalizer to use</param>
        public PennTreeReaderFactory(ITreeFactory tf, TreeNormalizer tn)
        {
            this.tf = tf;
            this.tn = tn;
        }

        public ITreeReader NewTreeReader(TextReader input)
        {
            return new PennTreeReader(input, tf, tn, new PennTreebankTokenizer(input));
        }
    }
}