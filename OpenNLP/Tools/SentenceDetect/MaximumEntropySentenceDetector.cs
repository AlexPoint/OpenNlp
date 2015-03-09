//Copyright (C) 2005 Richard J. Northedge
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

//This file is based on the SentenceDetectorME.java source file found in the
//original java implementation of OpenNLP.  That source file contains the following header:

// Copyright (C) 2004 Jason Baldridge, Gann Bierner and Tom Morton
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SharpEntropy;

namespace OpenNLP.Tools.SentenceDetect
{
	/// <summary>
	/// A sentence detector for splitting up raw text into sentences.  A maximum
	/// entropy model is used to evaluate the characters ".", "!", and "?" in a
	/// string to determine if they signify the end of a sentence.
	/// </summary>
	public class MaximumEntropySentenceDetector : ISentenceDetector
	{
        // Properties ---------------

		/// <summary>
		/// The maximum entropy model to use to evaluate contexts.
		/// </summary>
		private readonly IMaximumEntropyModel _model;
		
		/// <summary>
		/// The feature context generator.
		/// </summary>
        private readonly IContextGenerator<Tuple<StringBuilder, int>> _contextGenerator;
		
		/// <summary>
		/// The EndOfSentenceScanner to use when scanning for end of
		/// sentence offsets.
		/// </summary>
		private readonly IEndOfSentenceScanner _scanner;
		
        
        // Constructors ------------------

		/// <summary>
		/// Constructor which takes a IMaximumEntropyModel and calls the three-arg
		/// constructor with that model, a SentenceDetectionContextGenerator, and the
		/// default end of sentence scanner.
		/// </summary>
		/// <param name="model">
		/// The MaxentModel which this SentenceDetectorME will use to
		/// evaluate end-of-sentence decisions.
		/// </param>
		public MaximumEntropySentenceDetector(IMaximumEntropyModel model):
            this(model, new DefaultEndOfSentenceScanner()){}
		
		public MaximumEntropySentenceDetector(IMaximumEntropyModel model, IEndOfSentenceScanner scanner):
            this(model, new SentenceDetectionContextGenerator(scanner.GetPotentialEndOfSentenceCharacters().ToArray()), scanner){ }
		
		/// <summary>
		/// Constructor which takes a IMaximumEntropyModel and a IContextGenerator.
		/// calls the three-arg constructor with a default ed of sentence scanner.
		/// </summary>
		/// <param name="model">
		/// The MaxentModel which this SentenceDetectorME will use to
		/// evaluate end-of-sentence decisions.
		/// </param>
		/// <param name="contextGenerator">
		/// The IContextGenerator object which this MaximumEntropySentenceDetector
		/// will use to turn strings into contexts for the model to
		/// evaluate.
		/// </param>
        public MaximumEntropySentenceDetector(IMaximumEntropyModel model, IContextGenerator<Tuple<StringBuilder, int>> contextGenerator):
            this(model, contextGenerator, new DefaultEndOfSentenceScanner()){}
		
		/// <summary> 
		/// Creates a new <code>MaximumEntropySentenceDetector</code> instance.
		/// </summary>
		/// <param name="model">
		/// The IMaximumEntropyModel which this MaximumEntropySentenceDetector will use to
		/// evaluate end-of-sentence decisions.
		/// </param>
		/// <param name="contextGenerator">The IContextGenerator object which this MaximumEntropySentenceDetector
		/// will use to turn strings into contexts for the model to
		/// evaluate.
		/// </param>
		/// <param name="scanner">the EndOfSentenceScanner which this MaximumEntropySentenceDetector
		/// will use to locate end of sentence indexes.
		/// </param>
        public MaximumEntropySentenceDetector(IMaximumEntropyModel model, IContextGenerator<Tuple<StringBuilder, int>> contextGenerator, IEndOfSentenceScanner scanner)
		{
			_model = model;
			_contextGenerator = contextGenerator;
			_scanner = scanner;
		}
		
