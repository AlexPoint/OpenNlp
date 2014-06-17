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
            // read file
            var currentDirectory = Environment.CurrentDirectory + "/../../";
            var tokenizerTrainingFilePath = currentDirectory + "Input/tokenizer.train";
            var outputFilePath = currentDirectory + "Output/EnglishTok.nbin";
            
            MaximumEntropyTokenizer.Train(tokenizerTrainingFilePath, outputFilePath);

            Console.WriteLine("OK");
            Console.ReadKey();
        }
    }
}
