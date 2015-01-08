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
    /// <summary>
    /// Morphological feature specification for surface forms in a given language.
    /// Currently supported feature names are the values of MorphFeatureType.
    /// 
    /// @author Spence Green
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public abstract class MorphoFeatureSpecification
    {
        /// <summary>
        /// Delimiter for associating a surface form with a morphological analysis, e.g., his~#PRP_3ms
        /// </summary>
        public static readonly string MorphoMark = "~#";

        public static readonly string LemmaMark = "|||";

        public static readonly string NoAnalysis = "XXX";

        /// <summary>
        /// WSGDEBUG -- Added NNUM and NGEN for nominals in Arabic
        /// </summary>
        public enum MorphoFeatureType
        {
            Tense,
            Def,
            Asp,
            Mood,
            Nnum,
            Num,
            Ngen,
            Gen,
            Case,
            Per,
            Poss,
            Voice,
            Other,
            Prop
        };

        protected readonly Set<MorphoFeatureType> ActiveFeatures;

        public MorphoFeatureSpecification()
        {
            ActiveFeatures = new HashSet<MorphoFeatureType>();
            //activeFeatures = Generics.newHashSet();
        }

        public void Activate(MorphoFeatureType feat)
        {
            ActiveFeatures.Add(feat);
        }

        public bool IsActive(MorphoFeatureType feat)
        {
            return ActiveFeatures.Contains(feat);
        }

        public abstract List<string> GetValues(MorphoFeatureType feat);

        public abstract MorphoFeatures StrToFeatures(string spec);

        /// <summary>
        /// Returns the lemma as pair.first() and the morph analysis as pair.second()
        /// </summary>
        public static Tuple<string, string> SplitMorphString(string word, string morphStr)
        {
            if (morphStr == null || morphStr.Trim().Equals(""))
            {
                return new Tuple<string, string>(word, NoAnalysis);
            }
            string[] toks = morphStr.Split(new[] {ToLiteral(LemmaMark)}, StringSplitOptions.None);
            if (toks.Length != 2)
            {
                throw new Exception("Invalid morphology string: " + morphStr);
            }
            return new Tuple<string, string>(toks[0], toks[1]);
        }


        public override string ToString()
        {
            return ActiveFeatures.ToString();
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