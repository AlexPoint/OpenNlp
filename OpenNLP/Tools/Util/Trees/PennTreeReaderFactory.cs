using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees
{
    public class PennTreeReaderFactory : TreeReaderFactory
    {
        private readonly TreeFactory tf;
        private readonly TreeNormalizer tn;

        /**
   * Default constructor; uses a {@link LabeledScoredTreeFactory},
   * with StringLabels, a {@link PennTreebankTokenizer},
   * and a {@link TreeNormalizer}.
   */

        public PennTreeReaderFactory() :
            this(new LabeledScoredTreeFactory())
        {
        }

        /**
   * Specify your own {@link TreeFactory};
   * uses a {@link PennTreebankTokenizer}, and a {@link TreeNormalizer}.
   *
   * @param tf The TreeFactory to use in building Tree objects to return.
   */

        public PennTreeReaderFactory(TreeFactory tf) :
            this(tf, new TreeNormalizer())
        {
        }


        /**
   * Specify your own {@link TreeNormalizer};
   * uses a {@link PennTreebankTokenizer}, and a {@link LabeledScoredTreeFactory}.
   *
   * @param tn The TreeNormalizer to use in building Tree objects to return.
   */

        public PennTreeReaderFactory(TreeNormalizer tn) :
            this(new LabeledScoredTreeFactory(), tn)
        {
        }


        /**
   * Specify your own {@link TreeFactory};
   * uses a {@link PennTreebankTokenizer}, and a {@link TreeNormalizer}.
   *
   * @param tf The TreeFactory to use in building Tree objects to return.
   * @param tn The TreeNormalizer to use
   */

        public PennTreeReaderFactory(TreeFactory tf, TreeNormalizer tn)
        {
            this.tf = tf;
            this.tn = tn;
        }


        //@Override
        public TreeReader newTreeReader(TextReader input)
        {
            return new PennTreeReader(input, tf, tn, new PennTreebankTokenizer(input));
        }
    }
}