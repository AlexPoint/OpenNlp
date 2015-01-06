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
 * <code>string</code> serves as the value containing the
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

        /**
   * The CoreMap key identifying the annotation's text.
   *
   * Note that this key is intended to be used with many different kinds of
   * annotations - documents, sentences and tokens all have their own text.
   */

        public class TextAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }


        /**
   * The CoreMap key for getting the lemma (morphological stem) of a token.
   *
   * This key is typically set on token annotations.
   *
   * TODO: merge with StemAnnotation?
   */

        public class LemmaAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * The CoreMap key for getting the Penn part of speech of a token.
   *
   * This key is typically set on token annotations.
   */

        public class PartOfSpeechAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * The CoreMap key for getting the token-level named entity tag (e.g., DATE,
   * PERSON, etc.)
   *
   * This key is typically set on token annotations.
   */

        public class NamedEntityTagAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
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

        public class StackedNamedEntityTagAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * The CoreMap key for getting the token-level true case annotation (e.g.,
   * INIT_UPPER)
   *
   * This key is typically set on token annotations.
   */

        public class TrueCaseAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * The CoreMap key identifying the annotation's true-cased text.
   *
   * Note that this key is intended to be used with many different kinds of
   * annotations - documents, sentences and tokens all have their own text.
   */

        public class TrueCaseTextAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
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
            public Type GetType()
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
            public Type GetType()
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
            public Type GetType()
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
            public Type GetType()
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
            public Type GetType()
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
            public Type GetType()
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
            public Type GetType()
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

        public class DocIDAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
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
            public Type GetType()
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
            public Type GetType()
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
            public Type GetType()
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
            public Type GetType()
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
            public Type GetType()
            {
                return typeof (Boolean);
            }
        }

        /**
   * Unique identifier within a document for a given sentence.
   */

        public class SentenceIndexAnnotation : CoreAnnotation<int>
        {
            public Type GetType()
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
            public Type GetType()
            {
                return typeof (int);
            }
        }

        /**
   * Contains the "value" - an ill-defined string used widely in MapLabel.
   */

        public class ValueAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class CategoryAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * The exact original surface form of a token.  This is created in the
   * invertible PTBTokenizer. The tokenizer may normalize the token form to
   * match what appears in the PTB, but this key will hold the original characters.
   */

        public class OriginalTextAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * Annotation for the whitespace characters appearing before this word. This
   * can be filled in by the tokenizer so that the original text string can be
   * reconstructed.
   */

        public class BeforeAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * Annotation for the whitespace characters appear after this word. This can
   * be filled in by the tokenizer so that the original text string can be
   * reconstructed.
   */

        public class AfterAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * CoNLL dep parsing - coarser POS tags.
   */

        public class CoarseTagAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * CoNLL dep parsing - the dependency type
   */

        public class CoNLLDepAnnotation : CoreAnnotation<CoreMap>
        {
            public Type GetType()
            {
                return typeof (CoreMap);
            }
        }

        /**
   * CoNLL SRL/dep parsing - whether the word is a predicate
   */

        public class CoNLLPredicateAnnotation : CoreAnnotation<Boolean>
        {
            public Type GetType()
            {
                return typeof (Boolean);
            }
        }

        /**
   * CoNLL SRL/dep parsing - map which, for the current word, specifies its
   * specific role for each predicate
   */

        public class CoNLLSRLAnnotation : CoreAnnotation<Dictionary<int, string>>
        {
            public Type GetType()
            {
                return typeof (Dictionary<int, string>);
            }
        }

        /**
   * CoNLL dep parsing - the dependency type
   */

        public class CoNLLDepTypeAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * CoNLL dep parsing - the index of the word which is the parent of this word
   * in the dependency tree
   */

        public class CoNLLDepParentIndexAnnotation : CoreAnnotation<int>
        {
            public Type GetType()
            {
                return typeof (int);
            }
        }

        /**
   * Inverse document frequency of the word this label represents
   */

        public class IDFAnnotation : CoreAnnotation<Double>
        {
            public Type GetType()
            {
                return typeof (Double);
            }
        }

        /**
   * Keys from AbstractMapLabel (descriptions taken from that class)
   */
        /**
   * The standard key for storing a projected category in the map, as a string.
   * For any word (leaf node), the projected category is the syntactic category
   * of the maximal constituent headed by the word. Used in SemanticGraph.
   */

        public class ProjectedCategoryAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * The standard key for a propbank label which is of type Argument
   */

        public class ArgumentAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * Another key used for propbank - to signify core arg nodes or predicate
   * nodes
   */

        public class MarkingAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * The standard key for Semantic Head Word which is a String
   */

        public class SemanticHeadWordAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * The standard key for Semantic Head Word POS which is a String
   */

        public class SemanticHeadTagAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * Probank key for the Verb sense given in the Propbank Annotation, should
   * only be in the verbnode
   */

        public class VerbSenseAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * The standard key for storing category with functional tags.
   */

        public class CategoryFunctionalTagAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * This is an NER ID annotation (in case the all caps parsing didn't work out
   * for you...)
   */

        public class NERIDAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * The key for the normalized value of numeric named entities.
   */

        public class NormalizedNamedEntityTagAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
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
            public Type GetType()
            {
                return typeof (SRL_ID);
            }
        }

        /**
   * The standard key for the "shape" of a word: a string representing the type
   * of characters in a word, such as "Xx" for a capitalized word. See
   * {@link edu.stanford.nlp.process.WordShapeClassifier} for functions for
   * making shape strings.
   */

        public class ShapeAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * The Standard key for storing the left terminal number relative to the root
   * of the tree of the leftmost terminal dominated by the current node
   */

        public class LeftTermAnnotation : CoreAnnotation<int>
        {
            public Type GetType()
            {
                return typeof (int);
            }
        }

        /**
   * The standard key for the parent which is a String
   */

        public class ParentAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class INAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * The standard key for span which is an IntPair
   */

        public class SpanAnnotation : CoreAnnotation<IntPair>
        {
            public Type GetType()
            {
                return typeof (IntPair);
            }
        }

        /**
   * The standard key for the answer which is a String
   */

        public class AnswerAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * The standard key for gold answer which is a String
   */

        public class GoldAnswerAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * The standard key for the features which is a Collection
   */

        public class FeaturesAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * The standard key for the semantic interpretation
   */

        public class InterpretationAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * The standard key for the semantic role label of a phrase.
   */

        public class RoleAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * The standard key for the gazetteer information
   */

        public class GazetteerAnnotation : CoreAnnotation<List<string>>
        {
            public Type GetType()
            {
                return typeof (List<string>);
            }
        }

        /**
   * Morphological stem of the word this label represents
   */

        public class StemAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class PolarityAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class MorphoNumAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class MorphoPersAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class MorphoGenAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class MorphoCaseAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * for Chinese: character level information, segmentation
   */

        public class ChineseCharAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class ChineseOrigSegAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class ChineseSegAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * Not sure exactly what this is, but it is different from
   * ChineseSegAnnotation and seems to indicate if the text is segmented
   */

        public class ChineseIsSegmentedAnnotation : CoreAnnotation<Boolean>
        {
            public Type GetType()
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
            public Type GetType()
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
            public Type GetType()
            {
                return typeof (int);
            }
        }

        /**
   * Key for relative value of a word - used in RTE
   */

        public class CostMagnificationAnnotation : CoreAnnotation<Double>
        {
            public Type GetType()
            {
                return typeof (Double);
            }
        }

        public class WordSenseAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /*public class SRLInstancesAnnotation : CoreAnnotation<List<List<Pair<string, Pair>>>> {
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
            public Type GetType()
            {
                return typeof (int);
            }
        }

        /**
   * Used in Trees
   */

        public class TagLabelAnnotation : CoreAnnotation<Label>
        {
            public Type GetType()
            {
                return typeof (Label);
            }
        }

        /**
   * Used in CRFClassifier stuff PositionAnnotation should possibly be an int -
   * it's present as either an int or string depending on context CharAnnotation
   * may be "CharacterAnnotation" - not sure
   */

        public class DomainAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class PositionAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class CharAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        // note: this is not a catchall "unknown" annotation but seems to have a
        // specific meaning for sequence classifiers
        public class UnknownAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class IDAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        // possibly this should be grouped with gazetteer annotation - original key
        // was "gaz"
        public class GazAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class PossibleAnswersAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class DistSimAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class AbbrAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class ChunkAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class GovernorAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class AbgeneAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class GeniaAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class AbstrAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class FreqAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class DictAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class WebAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class FemaleGazAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class MaleGazAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class LastGazAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * it really seems like this should have a different name or else be a boolean
   */

        public class IsURLAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class LinkAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class MentionsAnnotation : CoreAnnotation<List<CoreMap>>
        {
            public Type GetType()
            {
                return typeof (List<CoreMap>);
            }
        }

        public class EntityTypeAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * it really seems like this should have a different name or else be a boolean
   */

        public class IsDateRangeAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class PredictedAnswerAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /** Seems like this could be consolidated with something else... */

        public class OriginalAnswerAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /** Seems like this could be consolidated with something else... */

        public class OriginalCharAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class UTypeAnnotation : CoreAnnotation<int>
        {
            public Type GetType()
            {
                return typeof (int);
            }
        }

        public class EntityRuleAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * Section of a document
   */

        public class SectionAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * Date for a section of a document
   */

        public class SectionDateAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * Id for a section of a document
   */

        public class SectionIDAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * Indicates that the token starts a new section and the attributes
   *   that should go into that section
   */

        public class SectionStartAnnotation : CoreAnnotation<CoreMap>
        {
            public Type GetType()
            {
                return typeof (CoreMap);
            }
        }

        /**
   * Indicates that the token end a section and the label of the section
   */

        public class SectionEndAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class WordPositionAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class ParaPositionAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class SentencePositionAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        // Why do both this and sentenceposannotation exist? I don't know, but one
        // class
        // uses both so here they remain for now...
        public class SentenceIDAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class EntityClassAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class AnswerObjectAnnotation : CoreAnnotation<Object>
        {
            public Type GetType()
            {
                return typeof (Object);
            }
        }

        /**
   * Used in Task3 Pascal system
   */

        public class BestCliquesAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class BestFullAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class LastTaggedAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * Used in wsd.supwsd package
   */

        public class LabelAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /*public class NeighborsAnnotation : CoreAnnotation<List<Tuple<WordLemmaTag, string>>> {
    public Type getType() {
      return typeof(List<Tuple<WordLemmaTag, string>>);
    }
  }*/

        public class ContextsAnnotation : CoreAnnotation<List<Tuple<string, string>>>
        {
            public Type GetType()
            {
                return typeof (List<Tuple<string, string>>);
            }
        }

        public class DependentsAnnotation :
            CoreAnnotation<List<Tuple<Tuple<string, String, string>, string>>>
        {
            public Type GetType()
            {
                return typeof (List<Tuple<Tuple<string, String, string>, string>>);
            }
        }

        public class WordFormAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class TrueTagAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class SubcategorizationAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class BagOfWordsAnnotation : CoreAnnotation<List<Tuple<string, string>>>
        {
            public Type GetType()
            {
                return typeof (List<Tuple<string, string>>);
            }
        }

        /**
   * Used in srl.unsup
   */

        public class HeightAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class LengthAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * Used in Gale2007ChineseSegmenter
   */

        public class LBeginAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class LMiddleAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class LEndAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class D2_LBeginAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class D2_LMiddleAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class D2_LEndAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class UBlockAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /** Used in Chinese segmenters for whether there was space before a character. */

        public class SpaceBeforeAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
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
            public Type GetType()
            {
                return typeof (CoreLabel);
            }
        }

        /**
   * used in binarized trees to say the name of the most recent child
   */

        public class PrevChildAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * used in binarized trees to specify the first child in the rule for which
   * this node is the parent
   */

        public class FirstChildAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * whether the node is the parent in a unary rule
   */

        public class UnaryAnnotation : CoreAnnotation<Boolean>
        {
            public Type GetType()
            {
                return typeof (Boolean);
            }
        }

        /**
   * annotation stolen from the lex parser
   */

        public class DoAnnotation : CoreAnnotation<Boolean>
        {
            public Type GetType()
            {
                return typeof (Boolean);
            }
        }

        /**
   * annotation stolen from the lex parser
   */

        public class HaveAnnotation : CoreAnnotation<Boolean>
        {
            public Type GetType()
            {
                return typeof (Boolean);
            }
        }

        /**
   * annotation stolen from the lex parser
   */

        public class BeAnnotation : CoreAnnotation<Boolean>
        {
            public Type GetType()
            {
                return typeof (Boolean);
            }
        }

        /**
   * annotation stolen from the lex parser
   */

        public class NotAnnotation : CoreAnnotation<Boolean>
        {
            public Type GetType()
            {
                return typeof (Boolean);
            }
        }

        /**
   * annotation stolen from the lex parser
   */

        public class PercentAnnotation : CoreAnnotation<Boolean>
        {
            public Type GetType()
            {
                return typeof (Boolean);
            }
        }

        /**
   * specifies the base state of the parent of this node in the parse tree
   */

        public class GrandparentAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * The key for storing a Head word as a string rather than a pointer (as in
   * TreeCoreAnnotations.HeadWordAnnotation)
   */

        public class HeadWordStringAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * Used in nlp.coref
   */

        public class MonthAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class DayAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class YearAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * Used in propbank.srl
   */

        public class PriorAnnotation : CoreAnnotation<Dictionary<string, Double>>
        {
            public Type GetType()
            {
                return typeof (Dictionary<string, Double>);
            }
        }

        public class SemanticWordAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class SemanticTagAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class CovertIDAnnotation : CoreAnnotation<List<IntPair>>
        {
            public Type GetType()
            {
                return typeof (List<IntPair>);
            }
        }

        public class ArgDescendentAnnotation : CoreAnnotation<Tuple<string, Double>>
        {

            public Type GetType()
            {
                return typeof (Tuple<string, Double>);
            }
        }

        /**
   * Used in nlp.trees. When nodes are duplicated in Stanford Dependencies
   * conversion (to represent conjunction of PPs with preposition collapsing,
   * this gets set to a positive number on duplicated nodes.
   */

        public class CopyAnnotation : CoreAnnotation<int>
        {
            public Type GetType()
            {
                return typeof (int);
            }
        }

        /**
   * Used in SimpleXMLAnnotator. The value is an XML element name string for the
   * innermost element in which this token was contained.
   */

        public class XmlElementAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * Used in CleanXMLAnnotator.  The value is a list of XML element names indicating
   * the XML tag the token was nested inside.
   */

        public class XmlContextAnnotation : CoreAnnotation<List<string>>
        {

            public Type GetType()
            {
                return typeof (List<string>);
            }
        }

        /**
   *
   * Used for Topic Assignments from LDA or its equivalent models. The value is
   * the topic ID assigned to the current token.
   *
   */

        public class TopicAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        // gets the synonymn of a word in the Wordnet (use a bit differently in sonalg's code)
        public class WordnetSynAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        //to get words of the phrase
        public class PhraseWordsTagAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        //to get pos tag of the phrase i.e. root of the phrase tree in the parse tree
        public class PhraseWordsAnnotation : CoreAnnotation<List<string>>
        {
            public Type GetType()
            {
                return typeof (List<string>);
            }
        }

        //to get prototype feature, see Haghighi Exemplar driven learning
        public class ProtoAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        //which common words list does this word belong to
        public class CommonWordsAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        // Document date
        // Needed by SUTime
        public class DocDateAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * Document type
   * What kind of document is it: story, multi-part article, listing, email, etc
   */

        public class DocTypeAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * Document source type
   * What kind of place did the document come from: newswire, discussion forum, web...
   */

        public class DocSourceTypeAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * Document title
   * What is the document title
   */

        public class DocTitleAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * Reference location for the document
   */

        public class LocationAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * Author for the document
   * (really should be a set of authors, but just have single string for simplicity)
   */

        public class AuthorAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        // Numeric annotations

        // Per token annotation indicating whether the token represents a NUMBER or ORDINAL
        // (twenty first => NUMBER ORDINAL)
        public class NumericTypeAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        // Per token annotation indicating the numeric value of the token
        // (twenty first => 20 1)
        public class NumericValueAnnotation : CoreAnnotation<Double>
        {
            public Type GetType()
            {
                return typeof (Double);
            }
        }

        // Per token annotation indicating the numeric object associated with an annotation
        public class NumericObjectAnnotation : CoreAnnotation<Object>
        {
            public Type GetType()
            {
                return typeof (Object);
            }
        }

        // Annotation indicating whether the numeric phrase the token is part of
        // represents a NUMBER or ORDINAL (twenty first => ORDINAL ORDINAL)
        public class NumericCompositeValueAnnotation : CoreAnnotation<Double>
        {
            public Type GetType()
            {
                return typeof (Double);
            }
        }

        // Annotation indicating the numeric value of the phrase the token is part of
        // (twenty first => 21 21 )
        public class NumericCompositeTypeAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        // Annotation indicating the numeric object associated with an annotation
        public class NumericCompositeObjectAnnotation : CoreAnnotation<Object>
        {
            public Type GetType()
            {
                return typeof (object);
            }
        }

        public class NumerizedTokensAnnotation : CoreAnnotation<List<CoreMap>>
        {
            public Type GetType()
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
            public Type GetType()
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
            public Type GetType()
            {
                return typeof (int);
            }
        }

        /**
   * used in dcoref.
   * to store speaker information.
   */

        public class SpeakerAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        /**
   * used in dcoref.
   * to store paragraph information.
   */

        public class ParagraphAnnotation : CoreAnnotation<int>
        {
            public Type GetType()
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

        public class LeftChildrenNodeAnnotation : CoreAnnotation<SortedSet<Tuple<CoreLabel, string>>>
        {
            public Type GetType()
            {
                return typeof (SortedSet<Tuple<CoreLabel, string>>);
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

        public class AntecedentAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }

        public class LabelWeightAnnotation : CoreAnnotation<Double>
        {
            public Type GetType()
            {
                return typeof (Double);
            }
        }

        public class ColumnDataClassifierAnnotation : CoreAnnotation<string>
        {
            public Type GetType()
            {
                return typeof (string);
            }
        }
    }
}