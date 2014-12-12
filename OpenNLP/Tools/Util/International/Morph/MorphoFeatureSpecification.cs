using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.International.Morph
{
    public abstract class MorphoFeatureSpecification
    {
        private static readonly long serialVersionUID = -5720683653931585664L;

  //Delimiter for associating a surface form with a morphological analysis, e.g.,
  //
  //     his~#PRP_3ms
  //
  public static readonly String MORPHO_MARK = "~#";
  
  public static readonly String LEMMA_MARK = "|||";
  
  public static readonly String NO_ANALYSIS = "XXX";
  
  // WSGDEBUG --
  //   Added NNUM and NGEN for nominals in Arabic
  public enum MorphoFeatureType {TENSE,DEF,ASP,MOOD,NNUM,NUM, NGEN, GEN,CASE,PER,POSS,VOICE,OTHER,PROP};
  
  protected readonly Set<MorphoFeatureType> activeFeatures;
  
  public MorphoFeatureSpecification() {
    activeFeatures = new HashSet<MorphoFeatureType>();
    //activeFeatures = Generics.newHashSet();
  }
  
  public void activate(MorphoFeatureType feat) {
    activeFeatures.Add(feat);
  }
  
  public bool isActive(MorphoFeatureType feat) { return activeFeatures.Contains(feat); }
  
  public abstract List<String> getValues(MorphoFeatureType feat);
  
  public abstract MorphoFeatures strToFeatures(String spec);
  
  /**
   * Returns the lemma as pair.first() and the morph analysis as pair.second().
   */
  public static Pair<String,String> splitMorphString(String word, String morphStr) {
    if (morphStr == null || morphStr.trim().equals("")) {
      return new Pair<String,String>(word, NO_ANALYSIS);
    }
    String[] toks = morphStr.split(Pattern.quote(LEMMA_MARK));
    if (toks.Length != 2) {
      throw new Exception("Invalid morphology string: " + morphStr);
    }
    return new Pair<String,String>(toks[0], toks[1]); 
  }
  
  
  //@Override
  public override String ToString() { return activeFeatures.ToString(); }
    }
}
