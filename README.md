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

###Sentence splitter


###Tokenizer
A tokenizer breaks a text into words, symbols or meaningful elements.
The historical tokenizers are based on the maxent alogirthm.
For English, a specific rule-based tokenizer (based on regexes) was created and has a better precision.

```csharp
// Regular tokenizer
var modelPath = "path/to/EnglishTok.nbin";
var sentence = "- Sorry Mrs. Hudson, I'll skip the tea.";
var tokenizer = new EnglishMaximumEntropyTokenizer(modelPath);
var tokens = tokenizer.Tokenize(sentence);
```
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
