#OpenNlp

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

##Quick start

To test easily the various NLP tools, run the ToolsExample winform project.
You'll find below a more detailed description of the tools and how code snippets to use them directly in your code.

###Sentence splitter
A sentence splitter splits a paragraph in sentences. 
Technically, the sentence detector will compute the likelihood that a specific character ('.', '?' or '!' in the case of English) marks the end of a sentence.

```csharp
var paragraph = "Mr. & Mrs. Smith is a 2005 American romantic comedy action film directed by Doug Liman and written by Simon Kinberg. The film stars Brad Pitt and Angelina Jolie as a bored upper-middle class married couple surprised to learn that they are both assassins hired by competing agencies to kill each other.";
var modelPath = "path/to/EnglishSD.nbin";
var sentenceDetector = EnglishMaximumEntropySentenceDetector(modelPath);
var sentences = sentenceDetector.SentenceDetect(paragraph);
// sentences = ["Mr. & Mrs. Smith is a 2005 American romantic comedy action film directed by Doug Liman and written by Simon Kinberg.", "The film stars Brad Pitt and Angelina Jolie as a bored upper-middle class married couple surprised to learn that they are both assassins hired by competing agencies to kill each other."]
```


###Tokenizer
A tokenizer breaks a text into words, symbols or meaningful elements.
The historical tokenizers are based on the maxent algorithm.

```csharp
// Regular tokenizer
var modelPath = "path/to/EnglishTok.nbin";
var sentence = "- Sorry Mrs. Hudson, I'll skip the tea.";
var tokenizer = new EnglishMaximumEntropyTokenizer(modelPath);
var tokens = tokenizer.Tokenize(sentence);
```

For English, a specific rule-based tokenizer (based on regexes) was created and has a better precision.
```csharp
// English tokenizer
var tokenizer = new EnglishRuleBasedTokenizer();
var sentence = "- Sorry Mrs. Hudson, I'll skip the tea.";
var tokens = tokenizer.Tokenize(sentence);
```


###Part-of-speech tagger


###Chunker


###Coreference


###Name entity recognition


###Parse tree
