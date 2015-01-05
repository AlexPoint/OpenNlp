using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Chunker;
using OpenNLP.Tools.PosTagger;
using OpenNLP.Tools.SentenceDetect;
using OpenNLP.Tools.Tokenize;
using OpenNLP.Tools.Util.Trees;
using OpenNLP.Tools.Util.Trees.TRegex;
using OpenNLP.Tools.Util.Trees.TRegex.Tsurgeon;
using SharpEntropy;
using SharpEntropy.IO;

namespace Test
{
    class Program
    {
        private static readonly string currentDirectory = Environment.CurrentDirectory + "/../../";

        static void Main(string[] args)
        {
            /*FileStream ostrm;
            StreamWriter writer;
            TextWriter oldOut = Console.Out;
            try
            {
                ostrm = new FileStream("C:\\Users\\Alexandre\\Desktop\\vs_output_2.txt", FileMode.OpenOrCreate, FileAccess.Write);
                writer = new StreamWriter(ostrm);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot open Redirect.txt for writing");
                Console.WriteLine(e.Message);
                return;
            }
            Console.SetOut(writer);*/


            /*// read file
            var tokenizerTrainingFilePath = currentDirectory + "Input/tokenizer.train";
            var outputFilePath = currentDirectory + "Output/EnglishTok.nbin";
            MaximumEntropyTokenizer.Train(tokenizerTrainingFilePath, outputFilePath);*/
            
            // test detokenization
            /*var tokens = new List<string>() {"do", "n't", "commit"};
            var detokenizer = new DictionaryDetokenizer();
            var result = detokenizer.Detokenize(tokens.ToArray());
            Console.WriteLine(result);*/

            /*// train model file
            var inputFilePath = currentDirectory + "Input/sentences.train";
            var outputFilePath = currentDirectory + "Output/" + Path.GetFileNameWithoutExtension(inputFilePath) + ".nbin";
            var iterations = 100;
            var cut = 5;
            var endOfSentenceScanner = new CharactersSpecificEndOfSentenceScanner();
            Console.WriteLine("Training model...");
            var model = MaximumEntropySentenceDetector.TrainModel(inputFilePath, iterations, cut, endOfSentenceScanner);
            Console.WriteLine("Writing output file '{0}'...", outputFilePath);
            new BinaryGisModelWriter().Persist(model, outputFilePath);
            Console.WriteLine("Output file written.");*/

            /*// tokenize tests
            var modelPath = currentDirectory + "../Resources/Models/";
            var tokenizer = new EnglishMaximumEntropyTokenizer(modelPath + "EnglishTok.nbin");

            var input = "It was built of a bright brick throughout; its skyline was fantastic, and even its ground plan was wild.";
            var tokens = tokenizer.Tokenize(input);
            Console.WriteLine(string.Join(" | ", tokens));*/


            // detect tokenization issues
            /*var pathToFile = currentDirectory + "Input/tokenizerIssues.txt";
            var modelPath = currentDirectory + "../Resources/Models/";
            var tokenizer = new EnglishMaximumEntropyTokenizer(modelPath + "EnglishTok.nbin");
            var allLines = File.ReadAllLines(pathToFile);
            foreach (var line in allLines)
            {
                var tokens = tokenizer.Tokenize(line);
                Console.WriteLine(string.Join(" | ", tokens));
            }*/

            /*String[] jjstrLiteralImages =
        {
            "", null, null, null, null, "\x74\x56\x56\x56", null, null, null, @"\137\137", null,
            null, @"\174", @"\12", @"\50", @"\51", @"\41", @"\100", @"\43", @"\45", @"\75", @"\176",
            @"\46", @"\77", @"\133", @"\135", @"\173", @"\73", @"\175",
        };
            foreach (var lit in jjstrLiteralImages)
            {
                Console.WriteLine(lit);
            }*/
            
            // parsing
            var sentence = "I looked the word up in the dictionary.";
            var modelPath = currentDirectory + "../Resources/Models/";
			var parser = new OpenNLP.Tools.Parser.EnglishTreebankParser(modelPath, true, false);
            var parse = parser.DoParse(sentence);
            // Extract dependencies from lexical tree
            var tlp = new PennTreebankLanguagePack();
            var gsf = tlp.grammaticalStructureFactory();
            var tree = new ParseTree(parse);
            Console.WriteLine(tree);
            var gs = gsf.newGrammaticalStructure(tree);
            var dependencies = gs.mtypedDependencies();

            foreach (var dep in dependencies)
            {
                Console.WriteLine(dep);
            }


            /*for (var i = 0; i < 256; i++)
            {
                var curChar = (char) i;
                //var res = (curChar & ~077);
                var res1 = 1L << curChar;
                var res2 = (0x100002400L & (1L << curChar)) == 0L;
                Console.WriteLine("{0}({1}) -> {2}|{3}", curChar, i, res1, res2);
            }*/

            /*TsurgeonPattern rearrangeNowThatTsurgeon = Tsurgeon.parseOperation("createSubtree CONJP start end");
            Console.WriteLine(rearrangeNowThatTsurgeon);*/

            /*var pattern = TregexPattern.compile("CONJP < (CC <: /^(?i:but|and)$/ $+ (RB=head <: /^(?i:not)$/))");
            Console.WriteLine(pattern);*/

            /*var charStream = "CONJP < (CC <: /^(?i:but|and)$/ $+ (RB=head <: /^(?i:not)$/))";
            var reader = new StringReader(charStream);
            var stream = new SimpleCharStream(reader);
            Console.WriteLine(charStream);
            var c = stream.readChar();
            while (c != default(char))
            {
                Console.WriteLine(c);
                c = stream.readChar();
            }*/
            
            Console.WriteLine("===========");
            Console.WriteLine("OK");
            Console.ReadKey();
        }
        
        /*private void TestDechunk()
        {
            // detokenize
            var inputs = new string[]
            {
                "- Harry's your sister. - Look, what exactly am I supposed to be doing here?",
                "\"Piss off!\"",
                "- Sorry Mrs. Hudson, I'll skip the tea. Off out. - Both of you?",
                "I love playing half-life; that's just who I am!",
                "That's why I... have just begun to write a book.",
                "And they lived happily ever after...",
                "It's gonna be $1.5 sir."
            };

            // 
            var tokenizer = new EnglishMaximumEntropyTokenizer(currentDirectory + "../Resources/Models/EnglishTok.nbin");
            var chunker = new EnglishTreebankChunker(currentDirectory + "../Resources/Models/EnglishChunk.nbin");
            var dechunker = new RegexDictionaryDechunker();
            var detokienizer = new DictionaryDetokenizer();
            var englishPosPath = currentDirectory + "../Resources/Models/EnglishPOS.nbin";
            var tagDictPath = currentDirectory + "../Resources/Models/Parser/tagdict";
            var posTagger = new EnglishMaximumEntropyPosTagger(englishPosPath, tagDictPath);

            foreach (var input in inputs)
            {
                string[] tokens = tokenizer.Tokenize(input);
                string[] tags = posTagger.Tag(tokens);

                var chunks = chunker.GetChunks(tokens, tags);
                var chunksStrings = chunks
                    .Select(ch => detokienizer.Detokenize(ch.TaggedWords.Select(tw => tw.Word).ToArray()))
                    .ToArray();
                var output = dechunker.Dechunk(chunksStrings);
                Console.WriteLine("input: " + input);
                Console.WriteLine("chunks: " + string.Join(" | ", chunks));
                Console.WriteLine("ouput: " + output);
                Console.WriteLine("--");
            }
        }*/
    }
}
