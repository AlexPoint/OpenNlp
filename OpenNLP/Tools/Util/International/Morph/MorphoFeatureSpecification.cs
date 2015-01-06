using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.International.Morph
{
    /**
 * Morphological feature specification for surface forms in a given language.
 * Currently supported feature names are the values of MorphFeatureType.
 * 
 * @author Spence Green
 *
 */

    public abstract class MorphoFeatureSpecification
    {
        private static readonly long serialVersionUID = -5720683653931585664L;

        //Delimiter for associating a surface form with a morphological analysis, e.g.,
        //
        //     his~#PRP_3ms
        //
        public static readonly string MORPHO_MARK = "~#";

        public static readonly string LEMMA_MARK = "|||";

        public static readonly string NO_ANALYSIS = "XXX";

        // WSGDEBUG --
        //   Added NNUM and NGEN for nominals in Arabic
        public enum MorphoFeatureType
        {
            TENSE,
            DEF,
            ASP,
            MOOD,
            NNUM,
            NUM,
            NGEN,
            GEN,
            CASE,
            PER,
            POSS,
            VOICE,
            OTHER,
            PROP
        };

        protected readonly Set<MorphoFeatureType> activeFeatures;

        public MorphoFeatureSpecification()
        {
            activeFeatures = new HashSet<MorphoFeatureType>();
            //activeFeatures = Generics.newHashSet();
        }

        public void activate(MorphoFeatureType feat)
        {
            activeFeatures.Add(feat);
        }

        public bool isActive(MorphoFeatureType feat)
        {
            return activeFeatures.Contains(feat);
        }

        public abstract List<string> getValues(MorphoFeatureType feat);

        public abstract MorphoFeatures strToFeatures(string spec);

        /**
   * Returns the lemma as pair.first() and the morph analysis as pair.second().
   */

        public static Tuple<string, string> splitMorphString(string word, string morphStr)
        {
            if (morphStr == null || morphStr.Trim().Equals(""))
            {
                return new Tuple<string, string>(word, NO_ANALYSIS);
            }
            string[] toks = morphStr.Split(new[] {ToLiteral(LEMMA_MARK)}, StringSplitOptions.None);
            if (toks.Length != 2)
            {
                throw new Exception("Invalid morphology string: " + morphStr);
            }
            return new Tuple<string, string>(toks[0], toks[1]);
        }


        //@Override
        public override string ToString()
        {
            return activeFeatures.ToString();
        }

        private static string ToLiteral(string input)
        {
            using (var writer = new StringWriter())
            {
                using (var provider = CodeDomProvider.CreateProvider("CSharp"))
                {
                    provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
                    return writer.ToString();
                }
            }
        }
    }
}