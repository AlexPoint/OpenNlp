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
        static void Main(string[] args)
        {
            var currentDirectory = Environment.CurrentDirectory + "/../../";

            /*// read file
            var tokenizerTrainingFilePath = currentDirectory + "Input/tokenizer.train";
            var outputFilePath = currentDirectory + "Output/EnglishTok.nbin";
            
            MaximumEntropyTokenizer.Train(tokenizerTrainingFilePath, outputFilePath);*/

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
            var englishPosPath = currentDirectory + "../Resources/Models/EnglishPOS.nbin";
            var tagDictPath = currentDirectory + "../Resources/Models/Parser/tagdict";
            var posTagger = new EnglishMaximumEntropyPosTagger(englishPosPath, tagDictPath);

            foreach (var input in inputs)
            {
                string[] tokens = tokenizer.Tokenize(input);
                string[] tags = posTagger.Tag(tokens);

                var chunksAsString = chunker.GetChunks(tokens, tags);
                
                var chunks = chunksAsString.Split(new[] {'[', ']'})
                    .Select(s => s.Trim(new[] {'[', ']', ' '}))
                    .SelectMany(s => s.Split(' '))
                    .Where(s => !string.IsNullOrEmpty(s) && s.Contains("/"))
                    .Select(s => s.Split('/').First())
                    .ToArray();
                var output = dechunker.Dechunk(chunks);
                Console.WriteLine("input: " + input);
                Console.WriteLine("chunks: "+ string.Join(" | ", chunks));
                Console.WriteLine("ouput: " + output);
                Console.WriteLine("--");
            }

            Console.WriteLine("OK");
            Console.ReadKey();
        }
    }
}
