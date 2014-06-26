using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.SentenceDetect;
using SharpEntropy.IO;

namespace Trainer
{
    class Program
    {
        private static readonly string currentDirectory = Environment.CurrentDirectory + "/../../";

        static void Main(string[] args)
        {
            // all directories in Input folder
            var inputFolderPath = currentDirectory + "Input/";
            var allDirectories = Directory.GetDirectories(inputFolderPath);
            Console.WriteLine("Pick the model to train:");
            for (var i = 0; i < allDirectories.Length; i++)
            {
                Console.WriteLine("{0} - {1}", i, Path.GetFileName(allDirectories[i]));
            }
            var input = Console.ReadKey();
            int directoryIndexPicked;
            
            var isNumber = int.TryParse(input.KeyChar.ToString(), out directoryIndexPicked);

            if (isNumber && directoryIndexPicked < allDirectories.Length)
            {
                Console.WriteLine();
                // train model file
                var directory = allDirectories[directoryIndexPicked];
                var allTrainFiles = Directory.GetFiles(directory, "*.train");

                var outputFilePath = currentDirectory + "Output/" + Path.GetFileName(directory) + ".nbin";
                var iterations = 100;
                var cut = 5;
                var endOfSentenceScanner = new CharactersSpecificEndOfSentenceScanner('.','?','!','"','-');
                Console.WriteLine("Training model...");
                var model = MaximumEntropySentenceDetector.TrainModel(allTrainFiles, iterations, cut,
                    endOfSentenceScanner);
                Console.WriteLine("Writing output file to '{0}'...", outputFilePath);
                new BinaryGisModelWriter().Persist(model, outputFilePath);
                Console.WriteLine("Output file written.");

                Console.WriteLine("OK");
            }
            else
            {
                Console.WriteLine("Wrong input...");
            }

            Console.ReadKey();
        }
    }
}
