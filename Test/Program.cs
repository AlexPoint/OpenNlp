using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var binFilePath = currentDirectory + "../Resources/Models/EnglishSD.nbin";
            var reader = new BinaryGisModelReader(binFilePath);
            var model = new GisModel(reader);

            var outputFilePath = currentDirectory + "/Output/EnglishSDClear.txt";
            var writer = new PlainTextGisModelWriter();
            writer.Persist(model, outputFilePath);

            Console.WriteLine("OK");
            Console.ReadKey();
        }
    }
}
