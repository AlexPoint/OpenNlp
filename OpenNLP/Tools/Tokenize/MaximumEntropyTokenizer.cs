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

//This file is based on the TokenizerME.java source file found in the
//original java implementation of OpenNLP.  That source file contains the following header:

// Copyright (C) 2002 Jason Baldridge, Gann Bierner, and Tom Morton
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
using System.Text.RegularExpressions;
using OpenNLP.Tools.Util;
using SharpEntropy;

namespace OpenNLP.Tools.Tokenize
{
	/// <summary>
	/// A Tokenizer for converting raw text into separated tokens.  It uses
	/// Maximum Entropy to make its decisions.  The features are loosely
	/// based on Jeff Reynar's UPenn thesis "Topic Segmentation:
	/// Algorithms and Applications.", which is available from his
	/// homepage: http://www.cis.upenn.edu/~jcreynar.
	/// </summary>
	public class MaximumEntropyTokenizer : AbstractTokenizer
	{
        internal static Regex AlphaNumeric = new Regex("^[A-Za-z0-9]+$", RegexOptions.Compiled);

		/// <summary>
		/// the maximum entropy model to use to evaluate contexts.
		/// </summary>
		private readonly IMaximumEntropyModel _model;
		
		/// <summary>
		/// The context generator.
		/// </summary>
        private readonly IContextGenerator<Tuple<string, int>> _contextGenerator;
		
		/// <summary>
        /// Optimization flag to skip alpha numeric tokens for further tokenization.
		/// </summary>
		public bool AlphaNumericOptimization { get; set; }
		
		/// <summary>
		/// Class constructor which takes the string locations of the
		/// information which the maxent model needs.
		/// </summary>
		public MaximumEntropyTokenizer(IMaximumEntropyModel model)
		{
			_contextGenerator = new TokenContextGenerator();
			AlphaNumericOptimization = false;
			this._model = model;
		}
		
		/// <summary>Tokenizes the string</summary>
		/// <param name="input">The string to be tokenized</param>
		/// <returns>A span array containing individual tokens as elements</returns>
		public override Span[] TokenizePositions(string input)
		{
            if (string.IsNullOrEmpty(input)) { return new Span[0]; }

			var tokens = SplitOnWhitespaces(input);
			var newTokens = new List<Span>();
			var tokenProbabilities = new List<double>();
			
			for (int currentToken = 0, tokenCount = tokens.Length; currentToken < tokenCount; currentToken++)
			{
				var tokenSpan = tokens[currentToken];
				string token = input.Substring(tokenSpan.Start, (tokenSpan.End) - (tokenSpan.Start));
				// Can't tokenize single characters
				if (token.Length < 2)
				{
					newTokens.Add(tokenSpan);
					tokenProbabilities.Add(1.0);
				}
				else if (AlphaNumericOptimization && AlphaNumeric.IsMatch(token))
				{
					newTokens.Add(tokenSpan);
					tokenProbabilities.Add(1.0);
				}
				else
				{
					int startPosition = tokenSpan.Start;
					int endPosition = tokenSpan.End;
					int originalStart = tokenSpan.Start;
					double tokenProbability = 1.0;
					for (int currentPosition = originalStart + 1; currentPosition < endPosition; currentPosition++)
					{
					    var context = _contextGenerator.GetContext(new Tuple<string, int>(token, currentPosition - originalStart));
						double[] probabilities = _model.Evaluate(context);
						string bestOutcome = _model.GetBestOutcome(probabilities);
						
						tokenProbability *= probabilities[_model.GetOutcomeIndex(bestOutcome)];
						if (bestOutcome == TokenContextGenerator.SplitIndicator)
						{
							newTokens.Add(new Span(startPosition, currentPosition));
							tokenProbabilities.Add(tokenProbability);
							startPosition = currentPosition;
							tokenProbability = 1.0;
						}
					}
					newTokens.Add(new Span(startPosition, endPosition));
					tokenProbabilities.Add(tokenProbability);
				}
			}
			
			return newTokens.ToArray();
		}

	    
        // Utilities --------------------------------------------
        
        /// <summary>
        /// Trains a tokenizer model from the "events" in the input file
        /// and write the resulting gis model in the ouput file (as binary)
        /// </summary>
        public static GisModel Train(string inputFilePath, int iterations, int cut, char splitMarker = '|', bool includeAllCapsExamples = false)
        {
            return Train(new List<string>() {inputFilePath}, iterations, cut, splitMarker);
        }

	    /// <summary>
	    /// Trains a tokenizer model from input files well formatted for
	    /// a token event reader.
	    /// </summary>
	    /// <param name="inputFilePaths">The collection of training input files</param>
	    /// <param name="iterations">The number of iterations to run when training the model</param>
	    /// <param name="cut">The minimum nb of occurences for statistical relevancy in the trained model</param>
	    /// <param name="splitMarker">The character indicating a split in the files</param>
	    /// <returns>The freshly trained GisModel</returns>
        public static GisModel Train(IEnumerable<string> inputFilePaths, int iterations, int cut, char splitMarker = '|', bool includeAllCapsExamples = false)
	    {
	        var trainer = new GisTrainer(0.1);

            var dataReaders = new List<StreamReader>();
	        foreach (var path in inputFilePaths)
	        {
                var dataReader = new StreamReader(path);
                dataReaders.Add(dataReader);
	        }

            // train the model
            var eventReader = new MultipleFileTokenEventReader(dataReaders, splitMarker, includeAllCapsExamples);
            trainer.TrainModel(iterations, new TwoPassDataIndexer(eventReader, cut));

            return new GisModel(trainer);
	    }
	}
}
