using System;
using System.IO;
using SharpEntropy;
using SharpEntropy.IO;

namespace ModelConverter
{
	/// <summary>
	/// Summary description for Converter.
	/// </summary>
	class Converter
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			if (args.Length != 1)
			{
				Console.WriteLine("You need to specify 1 argument - the path of the folder to convert.");
			}
			else
			{
				string modelPath = args[0];

				if (ConvertFolder(modelPath))
				{
					Console.WriteLine("conversion complete");
				}
				else
				{
					Console.WriteLine("conversion failed");
				}
			}
			Console.ReadLine();
		}

		private static bool ConvertFolder(string folder)
		{
			try
			{
				BinaryGisModelWriter writer = new BinaryGisModelWriter();

				foreach (string file in Directory.GetFiles(folder))
				{
					if (file.Substring(file.Length - 4, 4) == ".bin")
					{
						Console.Write("converting " + file + " ...");
						writer.Persist(new GisModel(new JavaBinaryGisModelReader(file)), file.Replace(".bin", ".nbin"));
						Console.WriteLine("done");
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception occurred: " + ex.Message);
				return false;
			}

			foreach (string childFolder in Directory.GetDirectories(folder))
			{
				if (!ConvertFolder(childFolder))
				{
					return false;
				}
			}

			return true;
		}
	}
}
