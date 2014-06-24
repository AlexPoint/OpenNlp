using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Chunker;
using OpenNLP.Tools.PosTagger;
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

            /*var input = "She didn't have a laptop, which means she did her business on her phone.";

            var tokenizer = new EnglishMaximumEntropyTokenizer(currentDirectory + "../Resources/Models/EnglishTok.nbin");
            var detokienizer = new DictionaryDetokenizer();

            Console.WriteLine("input: {0}", input);
            var tokens = tokenizer.Tokenize(input);
            var output = detokienizer.Detokenize(tokens);
            Console.WriteLine("ouput: {0}", output);*/

            var tokens = new List<string>() {"do", "n't", "commit"};
            var detokenizer = new DictionaryDetokenizer();
            var result = detokenizer.Detokenize(tokens.ToArray());

            Console.WriteLine(result);

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
