using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Ling
{
    public class AnnotationLookup
    {
        private AnnotationLookup() {}

  public class KeyLookup<T> {
    private KeyLookup VALUE_KEY = new KeyLookup(CoreAnnotations.ValueAnnotation, OldFeatureLabelKeys.VALUE_KEY),
    TAG_KEY(typeof(CoreAnnotations.PartOfSpeechAnnotation), OldFeatureLabelKeys.TAG_KEY),
    WORD_KEY(typeof(CoreAnnotations.TextAnnotation), OldFeatureLabelKeys.WORD_KEY),
    LEMMA_KEY(typeof(CoreAnnotations.LemmaAnnotation), OldFeatureLabelKeys.LEMMA_KEY),
    CATEGORY_KEY(typeof(CoreAnnotations.CategoryAnnotation), OldFeatureLabelKeys.CATEGORY_KEY),
    PROJ_CAT_KEY(typeof(CoreAnnotations.ProjectedCategoryAnnotation), OldFeatureLabelKeys.PROJ_CAT_KEY),
    HEAD_WORD_KEY("edu.stanford.nlp.ling.TreeCoreAnnotations.HeadWordAnnotation", OldFeatureLabelKeys.HEAD_WORD_KEY),
    HEAD_TAG_KEY("edu.stanford.nlp.ling.TreeCoreAnnotations.HeadTagAnnotation", OldFeatureLabelKeys.HEAD_TAG_KEY),
    INDEX_KEY(typeof(CoreAnnotations.IndexAnnotation), OldFeatureLabelKeys.INDEX_KEY),
    ARG_KEY(typeof(CoreAnnotations.ArgumentAnnotation), OldFeatureLabelKeys.ARG_KEY),
    MARKING_KEY(typeof(CoreAnnotations.MarkingAnnotation), OldFeatureLabelKeys.MARKING_KEY),
    SEMANTIC_HEAD_WORD_KEY(typeof(CoreAnnotations.SemanticHeadWordAnnotation), OldFeatureLabelKeys.SEMANTIC_HEAD_WORD_KEY),
    SEMANTIC_HEAD_POS_KEY(typeof(CoreAnnotations.SemanticHeadTagAnnotation), OldFeatureLabelKeys.SEMANTIC_HEAD_POS_KEY),
    VERB_SENSE_KEY(typeof(CoreAnnotations.VerbSenseAnnotation), OldFeatureLabelKeys.VERB_SENSE_KEY),
    CATEGORY_FUNCTIONAL_TAG_KEY(typeof(CoreAnnotations.CategoryFunctionalTagAnnotation), OldFeatureLabelKeys.CATEGORY_FUNCTIONAL_TAG_KEY),
    NER_KEY(typeof(CoreAnnotations.NamedEntityTagAnnotation), OldFeatureLabelKeys.NER_KEY),
    SHAPE_KEY(typeof(CoreAnnotations.ShapeAnnotation), OldFeatureLabelKeys.SHAPE_KEY),
    LEFT_TERM_KEY(typeof(CoreAnnotations.LeftTermAnnotation), OldFeatureLabelKeys.LEFT_TERM_KEY),
    PARENT_KEY(typeof(CoreAnnotations.ParentAnnotation), OldFeatureLabelKeys.PARENT_KEY),
    SPAN_KEY(typeof(CoreAnnotations.SpanAnnotation), OldFeatureLabelKeys.SPAN_KEY),
    BEFORE_KEY(typeof(CoreAnnotations.BeforeAnnotation), OldFeatureLabelKeys.BEFORE_KEY),
    AFTER_KEY(typeof(CoreAnnotations.AfterAnnotation), OldFeatureLabelKeys.AFTER_KEY),
    CURRENT_KEY(typeof(CoreAnnotations.OriginalTextAnnotation), OldFeatureLabelKeys.CURRENT_KEY),
    ANSWER_KEY(typeof(CoreAnnotations.AnswerAnnotation), OldFeatureLabelKeys.ANSWER_KEY),
    GOLDANSWER_Key(typeof(CoreAnnotations.GoldAnswerAnnotation), OldFeatureLabelKeys.GOLDANSWER_KEY),
    FEATURES_KEY(typeof(CoreAnnotations.FeaturesAnnotation), OldFeatureLabelKeys.FEATURES_KEY),
    INTERPRETATION_KEY(typeof(CoreAnnotations.InterpretationAnnotation), OldFeatureLabelKeys.INTERPRETATION_KEY),
    ROLE_KEY(typeof(CoreAnnotations.RoleAnnotation), OldFeatureLabelKeys.ROLE_KEY),
    GAZETTEER_KEY(typeof(CoreAnnotations.GazetteerAnnotation), OldFeatureLabelKeys.GAZETTEER_KEY),
    STEM_KEY(typeof(CoreAnnotations.StemAnnotation), OldFeatureLabelKeys.STEM_KEY),
    POLARITY_KEY(typeof(CoreAnnotations.PolarityAnnotation), OldFeatureLabelKeys.POLARITY_KEY),
    CH_CHAR_KEY(typeof(CoreAnnotations.ChineseCharAnnotation), OldFeatureLabelKeys.CH_CHAR_KEY),
    CH_ORIG_SEG_KEY(typeof(CoreAnnotations.ChineseOrigSegAnnotation), OldFeatureLabelKeys.CH_ORIG_SEG_KEY),
    CH_SEG_KEY(typeof(CoreAnnotations.ChineseSegAnnotation), OldFeatureLabelKeys.CH_SEG_KEY),
    BEGIN_POSITION_KEY(typeof(CoreAnnotations.CharacterOffsetBeginAnnotation), OldFeatureLabelKeys.BEGIN_POSITION_KEY),
    END_POSITION_KEY(typeof(CoreAnnotations.CharacterOffsetEndAnnotation), OldFeatureLabelKeys.END_POSITION_KEY),
    DOCID_KEY(typeof(CoreAnnotations.DocIDAnnotation), OldFeatureLabelKeys.DOCID_KEY),
    SENTINDEX_KEY(typeof(CoreAnnotations.SentenceIndexAnnotation), OldFeatureLabelKeys.SENTINDEX_KEY),
    IDF_KEY(typeof(CoreAnnotations.IDFAnnotation), "idf"),
    END_POSITION_KEY2(typeof(CoreAnnotations.CharacterOffsetEndAnnotation), "endPosition"),
    CHUNK_KEY(typeof(CoreAnnotations.ChunkAnnotation), "chunk"),
    NORMALIZED_NER_KEY(typeof(CoreAnnotations.NormalizedNamedEntityTagAnnotation), "normalized"),
    MORPHO_NUM_KEY(typeof(CoreAnnotations.MorphoNumAnnotation),"num"),
    MORPHO_PERS_KEY(typeof(CoreAnnotations.MorphoPersAnnotation),"pers"),
    MORPHO_GEN_KEY(typeof(CoreAnnotations.MorphoGenAnnotation),"gen"),
    MORPHO_CASE_KEY(typeof(CoreAnnotations.MorphoCaseAnnotation),"case"),
    WORDNET_SYN_KEY(typeof(CoreAnnotations.WordnetSynAnnotation),"wordnetsyn"),
    PROTO_SYN_KEY(typeof(CoreAnnotations.ProtoAnnotation),"proto"),
    DOCTITLE_KEY(typeof(CoreAnnotations.DocTitleAnnotation),"doctitle"),
    DOCTYPE_KEY(typeof(CoreAnnotations.DocTypeAnnotation),"doctype"),
    DOCDATE_KEY(typeof(CoreAnnotations.DocDateAnnotation),"docdate"),
    DOCSOURCETYPE_KEY(typeof(CoreAnnotations.DocSourceTypeAnnotation),"docsourcetype"),
    LINK_KEY(typeof(CoreAnnotations.LinkAnnotation),"link"),
    SPEAKER_KEY(typeof(CoreAnnotations.SpeakerAnnotation),"speaker"),
    AUTHOR_KEY(typeof(CoreAnnotations.AuthorAnnotation),"author"),
    SECTION_KEY(typeof(CoreAnnotations.SectionAnnotation),"section"),
    SECTIONID_KEY(typeof(CoreAnnotations.SectionIDAnnotation),"sectionID"),
    SECTIONDATE_KEY(typeof(CoreAnnotations.SectionDateAnnotation),"sectionDate"),

    // Thang Sep13: for Genia NER
    HEAD_KEY(typeof(CoreAnnotations.HeadWordStringAnnotation), "head"),
    GOVERNOR_KEY(typeof(CoreAnnotations.GovernorAnnotation), "governor"),
    GAZ_KEY(typeof(CoreAnnotations.GazAnnotation), "gaz"),
    ABBR_KEY(typeof(CoreAnnotations.AbbrAnnotation), "abbr"),
    ABSTR_KEY(typeof(CoreAnnotations.AbstrAnnotation), "abstr"),
    FREQ_KEY(typeof(CoreAnnotations.FreqAnnotation), "freq"),
    WEB_KEY(typeof(CoreAnnotations.WebAnnotation), "web"),

    // Also have "pos" for PartOfTag (POS is also the TAG_KEY - "tag", but "pos" makes more sense)
    // Still keep "tag" for POS tag so we don't break anything
    POS_TAG_KEY(CoreAnnotations.PartOfSpeechAnnotation), "pos");


    public readonly object coreKey;
    public readonly String oldKey;

    private /*<T>*/ KeyLookup(/*Class<? extends CoreAnnotation<T>>*/CoreAnnotation<T> coreKey, String oldKey) {
      this.coreKey = coreKey;
      this.oldKey = oldKey;
    }

    /**
     * This constructor allows us to use reflection for loading old class keys.
     * This is useful because we can then create distributions that do not have
     * all of the classes required for all the old keys (such as trees package classes).
     */
    private KeyLookup(String className, String oldKey) {
      //Class<?> keyClass;
        Type keyClass;
      try {
       //keyClass = Class.forName(className);
          keyClass = Type.GetType(className);
      } catch(SystemException e) {
          CoreLabel.GenericAnnotation<Object> newKey = () => typeof(Object);
        keyClass = newKey.getClass();
      }
      this.coreKey = ErasureUtils.uncheckedCast(keyClass);
      this.oldKey = oldKey;
    }


  }

  /**
   * Returns a CoreAnnotation class key for the given old-style FeatureLabel
   * key if one exists; null otherwise.
   */
  public static KeyLookup getCoreKey(String oldKey) {
    foreach (KeyLookup lookup in KeyLookup.values()) {
      if (lookup.oldKey.equals(oldKey)) {
        return lookup;
      }
    }
    return null;
  }

  private static Map<Class<CoreAnnotation<?>>,Class<?>> valueCache = Generics.newHashMap();

  /**
   * Returns the runtime value type associated with the given key.  Caches
   * results.
   */
  //@SuppressWarnings("unchecked")
  public static Class<?> getValueType(Class<? extends CoreAnnotation> key) {
    Class type = valueCache.get(key);
    if (type == null) {
      try {
        type = key.newInstance().getType();
      } catch (Exception e) {
        throw new RuntimeException("Unexpected failure to instantiate - is your key class fancy?", e);
      }
      valueCache.put((Class)key, type);
    }
    return type;
  }

  /**
   * Lookup table for mapping between old-style *Label keys and classes
   * the provide comparable backings in the core.
   */
//OLD keys kept around b/c we're kill IndexedFeatureLabel and these keys used to live there
  private static class OldFeatureLabelKeys {

    public static readonly String DOCID_KEY = "docID";
    public static readonly String SENTINDEX_KEY = "sentIndex";
    public static readonly Object WORD_FORMAT = "WORD_FORMAT";
    public static readonly Object WORD_TAG_FORMAT = "WORD_TAG_FORMAT";
    public static readonly Object WORD_TAG_INDEX_FORMAT = "WORD_TAG_INDEX_FORMAT";
    public static readonly Object VALUE_FORMAT = "VALUE_FORMAT";
    public static readonly Object COMPLETE_FORMAT = "COMPLETE_FORMAT";
    public static readonly String VALUE_KEY = "value";
    public static readonly String TAG_KEY = "tag";
    public static readonly String WORD_KEY = "word";
    public static readonly String LEMMA_KEY = "lemma";
    public static readonly String CATEGORY_KEY = "cat";
    public static readonly String PROJ_CAT_KEY = "pcat";
    public static readonly String HEAD_WORD_KEY = "hw";
    public static readonly String HEAD_TAG_KEY = "ht";
    public static readonly String INDEX_KEY = "idx";
    public static readonly String ARG_KEY = "arg";
    public static readonly String MARKING_KEY = "mark";
    public static readonly String SEMANTIC_HEAD_WORD_KEY = "shw";
    public static readonly String SEMANTIC_HEAD_POS_KEY = "shp";
    public static readonly String VERB_SENSE_KEY = "vs";
    public static readonly String CATEGORY_FUNCTIONAL_TAG_KEY = "cft";
    public static readonly String NER_KEY = "ner";
    public static readonly String SHAPE_KEY = "shape";
    public static readonly String LEFT_TERM_KEY = "LEFT_TERM";
    public static readonly String PARENT_KEY = "PARENT";
    public static readonly String SPAN_KEY = "SPAN";
    public static readonly String BEFORE_KEY = "before";
    public static readonly String AFTER_KEY = "after";
    public static readonly String CURRENT_KEY = "current";
    public static readonly String ANSWER_KEY = "answer";
    public static readonly String GOLDANSWER_KEY = "goldAnswer";
    public static readonly String FEATURES_KEY = "features";
    public static readonly String INTERPRETATION_KEY = "interpretation";
    public static readonly String ROLE_KEY = "srl";
    public static readonly String GAZETTEER_KEY = "gazetteer";
    public static readonly String STEM_KEY = "stem";
    public static readonly String POLARITY_KEY = "polarity";
    public static readonly String CH_CHAR_KEY = "char";
    public static readonly String CH_ORIG_SEG_KEY = "orig_seg"; // the segmentation info existing in the original text
    public static readonly String CH_SEG_KEY = "seg"; // the segmentation information from the segmenter
    public static readonly String BEGIN_POSITION_KEY = "BEGIN_POS";
    public static readonly String END_POSITION_KEY = "END_POS";


    /*private OldFeatureLabelKeys() {
    }*/

  } // end static class OldFeatureLabelKeys
    }
}
