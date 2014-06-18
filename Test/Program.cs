using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            var tokenizer = new EnglishMaximumEntropyTokenizer(currentDirectory + "../Resources/Models/EnglishTok.nbin");
            var detokenizer = new DictionaryDetokenizer();

            foreach (var input in inputs)
            {
                var tokens = tokenizer.Tokenize(input);
                var output = detokenizer.Detokenize(tokens, "");
                Console.WriteLine("input: " + input);
                Console.WriteLine("tokens: "+ string.Join(", ", tokens));
                Console.WriteLine("ouput: " + output);
            }

            Console.WriteLine("OK");
            Console.ReadKey();
        }
    }
}
