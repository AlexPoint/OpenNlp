# OpenNlp

OpenNlp is an open source library for Natural Language Processing (NLP). 
It provides a number of NLP tools in C#:
* sentence splitter
* tokenizer
* part-of-speech tagger
* chunker
* coreference
* name entity recognition
* parse trees

This project started as a C# port of the Java OpenNLP tools (initial code was retrieved on http://sharpnlp.codeplex.com). It was moved to Github to improve the code (add new features and fix detected bugs) and create a nuget package.

You can install this library via nuget: 
>Install-Package OpenNp 

For use with .net Core applications, the System.Runtime.Caching nuget package is also required for full functionality:
>Install-Package System.Runtime.Caching

## Quick start

To test easily the various NLP tools, run the ToolsExample winform project.
You'll find below a more detailed description of the tools and how code snippets to use them directly in your code.
All NLP tools based on the maxent algorithm need model files to run. You'll find those files for English in Resources/Models. If you want to train your own models (to improve precision on English or to use those tools on other languages), please refer to the last section.

### Sentence splitter
A sentence splitter splits a paragraph in sentences. 
Technically, the sentence detector will compute the likelihood that a specific character ('.', '?' or '!' in the case of English) marks the end of a sentence.

```csharp
var paragraph = "Mr. & Mrs. Smith is a 2005 American romantic comedy action film. The film stars Brad Pitt and Angelina Jolie as a bored upper-middle class married couple. They are surprised to learn that they are both assassins hired by competing agencies to kill each other.";
var modelPath = "path/to/EnglishSD.nbin";
var sentenceDetector = EnglishMaximumEntropySentenceDetector(modelPath);
var sentences = sentenceDetector.SentenceDetect(paragraph);
/* 
 * sentences = ["Mr. & Mrs. Smith is a 2005 American romantic comedy action film.", 
 * "The film stars Brad Pitt and Angelina Jolie as a bored upper-middle class married couple.", 
 * "They are surprised to learn that they are both assassins hired by competing agencies to kill each other."]
 */
```

### Tokenizer
A tokenizer breaks a text into words, symbols or other meaningful elements.
The historical tokenizers are based on the maxent algorithm.

```csharp
// Regular tokenizer
var modelPath = "path/to/EnglishTok.nbin";
var sentence = "- Sorry Mrs. Hudson, I'll skip the tea.";
var tokenizer = new EnglishMaximumEntropyTokenizer(modelPath);
var tokens = tokenizer.Tokenize(sentence);
// tokens = ["-", "Sorry", "Mrs.", "Hudson", ",", "I", "'ll", "skip", "the", "tea", "."]
```

For English, a specific rule-based tokenizer (based on regexes) was created and has a better precision. This tokenizer doesn't need any model.
```csharp
// English tokenizer
var tokenizer = new EnglishRuleBasedTokenizer();
var sentence = "- Sorry Mrs. Hudson, I'll skip the tea.";
var tokens = tokenizer.Tokenize(sentence);
// tokens = ["-", "Sorry", "Mrs.", "Hudson", ",", "I", "'ll", "skip", "the", "tea", "."]
```


### Part-of-speech tagger
A part of speech tagger assigns a part of speech (noun, verb etc.) to each token in a sentence.

```csharp
var modelPath = "path/to/EnglishPOS.nbin";
var tagDictDir = "path/to/tagdict/directory";
var posTagger = EnglishMaximumEntropyPosTagger(modelPath, tagdictDir);
var tokens = ["-", "Sorry", "Mrs.", "Hudson", ",", "I", "'ll", "skip", "the", "tea", "."];
var pos = posTagger.Tag(tokens);
// pos = [":", "NNP", "NNP", "NNP", ".", "PRP", "MD", "VB", "DT", "NN", "."]
```
For the full list of part of speech abbreviations, please refer to the [Penn Treebank Project](https://www.ling.upenn.edu/courses/Fall_2003/ling001/penn_treebank_pos.html)

### Chunker
A chunker is an alternative to a full sentence parser which gives the partial syntactic structure of a sentence (for instance the noun/verg groups).
```csharp
var modelPath = "path/to/EnglishChunk.nbin";
var chunker = EnglishTreebankChunker(modelPath);
var tokens = ["-", "Sorry", "Mrs.", "Hudson", ",", "I", "'ll", "skip", "the", "tea", "."];
var pos = [":", "NNP", "NNP", "NNP", ".", "PRP", "MD", "VB", "DT", "NN", "."];
var chunks = chunker.GetChunks(tokens, tags);
// chunks = [["NP", "- Sorry Mrs. Hudson"], [",", ","], ["NP", "I"], ["VP", "'ll skip"], ["NP", "the tea"], [".", "."]]
```

### Coreference
Coference detects all expressions that refer to the same entities in a text.

```csharp
var modelPath = "path/to/coref/dir";
var coreferenceFinder = new TreebankLinker(modelPath);
var sentences = ["Mr. & Mrs. Smith is a 2005 American romantic comedy action film.", 
	"The film stars Brad Pitt and Angelina Jolie as a bored upper-middle class married couple.", 
	"They are surprised to learn that they are both assassins hired by competing agencies to kill each other."];
var coref = coreferenceFinder.GetCoreferenceParse(sentences);
// coref = 
```

### Name entity recognition
Name entity recognition identifies specific entities in sentences. With the current models, you can detect persons, dates, locations, money, percentages and time

```csharp
var modelPath = "path/to/namefind/dir";
var nameFinder = new EnglishNameFinder(modelPath);
var sentence = "Mr. & Mrs. Smith is a 2005 American romantic comedy action film.";
// specify which types of entities you want to detect
var models = ["date", "location", "money", "organization", "percentage", "person", "time"];
var ner = nameFinder.GetNames(models, sentence);
// ner = Mr. & Mrs. <person>Smith</person> is a <date>2005</date> American romantic comedy action film.
```

### Parse tree
A parser gives the full syntactic structure of a sentence.

```csharp
var modelPath = "path/to/models/dir";
var sentence = "- Sorry Mrs Hudson, I'll skiip the tea.";
var parser = new EnglishTreebankParser(_modelPath);
var parse = parser.DoParse(sentence);
// parse = (TOP (S (NP (: -) (NNP Sorry) (NNP Mrs.) (NNP Hudson)) (, ,) (NP (PRP I)) (VP (MD 'll) (VP (VB skip) (NP (DT the) (NN tea)))) (. .)))
```

## Train your models
The models proposed are general models for English. If you need those tools on other languages or on a specialized English corpus, you can train your own models.
To do so, you'll need examples; for instance for sentence detections, you'll need a (big) number of paragraphs with the sentences appropriately delimited.

```csharp
// The file with the training samples; works also with an array of files
var trainingFile  = "path/to/training/file";
// The number of iterations; no general rule for finding the best value, just try several!
var iterations = 5;
// The cut; no general rule for finding the best value, just try several!
var cut = 2;
// The characters which can mark an end of sentence
var endOfSentenceScanner = new CharactersSpecificEndOfSentenceScanner('.', '?', '!', '"', '-', '…');
// Train the model (can take some time depending on your training file size)
var model = MaximumEntropySentenceDetector.TrainModel(trainingFile, iterations, cut, endOfSentenceScanner);
// Persist the model to use it later
var outputFilePath = "path/to/persisted/model";
new BinaryGisModelWriter().Persist(bestModel, outputFilePath);
```
