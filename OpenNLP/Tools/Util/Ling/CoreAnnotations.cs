using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Ling
{
    /**
 * <p>
 * Set of common annotations for {@link CoreMap}s. The classes
 * defined here are typesafe keys for getting and setting annotation
 * values. These classes need not be instantiated outside of this
 * class. e.g {@link TextAnnotation}.class serves as the key and a
 * <code>String</code> serves as the value containing the
 * corresponding word.
 * </p>
 *
 * <p>
 * New types of {@link CoreAnnotation} can be defined anywhere that is
 * convenient in the source tree - they are just classes. This file exists to
 * hold widely used "core" annotations and others inherited from the
 * {@link Label} family. In general, most keys should be placed in this file as
 * they may often be reused throughout the code. This architecture allows for
 * flexibility, but in many ways it should be considered as equivalent to an
 * enum in which everything should be defined
 * </p>
 *
 * <p>
 * The getType method required by CoreAnnotation must return the same class type
 * as its value type parameter. It feels like one should be able to get away
 * without that method, but because Java erases the generic type signature, that
 * info disappears at runtime. See {@link ValueAnnotation} for an example.
 * </p>
 *
 * @author dramage
 * @author rafferty
 * @author bethard
 */

    public class CoreAnnotations
    {
        private CoreAnnotations()
        {
        } // only static members

        /**
   * The CoreMap key identifying the annotation's text.
   *
   * Note that this key is intended to be used with many different kinds of
   * annotations - documents, sentences and tokens all have their own text.
   */

        public class TextAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }


        /**
   * The CoreMap key for getting the lemma (morphological stem) of a token.
   *
   * This key is typically set on token annotations.
   *
   * TODO: merge with StemAnnotation?
   */

        public class LemmaAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * The CoreMap key for getting the Penn part of speech of a token.
   *
   * This key is typically set on token annotations.
   */

        public class PartOfSpeechAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * The CoreMap key for getting the token-level named entity tag (e.g., DATE,
   * PERSON, etc.)
   *
   * This key is typically set on token annotations.
   */

        public class NamedEntityTagAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * The CoreMap key for getting the token-level named entity tag (e.g., DATE,
   * PERSON, etc.) from a previous NER tagger. NERFeatureFactory is sensitive to
   * this tag and will turn the annotations from the previous NER tagger into
   * new features. This is currently used to implement one level of stacking --
   * we may later change it to take a list as needed.
   *
   * This key is typically set on token annotations.
   */

        public class StackedNamedEntityTagAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * The CoreMap key for getting the token-level true case annotation (e.g.,
   * INIT_UPPER)
   *
   * This key is typically set on token annotations.
   */

        public class TrueCaseAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * The CoreMap key identifying the annotation's true-cased text.
   *
   * Note that this key is intended to be used with many different kinds of
   * annotations - documents, sentences and tokens all have their own text.
   */

        public class TrueCaseTextAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * The CoreMap key for getting the tokens contained by an annotation.
   *
   * This key should be set for any annotation that contains tokens. It can be
   * done without much memory overhead using List.subList.
   */

        public class TokensAnnotation : CoreAnnotation<List<CoreLabel>>
        {
            public Type getType()
            {
                return typeof (List<CoreLabel>);
            }
        }

        /**
   * The CoreMap key for getting the tokens (can be words, phrases or anything that are of type CoreMap) contained by an annotation.
   *
   * This key should be set for any annotation that contains tokens (words, phrases etc). It can be
   * done without much memory overhead using List.subList.
   */

        public class GenericTokensAnnotation : CoreAnnotation<List<CoreMap>>
        {
            public Type getType()
            {
                return typeof (List<CoreMap>);
            }
        }

        /**
   * The CoreMap key for getting the sentences contained by an annotation.
   *
   * This key is typically set only on document annotations.
   */

        public class SentencesAnnotation : CoreAnnotation<List<CoreMap>>
        {
            public Type getType()
            {
                return typeof (List<CoreMap>);
            }
        }

        /**
   * The CoreMap key for getting the paragraphs contained by an annotation.
   *
   * This key is typically set only on document annotations.
   */

        public class ParagraphsAnnotation : CoreAnnotation<List<CoreMap>>
        {
            public Type getType()
            {
                return typeof (List<CoreMap>);
            }
        }

        /**
   * The CoreMap key identifying the first token included in an annotation. The
   * token with index 0 is the first token in the document.
   *
   * This key should be set for any annotation that contains tokens.
   */

        public class TokenBeginAnnotation : CoreAnnotation<int>
        {
            public Type getType()
            {
                return typeof (int);
            }
        }

        /**
   * The CoreMap key identifying the last token after the end of an annotation.
   * The token with index 0 is the first token in the document.
   *
   * This key should be set for any annotation that contains tokens.
   */

        public class TokenEndAnnotation : CoreAnnotation<int>
        {
            public Type getType()
            {
                return typeof (int);
            }
        }

        /**
   * The CoreMap key identifying the date and time associated with an
   * annotation.
   *
   * This key is typically set on document annotations.
   */

        public class CalendarAnnotation : CoreAnnotation<Calendar>
        {
            public Type getType()
            {
                return typeof (Calendar);
            }
        }

        /**
   * These are the keys hashed on by IndexedWord
   */
        /**
   * This refers to the unique identifier for a "document", where document may
   * vary based on your application.
   */

        public class DocIDAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * This indexes a token number inside a sentence.  Standardly, tokens are
   * indexed within a sentence starting at 1 (not 0: we follow common parlance
   * whereby we speak of the first word of a sentence).
   * This is generally an individual word or feature index - it is local, and
   * may not be uniquely identifying without other identifiers such as sentence
   * and doc. However, if these are the same, the index annotation should be a
   * unique identifier for differentiating objects.
   */

        public class IndexAnnotation : CoreAnnotation<int>
        {
            public Type getType()
            {
                return typeof (int);
            }
        }

        /**
   * This indexes the beginning of a span of words, e.g., a constituent in a
   * tree. See {@link edu.stanford.nlp.trees.Tree#indexSpans(int)}.
   * This annotation counts tokens.
   * It standardly indexes from 1 (like IndexAnnotation).  The reasons for
   * this are: (i) Talking about the first word of a sentence is kind of
   * natural, and (ii) We use index 0 to refer to an imaginary root in
   * dependency output.
   */

        public class BeginIndexAnnotation : CoreAnnotation<int>
        {
            public Type getType()
            {
                return typeof (int);
            }
        }

        /**
   * This indexes the end of a span of words, e.g., a constituent in a
   * tree.  See {@link edu.stanford.nlp.trees.Tree#indexSpans(int)}. This annotation
   * counts tokens.  It standardly indexes from 1 (like IndexAnnotation).
   * The end index is not a fencepost: its value is equal to the
   * IndexAnnotation of the last word in the span.
   */

        public class EndIndexAnnotation : CoreAnnotation<int>
        {
            public Type getType()
            {
                return typeof (int);
            }
        }

        /**
   * This indicates that starting at this token, the sentence should not be ended until
   * we see a ForcedSentenceEndAnnotation.  Used to force the ssplit annotator
   * (eg the WordToSentenceProcessor) to keep tokens in the same sentence
   * until ForcedSentenceEndAnnotation is seen.
   */

        public class ForcedSentenceUntilEndAnnotation
            : CoreAnnotation<Boolean>
        {
            public Type getType()
            {
                return typeof (Boolean);
            }
        }

        /**
   * This indicates the sentence should end at this token.  Used to
   * force the ssplit annotator (eg the WordToSentenceProcessor) to
   * start a new sentence at the next token.
   */

        public class ForcedSentenceEndAnnotation
            : CoreAnnotation<Boolean>
        {
            public Type getType()
            {
                return typeof (Boolean);
            }
        }

        /**
   * Unique identifier within a document for a given sentence.
   */

        public class SentenceIndexAnnotation : CoreAnnotation<int>
        {
            public Type getType()
            {
                return typeof (int);
            }
        }

        /**
   * Line number for a sentence in a document delimited by newlines
   * instead of punctuation.  May skip numbers if there are blank
   * lines not represented as sentences.  Indexed from 1 rather than 0.
   */

        public class LineNumberAnnotation : CoreAnnotation<int>
        {
            public Type getType()
            {
                return typeof (int);
            }
        }

        /**
   * Contains the "value" - an ill-defined string used widely in MapLabel.
   */

        public class ValueAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class CategoryAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * The exact original surface form of a token.  This is created in the
   * invertible PTBTokenizer. The tokenizer may normalize the token form to
   * match what appears in the PTB, but this key will hold the original characters.
   */

        public class OriginalTextAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * Annotation for the whitespace characters appearing before this word. This
   * can be filled in by the tokenizer so that the original text string can be
   * reconstructed.
   */

        public class BeforeAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * Annotation for the whitespace characters appear after this word. This can
   * be filled in by the tokenizer so that the original text string can be
   * reconstructed.
   */

        public class AfterAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * CoNLL dep parsing - coarser POS tags.
   */

        public class CoarseTagAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * CoNLL dep parsing - the dependency type
   */

        public class CoNLLDepAnnotation : CoreAnnotation<CoreMap>
        {
            public Type getType()
            {
                return typeof (CoreMap);
            }
        }

        /**
   * CoNLL SRL/dep parsing - whether the word is a predicate
   */

        public class CoNLLPredicateAnnotation : CoreAnnotation<Boolean>
        {
            public Type getType()
            {
                return typeof (Boolean);
            }
        }

        /**
   * CoNLL SRL/dep parsing - map which, for the current word, specifies its
   * specific role for each predicate
   */

        public class CoNLLSRLAnnotation : CoreAnnotation<Dictionary<int, String>>
        {
            public Type getType()
            {
                return typeof (Dictionary<int, String>);
            }
        }

        /**
   * CoNLL dep parsing - the dependency type
   */

        public class CoNLLDepTypeAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * CoNLL dep parsing - the index of the word which is the parent of this word
   * in the dependency tree
   */

        public class CoNLLDepParentIndexAnnotation : CoreAnnotation<int>
        {
            public Type getType()
            {
                return typeof (int);
            }
        }

        /**
   * Inverse document frequency of the word this label represents
   */

        public class IDFAnnotation : CoreAnnotation<Double>
        {
            public Type getType()
            {
                return typeof (Double);
            }
        }

        /**
   * Keys from AbstractMapLabel (descriptions taken from that class)
   */
        /**
   * The standard key for storing a projected category in the map, as a String.
   * For any word (leaf node), the projected category is the syntactic category
   * of the maximal constituent headed by the word. Used in SemanticGraph.
   */

        public class ProjectedCategoryAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * The standard key for a propbank label which is of type Argument
   */

        public class ArgumentAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * Another key used for propbank - to signify core arg nodes or predicate
   * nodes
   */

        public class MarkingAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * The standard key for Semantic Head Word which is a String
   */

        public class SemanticHeadWordAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * The standard key for Semantic Head Word POS which is a String
   */

        public class SemanticHeadTagAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * Probank key for the Verb sense given in the Propbank Annotation, should
   * only be in the verbnode
   */

        public class VerbSenseAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * The standard key for storing category with functional tags.
   */

        public class CategoryFunctionalTagAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * This is an NER ID annotation (in case the all caps parsing didn't work out
   * for you...)
   */

        public class NERIDAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * The key for the normalized value of numeric named entities.
   */

        public class NormalizedNamedEntityTagAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public enum SRL_ID
        {
            ARG,
            NO,
            ALL_NO,
            REL
        }

        /**
   * The key for semantic role labels (Note: please add to this description if
   * you use this key)
   */

        public class SRLIDAnnotation : CoreAnnotation<SRL_ID>
        {
            public Type getType()
            {
                return typeof (SRL_ID);
            }
        }

        /**
   * The standard key for the "shape" of a word: a String representing the type
   * of characters in a word, such as "Xx" for a capitalized word. See
   * {@link edu.stanford.nlp.process.WordShapeClassifier} for functions for
   * making shape strings.
   */

        public class ShapeAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * The Standard key for storing the left terminal number relative to the root
   * of the tree of the leftmost terminal dominated by the current node
   */

        public class LeftTermAnnotation : CoreAnnotation<int>
        {
            public Type getType()
            {
                return typeof (int);
            }
        }

        /**
   * The standard key for the parent which is a String
   */

        public class ParentAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class INAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * The standard key for span which is an IntPair
   */

        public class SpanAnnotation : CoreAnnotation<IntPair>
        {
            public Type getType()
            {
                return typeof (IntPair);
            }
        }

        /**
   * The standard key for the answer which is a String
   */

        public class AnswerAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * The standard key for gold answer which is a String
   */

        public class GoldAnswerAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * The standard key for the features which is a Collection
   */

        public class FeaturesAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * The standard key for the semantic interpretation
   */

        public class InterpretationAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * The standard key for the semantic role label of a phrase.
   */

        public class RoleAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * The standard key for the gazetteer information
   */

        public class GazetteerAnnotation : CoreAnnotation<List<String>>
        {
            public Type getType()
            {
                return typeof (List<String>);
            }
        }

        /**
   * Morphological stem of the word this label represents
   */

        public class StemAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class PolarityAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class MorphoNumAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class MorphoPersAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class MorphoGenAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class MorphoCaseAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * for Chinese: character level information, segmentation
   */

        public class ChineseCharAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class ChineseOrigSegAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class ChineseSegAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * Not sure exactly what this is, but it is different from
   * ChineseSegAnnotation and seems to indicate if the text is segmented
   */

        public class ChineseIsSegmentedAnnotation : CoreAnnotation<Boolean>
        {
            public Type getType()
            {
                return typeof (Boolean);
            }
        }

        /**
   * The CoreMap key identifying the offset of the first character of an
   * annotation. The character with index 0 is the first character in the
   * document.
   *
   * This key should be set for any annotation that represents a span of text.
   */

        public class CharacterOffsetBeginAnnotation : CoreAnnotation<int>
        {
            public Type getType()
            {
                return typeof (int);
            }
        }

        /**
   * The CoreMap key identifying the offset of the last character after the end
   * of an annotation. The character with index 0 is the first character in the
   * document.
   *
   * This key should be set for any annotation that represents a span of text.
   */

        public class CharacterOffsetEndAnnotation : CoreAnnotation<int>
        {
            public Type getType()
            {
                return typeof (int);
            }
        }

        /**
   * Key for relative value of a word - used in RTE
   */

        public class CostMagnificationAnnotation : CoreAnnotation<Double>
        {
            public Type getType()
            {
                return typeof (Double);
            }
        }

        public class WordSenseAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /*public class SRLInstancesAnnotation : CoreAnnotation<List<List<Pair<String, Pair>>>> {
    public Type getType() {
      return ErasureUtils.uncheckedCast(List.class);
    }
  }*/

        /**
   * Used by RTE to track number of text sentences, to determine when hyp
   * sentences begin.
   */

        public class NumTxtSentencesAnnotation : CoreAnnotation<int>
        {
            public Type getType()
            {
                return typeof (int);
            }
        }

        /**
   * Used in Trees
   */

        public class TagLabelAnnotation : CoreAnnotation<Label>
        {
            public Type getType()
            {
                return typeof (Label);
            }
        }

        /**
   * Used in CRFClassifier stuff PositionAnnotation should possibly be an int -
   * it's present as either an int or string depending on context CharAnnotation
   * may be "CharacterAnnotation" - not sure
   */

        public class DomainAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class PositionAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class CharAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        // note: this is not a catchall "unknown" annotation but seems to have a
        // specific meaning for sequence classifiers
        public class UnknownAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class IDAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        // possibly this should be grouped with gazetteer annotation - original key
        // was "gaz"
        public class GazAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class PossibleAnswersAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class DistSimAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class AbbrAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class ChunkAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class GovernorAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class AbgeneAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class GeniaAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class AbstrAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class FreqAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class DictAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class WebAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class FemaleGazAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class MaleGazAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class LastGazAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * it really seems like this should have a different name or else be a boolean
   */

        public class IsURLAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class LinkAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class MentionsAnnotation : CoreAnnotation<List<CoreMap>>
        {
            public Type getType()
            {
                return typeof (List<CoreMap>);
            }
        }

        public class EntityTypeAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * it really seems like this should have a different name or else be a boolean
   */

        public class IsDateRangeAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class PredictedAnswerAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /** Seems like this could be consolidated with something else... */

        public class OriginalAnswerAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /** Seems like this could be consolidated with something else... */

        public class OriginalCharAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class UTypeAnnotation : CoreAnnotation<int>
        {
            public Type getType()
            {
                return typeof (int);
            }
        }

        public class EntityRuleAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * Section of a document
   */

        public class SectionAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * Date for a section of a document
   */

        public class SectionDateAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * Id for a section of a document
   */

        public class SectionIDAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * Indicates that the token starts a new section and the attributes
   *   that should go into that section
   */

        public class SectionStartAnnotation : CoreAnnotation<CoreMap>
        {
            public Type getType()
            {
                return typeof (CoreMap);
            }
        }

        /**
   * Indicates that the token end a section and the label of the section
   */

        public class SectionEndAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class WordPositionAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class ParaPositionAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class SentencePositionAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        // Why do both this and sentenceposannotation exist? I don't know, but one
        // class
        // uses both so here they remain for now...
        public class SentenceIDAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class EntityClassAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class AnswerObjectAnnotation : CoreAnnotation<Object>
        {
            public Type getType()
            {
                return typeof (Object);
            }
        }

        /**
   * Used in Task3 Pascal system
   */

        public class BestCliquesAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class BestFullAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class LastTaggedAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * Used in wsd.supwsd package
   */

        public class LabelAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /*public class NeighborsAnnotation : CoreAnnotation<List<Tuple<WordLemmaTag, String>>> {
    public Type getType() {
      return typeof(List<Tuple<WordLemmaTag, String>>);
    }
  }*/

        public class ContextsAnnotation : CoreAnnotation<List<Tuple<String, String>>>
        {
            public Type getType()
            {
                return typeof (List<Tuple<String, String>>);
            }
        }

        public class DependentsAnnotation :
            CoreAnnotation<List<Tuple<Tuple<String, String, String>, String>>>
        {
            public Type getType()
            {
                return typeof (List<Tuple<Tuple<String, String, String>, String>>);
            }
        }

        public class WordFormAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class TrueTagAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class SubcategorizationAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class BagOfWordsAnnotation : CoreAnnotation<List<Tuple<String, String>>>
        {
            public Type getType()
            {
                return typeof (List<Tuple<String, String>>);
            }
        }

        /**
   * Used in srl.unsup
   */

        public class HeightAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class LengthAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * Used in Gale2007ChineseSegmenter
   */

        public class LBeginAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class LMiddleAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class LEndAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class D2_LBeginAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class D2_LMiddleAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class D2_LEndAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class UBlockAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /** Used in Chinese segmenters for whether there was space before a character. */

        public class SpaceBeforeAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * Used in parser.discrim
   */

        /**
   * The base version of the parser state, like NP or VBZ or ...
   */

        public class StateAnnotation : CoreAnnotation<CoreLabel>
        {
            public Type getType()
            {
                return typeof (CoreLabel);
            }
        }

        /**
   * used in binarized trees to say the name of the most recent child
   */

        public class PrevChildAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * used in binarized trees to specify the first child in the rule for which
   * this node is the parent
   */

        public class FirstChildAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * whether the node is the parent in a unary rule
   */

        public class UnaryAnnotation : CoreAnnotation<Boolean>
        {
            public Type getType()
            {
                return typeof (Boolean);
            }
        }

        /**
   * annotation stolen from the lex parser
   */

        public class DoAnnotation : CoreAnnotation<Boolean>
        {
            public Type getType()
            {
                return typeof (Boolean);
            }
        }

        /**
   * annotation stolen from the lex parser
   */

        public class HaveAnnotation : CoreAnnotation<Boolean>
        {
            public Type getType()
            {
                return typeof (Boolean);
            }
        }

        /**
   * annotation stolen from the lex parser
   */

        public class BeAnnotation : CoreAnnotation<Boolean>
        {
            public Type getType()
            {
                return typeof (Boolean);
            }
        }

        /**
   * annotation stolen from the lex parser
   */

        public class NotAnnotation : CoreAnnotation<Boolean>
        {
            public Type getType()
            {
                return typeof (Boolean);
            }
        }

        /**
   * annotation stolen from the lex parser
   */

        public class PercentAnnotation : CoreAnnotation<Boolean>
        {
            public Type getType()
            {
                return typeof (Boolean);
            }
        }

        /**
   * specifies the base state of the parent of this node in the parse tree
   */

        public class GrandparentAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * The key for storing a Head word as a string rather than a pointer (as in
   * TreeCoreAnnotations.HeadWordAnnotation)
   */

        public class HeadWordStringAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * Used in nlp.coref
   */

        public class MonthAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class DayAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class YearAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * Used in propbank.srl
   */

        public class PriorAnnotation : CoreAnnotation<Dictionary<String, Double>>
        {
            public Type getType()
            {
                return typeof (Dictionary<String, Double>);
            }
        }

        public class SemanticWordAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class SemanticTagAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class CovertIDAnnotation : CoreAnnotation<List<IntPair>>
        {
            public Type getType()
            {
                return typeof (List<IntPair>);
            }
        }

        public class ArgDescendentAnnotation : CoreAnnotation<Tuple<String, Double>>
        {

            public Type getType()
            {
                return typeof (Tuple<String, Double>);
            }
        }

        /**
   * Used in nlp.trees. When nodes are duplicated in Stanford Dependencies
   * conversion (to represent conjunction of PPs with preposition collapsing,
   * this gets set to a positive number on duplicated nodes.
   */

        public class CopyAnnotation : CoreAnnotation<int>
        {
            public Type getType()
            {
                return typeof (int);
            }
        }

        /**
   * Used in SimpleXMLAnnotator. The value is an XML element name String for the
   * innermost element in which this token was contained.
   */

        public class XmlElementAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * Used in CleanXMLAnnotator.  The value is a list of XML element names indicating
   * the XML tag the token was nested inside.
   */

        public class XmlContextAnnotation : CoreAnnotation<List<String>>
        {

            public Type getType()
            {
                return typeof (List<String>);
            }
        }

        /**
   *
   * Used for Topic Assignments from LDA or its equivalent models. The value is
   * the topic ID assigned to the current token.
   *
   */

        public class TopicAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        // gets the synonymn of a word in the Wordnet (use a bit differently in sonalg's code)
        public class WordnetSynAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        //to get words of the phrase
        public class PhraseWordsTagAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        //to get pos tag of the phrase i.e. root of the phrase tree in the parse tree
        public class PhraseWordsAnnotation : CoreAnnotation<List<String>>
        {
            public Type getType()
            {
                return typeof (List<String>);
            }
        }

        //to get prototype feature, see Haghighi Exemplar driven learning
        public class ProtoAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        //which common words list does this word belong to
        public class CommonWordsAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        // Document date
        // Needed by SUTime
        public class DocDateAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * Document type
   * What kind of document is it: story, multi-part article, listing, email, etc
   */

        public class DocTypeAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * Document source type
   * What kind of place did the document come from: newswire, discussion forum, web...
   */

        public class DocSourceTypeAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * Document title
   * What is the document title
   */

        public class DocTitleAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * Reference location for the document
   */

        public class LocationAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * Author for the document
   * (really should be a set of authors, but just have single string for simplicity)
   */

        public class AuthorAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        // Numeric annotations

        // Per token annotation indicating whether the token represents a NUMBER or ORDINAL
        // (twenty first => NUMBER ORDINAL)
        public class NumericTypeAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        // Per token annotation indicating the numeric value of the token
        // (twenty first => 20 1)
        public class NumericValueAnnotation : CoreAnnotation<Double>
        {
            public Type getType()
            {
                return typeof (Double);
            }
        }

        // Per token annotation indicating the numeric object associated with an annotation
        public class NumericObjectAnnotation : CoreAnnotation<Object>
        {
            public Type getType()
            {
                return typeof (Object);
            }
        }

        // Annotation indicating whether the numeric phrase the token is part of
        // represents a NUMBER or ORDINAL (twenty first => ORDINAL ORDINAL)
        public class NumericCompositeValueAnnotation : CoreAnnotation<Double>
        {
            public Type getType()
            {
                return typeof (Double);
            }
        }

        // Annotation indicating the numeric value of the phrase the token is part of
        // (twenty first => 21 21 )
        public class NumericCompositeTypeAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        // Annotation indicating the numeric object associated with an annotation
        public class NumericCompositeObjectAnnotation : CoreAnnotation<Object>
        {
            public Type getType()
            {
                return typeof (object);
            }
        }

        public class NumerizedTokensAnnotation : CoreAnnotation<List<CoreMap>>
        {
            public Type getType()
            {
                return typeof (List<CoreMap>);
            }
        }

        /**
   * used in dcoref.
   * to indicate that the it should use the discourse information annotated in the document
   */

        public class UseMarkedDiscourseAnnotation : CoreAnnotation<Boolean>
        {
            public Type getType()
            {
                return typeof (Boolean);
            }
        }

        /**
   * used in dcoref.
   * to store discourse information. (marking <TURN> or quotation)
   */

        public class UtteranceAnnotation : CoreAnnotation<int>
        {
            public Type getType()
            {
                return typeof (int);
            }
        }

        /**
   * used in dcoref.
   * to store speaker information.
   */

        public class SpeakerAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        /**
   * used in dcoref.
   * to store paragraph information.
   */

        public class ParagraphAnnotation : CoreAnnotation<int>
        {
            public Type getType()
            {
                return typeof (int);
            }
        }

        /**
   * used in dcoref.
   * to store premarked entity mentions.
   */
        /*public class MentionTokenAnnotation : CoreAnnotation<MultiTokenTag> {
    public Type getType() {
      return typeof(MultiTokenTag);
    }
  }*/

        /**
   * used in incremental DAG parser
   */

        public class LeftChildrenNodeAnnotation : CoreAnnotation<SortedSet<Tuple<CoreLabel, String>>>
        {
            public Type getType()
            {
                return typeof (SortedSet<Tuple<CoreLabel, String>>);
            }
        }


        /**
   * The CoreMap key identifying the annotation's antecedent.
   *
   * The intent of this annotation is to go with words that have been
   * linked via coref to some other entity.  For example, if "dog" is
   * corefed to "cirrus" in the sentence "Cirrus, a small dog, ate an
   * entire pumpkin pie", then "dog" would have the
   * AntecedentAnnotation "cirrus".
   *
   * This annotation is currently used ONLY in the KBP slot filling project.
   * In that project, "cirrus" from the example above would also have an
   * AntecedentAnnotation of "cirrus".
   * Generally, you want to use the usual coref graph annotations
   */

        public class AntecedentAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }

        public class LabelWeightAnnotation : CoreAnnotation<Double>
        {
            public Type getType()
            {
                return typeof (Double);
            }
        }

        public class ColumnDataClassifierAnnotation : CoreAnnotation<String>
        {
            public Type getType()
            {
                return typeof (String);
            }
        }
    }
}