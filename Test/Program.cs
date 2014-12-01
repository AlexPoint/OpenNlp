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
using SharpEntropy;
using SharpEntropy.IO;

namespace Test
{
    class Program
    {
        private static readonly string currentDirectory = Environment.CurrentDirectory + "/../../";

        static void Main(string[] args)
        {
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

            // invalid email detection
            var inputFilePath = currentDirectory + "Input/invalidEmailDetection.train";
            var outputFilePath = currentDirectory + "Output/" + Path.GetFileNameWithoutExtension(inputFilePath) + ".nbin";
            var iterations = 100;
            var cut = 5;
            Console.WriteLine("Training model...");
            var model = MaximumEntropyInvalidEmailDetector.TrainModel(inputFilePath, iterations, cut);
            Console.WriteLine("Writing output file '{0}'...", outputFilePath);
            new BinaryGisModelWriter().Persist(model, outputFilePath);
            Console.WriteLine("Output file written.");

            // test trained model
            var results = new List<Tuple<string, bool, double>>();
            var invalidEmailDetector = new MaximumEntropyInvalidEmailDetector(outputFilePath);
            var allLines = File.ReadAllLines(inputFilePath);
            foreach (var line in allLines)
            {
                var parts = line.Split('\t');
                var email = parts.First();
                var isInvalid = parts.Last() == "1";
                
                var invalidProbability = invalidEmailDetector.Detect(email);
                //var moreLikelyThanAverageToBeInvalid = invalidProbability*100 > 24.97;
                results.Add(new Tuple<string, bool, double>(email, isInvalid, invalidProbability));
            }

            var nbOfEmailsSent = 0;
            var nbOfEmailsSentWhichWouldBounce = 0;
            foreach (var result in results.OrderBy(tup => tup.Item3))
            {
                if (nbOfEmailsSentWhichWouldBounce < 25)
                {
                    nbOfEmailsSent++;
                    if (result.Item2) { nbOfEmailsSentWhichWouldBounce++; }
                }
                Console.WriteLine("{0} ({1})", result.Item1, result.Item2 ? "INVALID": "OK");
            }

            Console.WriteLine("Email that could have been sent: {0}", nbOfEmailsSent);
            /*var nbOfSamples = results.Count;
            Console.WriteLine("Nb of samples: {0}", nbOfSamples);
            var nbOfCorrectResults = results.Count(tup => tup.Item2 == tup.Item3);
            Console.WriteLine("Nb of correct results: {0}", nbOfCorrectResults);
            var nbOfNotDetected = results.Count(tup => tup.Item2 && !tup.Item3);
            Console.WriteLine("Nb of not detected: {0}", nbOfNotDetected);
            var nbOfFalsePositive = results.Count(tup => !tup.Item2 && tup.Item3);
            Console.WriteLine("Nb of false positive: {0}", nbOfFalsePositive);*/

            Console.WriteLine("OK");
            Console.ReadKey();
        }

        private void TestDechunk()
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
        }
    }
}