		/// <summary> 
		/// Detect sentences in a string.
		/// </summary>
		/// <param name="input">
		/// The string to be processed.
		/// </param>
		/// <returns>   
		/// A string array containing individual sentences as elements.
		/// </returns>
		public virtual string[] SentenceDetect(string input)
		{
			int[] startsList = SentencePositionDetect(input);
			if (startsList.Length == 0) 
			{
				return new string[] {input};
			}
			
			bool isLeftover = startsList[startsList.Length - 1] != input.Length;
			var sentences = new string[isLeftover ? startsList.Length + 1 : startsList.Length];

			sentences[0] = input.Substring(0, (startsList[0]) - (0)).Trim();
			for (int currentStart = 1; currentStart < startsList.Length; currentStart++)
			{
				sentences[currentStart] = input.Substring(startsList[currentStart - 1], (startsList[currentStart]) - (startsList[currentStart - 1])).Trim();
			}
			
			if (isLeftover) 
			{
                sentences[sentences.Length - 1] = input.Substring(startsList[startsList.Length - 1]).Trim();
			}

			return sentences;
		}
		
		private int GetFirstWhitespace(string input, int position) 
		{
			while (position < input.Length && !char.IsWhiteSpace(input[position]))
			{
				position++;
			}
			return position;
		}

		private int GetFirstNonWhitespace(string input, int position)
		{
			while (position < input.Length && char.IsWhiteSpace(input[position]))
			{
				position++;
			}
			return position;
		}
		
		/// <summary> 
		/// Detect the position of the first words of sentences in a string.
		/// </summary>
		/// <param name="input">
		/// The string to be processed.
		/// </param>
		/// <returns>
		/// A integer array containing the positions of the end index of
		/// every sentence
		/// </returns>
		public virtual int[] SentencePositionDetect(string input)
		{
		    if (string.IsNullOrEmpty(input)){ return new int[] {}; }
			var buffer = new StringBuilder(input);
			List<int> endersList = _scanner.GetPositions(input);
			var positions = new List<int>(endersList.Count);
			
			for (int currentEnder = 0, enderCount = endersList.Count, index = 0; currentEnder < enderCount; currentEnder++)
			{
				int candidate = endersList[currentEnder];
				int cInt = candidate;
								
				// skip over the leading parts of non-token final delimiters
				int firstWhiteSpace = GetFirstWhitespace(input, cInt + 1);
				if (((currentEnder + 1) < enderCount) && ((endersList[currentEnder + 1]) < firstWhiteSpace)) 
				{
					continue;
				}

                var pair = new Tuple<StringBuilder, int>(buffer, candidate);
			    var context = _contextGenerator.GetContext(pair);
				double[] probabilities = _model.Evaluate(context);
				string bestOutcome = _model.GetBestOutcome(probabilities);
				if (bestOutcome.Equals("T") && IsAcceptableBreak(input, index, cInt))
				{
					if (index != cInt)
					{
						positions.Add(GetFirstNonWhitespace(input, GetFirstWhitespace(input, cInt + 1)));
					}
					index = cInt + 1;
				}
			}
			
            return positions.ToArray();
		}
		
		/// <summary>
		/// Allows subclasses to check an overzealous (read: poorly
		/// trained) model from flagging obvious non-breaks as breaks based
		/// on some boolean determination of a break's acceptability.
		/// 
		/// <p>The implementation here always returns true, which means
		/// that the IMaximumEntropyModel's outcome is taken as is.</p>
		/// </summary>
		/// <param name="input">
		/// the string in which the break occured. 
		/// </param>
		/// <param name="fromIndex">
		/// the start of the segment currently being evaluated 
		/// </param>
		/// <param name="candidateIndex">
		/// the index of the candidate sentence ending 
		/// </param>
		/// <returns> true if the break is acceptable 
		/// </returns>
		protected internal virtual bool IsAcceptableBreak(string input, int fromIndex, int candidateIndex)
		{
			return true;
		}


        // Utilities ----------------------------
		
		/// <summary>
		/// Use this training method if you wish to supply an end of
		/// sentence scanner which provides a different set of ending chars
		/// other than the default ones. They are "\\.|!|\\?|\\\"|\\)".
		/// </summary>
		public static GisModel TrainModel(string filePath, int iterations, int cut, IEndOfSentenceScanner scanner)
		{
		    return TrainModel(new List<string>() {filePath}, iterations, cut, scanner);
		}

        public static GisModel TrainModel(IEnumerable<string> filePaths, int iterations, int cut, IEndOfSentenceScanner scanner)
        {
            var trainer = new GisTrainer();

            var readers = filePaths.Select(path => new StreamReader(path)).ToList();

            // train the model
            ITrainingDataReader<string> dataReader = new MultipleFilesPlainTextByLineDataReader(readers);
            ITrainingEventReader eventReader = new SentenceDetectionEventReader(dataReader, scanner);

            trainer.TrainModel(eventReader, iterations, cut);

            return new GisModel(trainer);
        }
		
	}
}
